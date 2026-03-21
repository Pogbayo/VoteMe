using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Vote;
using VoteMe.Application.Events.Voting;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services
{
    public class VoteService : IVoteService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VoteService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessageBus _messageBus;
        private readonly UserManager<AppUser> _userManager;

        public VoteService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,UserManager<AppUser> userManager, IMessageBus messageBus, ILogger<VoteService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _userManager = userManager;
        }
        public async Task<ApiResponse<bool>> CastVoteAsync(CastVoteDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Vote data is required");

            var userId = _currentUserService.UserId;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
                throw new UnauthorizedException("Your account is not found or has been deleted");

            var election = await _unitOfWork.Elections.GetByIdAsync(dto.ElectionId);
            if (election == null || election.IsDeleted)
                throw new NotFoundException("Election not found");

            if (election.Status != ElectionStatus.Active)
                throw new BadRequestException("Voting is not currently open for this election");

            var category = await _unitOfWork.ElectionCategories.GetByIdAsync(dto.ElectionCategoryId);
            if (category == null || category.IsDeleted || category.ElectionId != dto.ElectionId)
                throw new NotFoundException("Election category not found or does not belong to this election");

            var candidate = await _unitOfWork.Candidates.GetByIdAsync(dto.CandidateId);
            if (candidate == null || candidate.IsDeleted || candidate.ElectionCategoryId != dto.ElectionCategoryId)
                throw new NotFoundException("Candidate not found or does not belong to this category");

            var existingVote = await _unitOfWork.Votes.HasUserVotedAsync(userId, dto.ElectionCategoryId, dto.ElectionId);

            if (existingVote)
            {
                var oldVote = await _unitOfWork.Votes.FindOneAsync(
                    v => v.VoterId == userId 
                         &&
                         v.ElectionCategoryId == dto.ElectionCategoryId);

                if (oldVote != null)
                {
                    oldVote.CandidateId = dto.CandidateId;
                    oldVote.UpdateTimestamps();
                    _unitOfWork.Votes.Update(oldVote);

                    await _messageBus.PublishAsync("vote-changed", new VoteChangedEvent
                    {
                        VoterId = userId,
                        ElectionId = dto.ElectionId,
                        ElectionName = election.Name,
                        ElectionCategoryId = dto.ElectionCategoryId,
                        OldCandidateId = oldVote.CandidateId,
                        NewCandidateId = dto.CandidateId,
                        VoterDisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}",
                        VoterEmail = user.Email ?? string.Empty
                    });

                    await _unitOfWork.SaveChangesAsync();
                    return ApiResponse<bool>.SuccessResponse(true);
                }
            }

            var vote = new Vote
            {
                VoterId = userId,
                CandidateId = dto.CandidateId,
                ElectionCategoryId = dto.ElectionCategoryId,
                ElectionId = dto.ElectionId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Votes.AddAsync(vote);
            await _unitOfWork.SaveChangesAsync();

            await _messageBus.PublishAsync("vote-cast", new VoteCastEvent
            {
                VoterId = userId,
                VoterFirstName = user.FirstName,
                VoterLastName = user.LastName,
                VoterDisplayName = user.DisplayName ?? string.Empty,
                VoterEmail = user.Email ?? string.Empty,
                ElectionId = dto.ElectionId,
                ElectionName = election.Name,
                ElectionCategoryId = dto.ElectionCategoryId,
                ElectionCategoryName = category.Name,
                CandidateId = dto.CandidateId,
                CandidateFirstName = candidate.FirstName,
                CandidateLastName = candidate.LastName,
                IsPrivate = election.IsPrivate
            });

            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
