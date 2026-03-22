using Microsoft.Extensions.Logging;
using VoteMe.Application.Authorization;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Candidate;
using VoteMe.Application.Events.Candidate;
using VoteMe.Application.Events.Candidate.VoteMe.Application.Events.Candidate;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.Candidate;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services;

public class CandidateService : ICandidateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageBus _messageBus;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CandidateService> _logger;

    public CandidateService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMessageBus messageBus,
        ICacheService cacheService,
        ILogger<CandidateService> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _messageBus = messageBus;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<CandidateDto>> AddCandidateAsync(CreateCandidateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            throw new BadRequestException("First name and last name are required");

        var category = await _unitOfWork.ElectionCategories.GetByIdAsync(dto.ElectionCategoryId);
        if (category == null || category.IsDeleted)
            throw new NotFoundException("Election category not found");

        var election = await _unitOfWork.Elections.GetByIdAsync(category.ElectionId);
        if (election == null || election.IsDeleted)
            throw new NotFoundException("Election not found");

        await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(
            _unitOfWork,
            _currentUserService,
            election.OrganizationId,
            "add candidates");

        if (election.Status != ElectionStatus.Pending)
            throw new BadRequestException("Cannot add candidates after election has started");

        var candidate = new Candidate
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            DisplayName = dto.DisplayName?.Trim(),
            Bio = dto.Bio?.Trim() ?? string.Empty,
            PhotoUrl = dto.PhotoUrl!.Trim(),
            ElectionCategoryId = dto.ElectionCategoryId,
        };

        await _unitOfWork.Candidates.AddAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"election-category-candidates-{dto.ElectionCategoryId}");

        _logger.LogInformation("Candidate '{FullName}' added to category '{CategoryName}' in election '{ElectionName}' by user {UserId}",
            candidate.FirstName + " " + candidate.LastName, category.Name, election.Name, _currentUserService.UserId);

        await _messageBus.PublishAsync("candidate-added", new CandidateAddedEvent
        {
            CandidateId = candidate.Id,
            CandidateFirstName = candidate.FirstName,
            CandidateLastName = candidate.LastName,
            ElectionCategoryName = category.Name,
            ElectionName = election.Name,
            AddedByUserId = _currentUserService.UserId
        });

        return ApiResponse<CandidateDto>.SuccessResponse(
            CandidateMapper.ToDto(candidate),
            "Candidate added successfully");
    }

    public async Task<ApiResponse<bool>> DeleteCandidateAsync(Guid candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
        if (candidate == null || candidate.IsDeleted)
            throw new NotFoundException("Candidate not found");

        var category = await _unitOfWork.ElectionCategories.GetByIdAsync(candidate.ElectionCategoryId);
        if (category == null || category.IsDeleted)
            throw new NotFoundException("Election category not found");

        var election = await _unitOfWork.Elections.GetByIdAsync(category.ElectionId);
        if (election == null || election.IsDeleted)
            throw new NotFoundException("Election not found");

        await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(
            _unitOfWork,
            _currentUserService,
            election.OrganizationId,
            "delete this candidate");

        if (election.Status != ElectionStatus.Pending)
            throw new BadRequestException("Cannot delete candidates after election has started");

        await _unitOfWork.Candidates.SoftDeleteByIdAsync(candidateId);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"election-category-candidates-{candidate.ElectionCategoryId}");

        _logger.LogInformation("Candidate '{FullName}' deleted from category '{CategoryName}' by user {UserId}",
            candidate.FirstName + " " + candidate.LastName, category.Name, _currentUserService.UserId);

        await _messageBus.PublishAsync("candidate-deleted", new CandidateDeletedEvent
        {
            CandidateId = candidateId,
            CandidateFirstName = candidate.FirstName,
            CandidateLastName = candidate.LastName,
            ElectionCategoryName = category.Name,
            ElectionName = election.Name,
            DeletedByUserId = _currentUserService.UserId
        });

        return ApiResponse<bool>.SuccessResponse(true, "Candidate deleted successfully");
    }

    public async Task<ApiResponse<CandidateDto>> GetCandidateAsync(Guid candidateId)
    {
        var cacheKey = $"candidate-{candidateId}";
        var cached = await _cacheService.GetAsync<CandidateDto>(cacheKey);
        if (cached != null)
            return ApiResponse<CandidateDto>.SuccessResponse(cached, "Candidate retrieved successfully (from cache)");

        var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
        if (candidate == null || candidate.IsDeleted)
            throw new NotFoundException("Candidate not found");

        var dto = CandidateMapper.ToDto(candidate);

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15));
        return ApiResponse<CandidateDto>.SuccessResponse(dto, "Candidate retrieved successfully");
    }

    public async Task<ApiResponse<IEnumerable<CandidateDto>>> GetCategoryCandidatesAsync(Guid electionCategoryId)
    {
        var cacheKey = $"election-category-candidates-{electionCategoryId}";
        var cached = await _cacheService.GetAsync<IEnumerable<CandidateDto>>(cacheKey);
        if (cached != null)
            return ApiResponse<IEnumerable<CandidateDto>>.SuccessResponse(cached, "Candidates retrieved successfully (from cache)");

        var category = await _unitOfWork.ElectionCategories.GetByIdAsync(electionCategoryId);
        if (category == null || category.IsDeleted)
            throw new NotFoundException("Election category not found");

        var candidates = await _unitOfWork.Candidates
            .FindAsync(c => c.ElectionCategoryId == electionCategoryId && !c.IsDeleted);

        var dtos = CandidateMapper.ToDtoList(candidates);

        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10));

        return ApiResponse<IEnumerable<CandidateDto>>.SuccessResponse(
            dtos,
            "Candidates retrieved successfully");
    }

    public async Task<ApiResponse<CandidateDto>> UpdateCandidateAsync(Guid candidateId, UpdateCandidateDto dto)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
        if (candidate == null || candidate.IsDeleted)
            throw new NotFoundException("Candidate not found");

        var category = await _unitOfWork.ElectionCategories.GetByIdAsync(candidate.ElectionCategoryId);
        if (category == null || category.IsDeleted)
            throw new NotFoundException("Election category not found");

        var election = await _unitOfWork.Elections.GetByIdAsync(category.ElectionId);
        if (election == null || election.IsDeleted)
            throw new NotFoundException("Election not found");

        await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(
            _unitOfWork,
            _currentUserService,
            election.OrganizationId,
            "update this candidate");

        if (election.Status != ElectionStatus.Pending)
            throw new BadRequestException("Cannot update candidates after election has started");

        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            candidate.FirstName = dto.FirstName.Trim();

        if (!string.IsNullOrWhiteSpace(dto.LastName))
            candidate.LastName = dto.LastName.Trim();

        if (dto.DisplayName != null)
            candidate.DisplayName = dto.DisplayName.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Bio))
            candidate.Bio = dto.Bio.Trim();

        if (!string.IsNullOrWhiteSpace(dto.PhotoUrl))
            candidate.PhotoUrl = dto.PhotoUrl.Trim();

        candidate.UpdateTimestamps();

        _unitOfWork.Candidates.Update(candidate);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"candidate-{candidateId}");
        await _cacheService.RemoveAsync($"election-category-candidates-{candidate.ElectionCategoryId}");

        _logger.LogInformation("Candidate '{FullName}' updated in category '{CategoryName}' by user {UserId}",
            candidate.FirstName + " " + candidate.LastName, category.Name, _currentUserService.UserId);

        await _messageBus.PublishAsync("candidate-updated", new CandidateUpdatedEvent
        {
            CandidateId = candidate.Id,
            CandidateFirstName = candidate.FirstName,
            CandidateLastName = candidate.LastName,
            ElectionCategoryName = category.Name,
            ElectionName = election.Name,
            UpdatedByUserId = _currentUserService.UserId
        });

        return ApiResponse<CandidateDto>.SuccessResponse(
            CandidateMapper.ToDto(candidate),
            "Candidate updated successfully");
    }
}