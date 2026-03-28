using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Election;
using VoteMe.Application.Events.Election;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.Candidate;
using VoteMe.Application.Mappers.Election;
using VoteMe.Application.Mappers.ElectionCategory;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services
{
    public class ElectionService : IElectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessageBus _messageBus;
        private readonly IElectionScheduler _electionScheduler;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ElectionService> _logger;

        public ElectionService(
            ILogger<ElectionService> logger,
            IUnitOfWork unitOfWork,
            IMessageBus messageBus,
            ICurrentUserService currentUserService,
            IElectionScheduler electionScheduler,
            ICacheService cacheService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _messageBus = messageBus;
            _currentUserService = currentUserService;
            _electionScheduler = electionScheduler;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<ElectionDto>> GetElectionAsync(Guid electionId)
        {
            var cacheKey = $"election-{electionId}";
            var cached = await _cacheService.GetAsync<ElectionDto>(cacheKey);
            if (cached != null)
                return ApiResponse<ElectionDto>.SuccessResponse(cached, "Election retrieved successfully");

            var election = await _unitOfWork.Elections.GetWithCategoriesAsync(electionId);
            if (election == null)
                throw new NotFoundException("Election not found");

            var result = ElectionMapper.ToDto(election);
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(1));

            return ApiResponse<ElectionDto>.SuccessResponse(result, "Election retrieved successfully");
        }
        public async Task<ApiResponse<ElectionDto>> CreateElectionAsync( CreateElectionDto dto)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(dto.OrganizationId);
            if (organization == null)
                throw new NotFoundException("Organization not found");

            var election = new Election
            {
                Name = dto.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? string.Empty
                    : dto.Description.Trim(),
                EndDate = dto.EndDate,
                OrganizationId = dto.OrganizationId,
                Status = ElectionStatus.Pending,
            };

            await _unitOfWork.Elections.AddAsync(election);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Election '{Name}' created for organization '{OrgName}'",
                election.Name, organization.Name);
            await _cacheService.RemoveAsync($"organization-elections-{dto.OrganizationId}");


            var memberEmails = await _unitOfWork.OrganizationMembers
                .GetOrganizationMemberEmailsAsync(dto.OrganizationId);

            await _messageBus.PublishAsync("election-created", new ElectionCreatedEvent
            {
                ElectionId = election.Id,
                CreatedByUserId = _currentUserService.UserId,
                ElectionName = election.Name,
                OrganizationName = organization.Name,
                ElectionCategoryNames = new List<string>(),
                MemberEmails = memberEmails.ToList()
            });

            return ApiResponse<ElectionDto>.SuccessResponse(
                ElectionMapper.ToDto(election),
                "Election created successfully");
        }
        public async Task<ApiResponse<ElectionDto>> UpdateElectionAsync(Guid electionId, UpdateElectionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Election name is required");

            var election = await _unitOfWork.Elections.GetByIdAsync(electionId);
            if (election == null)
                throw new NotFoundException("Election not found");

            if (election.Status != ElectionStatus.Pending)
                throw new BadRequestException("Only pending elections can be updated");

            election.Name = dto.Name.Trim();
            election.Description = string.IsNullOrWhiteSpace(dto.Description)
                ? string.Empty
                : dto.Description.Trim();
            election.IsPrivate = dto.IsPrivate;
            election.UpdateTimestamps();

            _unitOfWork.Elections.Update(election);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"election-{electionId}");
            await _cacheService.RemoveAsync($"organization-elections-{election.OrganizationId}");

            _logger.LogInformation("Election '{Name}' updated", election.Name);

            await _messageBus.PublishAsync("election-updated", new ElectionUpdatedEvent
            {
                ElectionId = election.Id,
                UpdatedByUserId = _currentUserService.UserId,
                ElectionName = election.Name,
                OrganizationName = string.Empty
            });

            return ApiResponse<ElectionDto>.SuccessResponse(
                ElectionMapper.ToDto(election),
                "Election updated successfully");
        }
        public async Task<ApiResponse<PagedElectionResponse>> GetOrganizationElectionsAsync(Guid organizationId, int page = 1, int pageSize = 20)
        {
            if (organizationId == Guid.Empty)
                throw new BadRequestException("Organization with provided Id does not exist");

            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null)
                throw new NotFoundException("Organization not found");

            var cacheKey = $"organization-elections-{organizationId}";
            var cached = await _cacheService.GetAsync<PagedElectionResponse>(cacheKey);

            _logger.LogInformation($"Total elections for organization {organizationId}: {cached}");
            if (cached != null && cached.Items != null && cached.Items.Any())
            {
                return ApiResponse<PagedElectionResponse>.SuccessResponse(cached, "From cache");
            }
            var (items, totalCount) = await _unitOfWork.Elections.GetOrganizationElectionsAsync(organizationId, page, pageSize);
            _logger.LogInformation($"Total elections for organization {organizationId}: {totalCount}. Elections: {@items}");

            var paged = new PagedElectionResponse
            {
                Items = ElectionMapper.ToDtoList(items),
                TotalCount = totalCount
            };

            await _cacheService.SetAsync(cacheKey, paged, TimeSpan.FromMinutes(10));

            return ApiResponse<PagedElectionResponse>.SuccessResponse(paged, "Elections retrieved successfully");
        }
        public async Task<ApiResponse<bool>> DeleteElectionAsync(Guid electionId)
        {
            var election = await _unitOfWork.Elections.GetByIdAsync(electionId);
            if (election == null)
                throw new NotFoundException("Election not found");

            if (election.Status == ElectionStatus.Active)
                throw new BadRequestException("Cannot delete an active election");

            await _unitOfWork.Elections.SoftDeleteByIdAsync(electionId);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"election-{electionId}");
            await _cacheService.RemoveAsync($"organization-elections-{election.OrganizationId}");
            await _cacheService.RemoveAsync($"election-results-{electionId}");

            _logger.LogInformation("Election '{Name}' deleted", election.Name);

            return ApiResponse<bool>.SuccessResponse(true, "Election deleted successfully");
        }
        public async Task<ApiResponse<ElectionResultDto>> GetElectionResultsAsync(Guid electionId)
        {
            var cacheKey = $"election-results-{electionId}";
            var cached = await _cacheService.GetAsync<ElectionResultDto>(cacheKey);
            if (cached != null)
                return ApiResponse<ElectionResultDto>.SuccessResponse(cached, "Election results retrieved successfully");

            var election = await _unitOfWork.Elections.GetFullElectionAsync(electionId);
            if (election == null)
                throw new NotFoundException("Election not found");

            if (election.Status == ElectionStatus.Pending)
                throw new BadRequestException("Election is still pending. You will receive the results once the election closes.");

            if (election.Status == ElectionStatus.Active)
                throw new BadRequestException("Election is still active. Results are not available until the election is closed.");

            var totalVotes = election.Categories?
                     .Sum(c => c.Candidates?.Sum(cand => cand.Votes.Count));

            var categoryResults = election.Categories?.Select(c =>
            {
                var catTotalVotes = c.Candidates?.
                       Sum(cand => cand.Votes.Count) ?? 0;

                var candidateResults = c.Candidates?
                    .Select(cand => CandidateMapper.ToResultDto(cand, cand.Votes.Count, catTotalVotes))
                    .OrderByDescending(r => r.VoteCount)
                    .ToList();

                return ElectionCategoryMapper.ToResultDto(c, candidateResults!);
            }).ToList();

            var result = ElectionMapper.ToResultDto(election, categoryResults!);

            if (election.Status == ElectionStatus.Closed)
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(1));

            return ApiResponse<ElectionResultDto>.SuccessResponse(result, "Election results retrieved successfully");
        }
        public async Task<ApiResponse<ElectionDto>> OpenElectionAsync(Guid electionId)
        {
            var election = await _unitOfWork.Elections.GetWithCategoriesAsync(electionId);
            if (election == null) throw new NotFoundException("Election not found");

            if (election.Status != ElectionStatus.Pending)
                throw new BadRequestException("Election is not in pending status");

            if (election.Categories == null || !election.Categories.Any())
                throw new BadRequestException("Election must have at least one category");

            election.Status = ElectionStatus.Active;
            election.StartDate = DateTime.UtcNow;
            election.UpdateTimestamps();
            _unitOfWork.Elections.Update(election);

            await _unitOfWork.SaveChangesAsync();

            _electionScheduler.ScheduleCloseElection(election.Id, election.EndDate);

            await _messageBus.PublishAsync("election-opened", new ElectionOpenedEvent
            {
                ElectionId = electionId,
                ElectionName = election.Name,
                OrganizationName = election.Organization.Name
            });

            await _cacheService.RemoveAsync($"election-{electionId}");
            await _cacheService.RemoveAsync($"organization-elections-{election.OrganizationId}");

            return ApiResponse<ElectionDto>.SuccessResponse(
                ElectionMapper.ToDto(election),
                "Election opened successfully");
        }
    }
}






