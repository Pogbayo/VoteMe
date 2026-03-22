using Microsoft.Extensions.Logging;
using VoteMe.Application.Authorization;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Candidate;
using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Application.Events.ElectionCategory;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.Candidate;
using VoteMe.Application.Mappers.ElectionCategory;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services;

public class ElectionCategoryService : IElectionCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageBus _messageBus;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ElectionCategoryService> _logger;

    public ElectionCategoryService(
        ILogger<ElectionCategoryService> logger,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<ElectionCategoryDto>> GetElectionCategoryAsync(Guid electionCategoryId)
    {
        var electionCategory = await _unitOfWork.ElectionCategories.GetElectionCategoryAsync(electionCategoryId);
        if (electionCategory == null)
            throw new NotFoundException("ElectionCategory not found");

        return ApiResponse<ElectionCategoryDto>.SuccessResponse(
            ElectionCategoryMapper.ToDto(electionCategory),
            "ElectionCategory retrieved successfully");
    }

    public async Task<ApiResponse<IEnumerable<ElectionCategoryDto>>> GetElectionCategoriesAsync(Guid electionId)
    {
        var election = await _unitOfWork.Elections.GetByIdAsync(electionId);
        if (election == null)
            throw new NotFoundException("Election not found");

        var cacheKey = $"election-categories-{electionId}";
        var cached = await _cacheService.GetAsync<IEnumerable<ElectionCategoryDto>>(cacheKey);
        if (cached != null)
        {
            return ApiResponse<IEnumerable<ElectionCategoryDto>>.SuccessResponse(
                cached, "ElectionCategories retrieved successfully (from cache)");
        }

        var electionCategories = await _unitOfWork.ElectionCategories.GetElectionCategoriesAsync(electionId);
        var dtos = ElectionCategoryMapper.ToDtoList(electionCategories);

        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10));

        return ApiResponse<IEnumerable<ElectionCategoryDto>>.SuccessResponse(
            dtos, "ElectionCategories retrieved successfully");
    }

    public async Task<ApiResponse<ElectionCategoryResultDto>> GetElectionCategoryResultsAsync(Guid electionCategoryId)
    {
        var electionCategory = await _unitOfWork.ElectionCategories.GetElectionCategoryResultsAsync(electionCategoryId);

        if (electionCategory == null)
            throw new NotFoundException("ElectionCategory not found");

        var cacheKey = $"election-category-results-{electionCategoryId}";
        var cached = await _cacheService.GetAsync<ElectionCategoryResultDto>(cacheKey);
        if (cached != null)
        {
            return ApiResponse<ElectionCategoryResultDto>.SuccessResponse(
                cached, "ElectionCategory results retrieved successfully (from cache)");
        }

        var totalVotes = electionCategory.Candidates!.Count;

        var candidateResults = electionCategory.Candidates
            ?.Select(c => CandidateMapper.ToResultDto(c, c.Votes?.Count ?? 0, totalVotes))
            .OrderByDescending(r => r.VoteCount)
            .ToList() ?? new List<CandidateResultDto>();

        var result = ElectionCategoryMapper.ToResultDto(electionCategory, candidateResults);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return ApiResponse<ElectionCategoryResultDto>.SuccessResponse(
            result, "ElectionCategory results retrieved successfully");
    }

    public async Task<ApiResponse<ElectionCategoryDto>> CreateElectionCategoryAsync( CreateElectionCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("ElectionCategory name is required");

        var election = await _unitOfWork.Elections.GetByIdAsync(dto.ElectionId);
        if (election == null)
            throw new NotFoundException("Election not found");

        await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(_unitOfWork,_currentUserService,election.OrganizationId,"create ElectionCategories");

        var electionCategory = new ElectionCategory
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            ElectionId = dto.ElectionId
        };

        await _unitOfWork.ElectionCategories.AddAsync(electionCategory);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"election-categories-{dto.ElectionId}");

        _logger.LogInformation(
            "ElectionCategory '{CategoryName}' created in election '{ElectionName}' by user {UserId}",
            electionCategory.Name, election.Name, _currentUserService.UserId);

        await _messageBus.PublishAsync("election-category-created", new ElectionCategoryCreatedEvent
        {
            CreatedByUserId = _currentUserService.UserId,
            ElectionCategoryName = electionCategory.Name,
            ElectionName = election.Name
        });

        return ApiResponse<ElectionCategoryDto>.SuccessResponse(
            ElectionCategoryMapper.ToDto(electionCategory),
            "ElectionCategory created successfully");
    }

    public async Task<ApiResponse<ElectionCategoryDto>> UpdateElectionCategoryAsync(Guid electionCategoryId, UpdateElectionCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("ElectionCategory name is required");

        var electionCategory = await _unitOfWork.ElectionCategories.GetByIdAsync(electionCategoryId);
        if (electionCategory == null)
            throw new NotFoundException("ElectionCategory not found");

        var election = await _unitOfWork.Elections.GetByIdAsync(electionCategory.ElectionId);
        if (election == null)
            throw new NotFoundException("Election not found");

        await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(
            _unitOfWork,
            _currentUserService,
            election.OrganizationId,
            "update ElectionCategories");

        electionCategory.Name = dto.Name.Trim();
        electionCategory.Description = dto.Description?.Trim() ?? string.Empty;
        electionCategory.UpdateTimestamps();

        _unitOfWork.ElectionCategories.Update(electionCategory);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"election-categories-{electionCategory.ElectionId}");
        await _cacheService.RemoveAsync($"election-category-{electionCategoryId}");

        _logger.LogInformation(
            "ElectionCategory '{CategoryName}' updated in election '{ElectionName}' by user {UserId}",
            electionCategory.Name, election.Name, _currentUserService.UserId);

        await _messageBus.PublishAsync("election-category-updated", new ElectionCategoryUpdatedEvent
        {
            UpdatedByUserId = _currentUserService.UserId,
            ElectionCategoryName = electionCategory.Name,
            ElectionName = election.Name
        });

        return ApiResponse<ElectionCategoryDto>.SuccessResponse(
            ElectionCategoryMapper.ToDto(electionCategory),
            "ElectionCategory updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteElectionCategoryAsync(Guid electionCategoryId)
    {
        var electionCategory = await _unitOfWork.ElectionCategories.GetByIdAsync(electionCategoryId);
        if (electionCategory == null)
            throw new NotFoundException("ElectionCategory not found");

        var election = await _unitOfWork.Elections.GetByIdAsync(electionCategory.ElectionId);
        if (election == null)
            throw new NotFoundException("Election not found");

        await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(
            _unitOfWork,
            _currentUserService,
            election.OrganizationId,
            "delete ElectionCategories");

        await _unitOfWork.ElectionCategories.SoftDeleteByIdAsync(electionCategoryId);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"election-categories-{electionCategory.ElectionId}");
        await _cacheService.RemoveAsync($"election-category-{electionCategoryId}");
        await _cacheService.RemoveAsync($"election-category-results-{electionCategoryId}");
        await _cacheService.RemoveAsync($"election-results-{electionCategory.ElectionId}");

        _logger.LogInformation(
            "ElectionCategory '{CategoryName}' deleted from election '{ElectionName}' by user {UserId}",
            electionCategory.Name, election.Name, _currentUserService.UserId);

        await _messageBus.PublishAsync("election-category-deleted", new ElectionCategoryDeletedEvent
        {
            DeletedByUserId = _currentUserService.UserId,
            ElectionCategoryName = electionCategory.Name,
            ElectionName = election.Name
        });

        return ApiResponse<bool>.SuccessResponse(true, "ElectionCategory deleted successfully");
    }
}