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

            var candidate = await _unitOfWork.Candidates.GetByIdAsync(dto.CandidateId);
            if (candidate == null || candidate.IsDeleted)
                throw new NotFoundException("Candidate not found");

            var category = await _unitOfWork.ElectionCategories.GetByIdAsync(candidate.ElectionCategoryId);
            if (category == null || category.IsDeleted)
                throw new NotFoundException("Election category not found");

            var election = await _unitOfWork.Elections.GetByIdAsync(category.ElectionId);
            if (election == null || election.IsDeleted)
                throw new NotFoundException("Election not found");

            if (election.Status != ElectionStatus.Active)
                throw new BadRequestException("Voting is not currently open for this election");

            var membership = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(userId, election.OrganizationId);

            if (membership == null)
                throw new ForbiddenException("You are not a member of this organization");

            if (membership.Status == MembershipStatus.Pending)
                throw new ForbiddenException("Your membership is pending admin approval");

            if (membership.Status == MembershipStatus.Rejected)
                throw new ForbiddenException("Your membership request was rejected");

            if (membership.Status == MembershipStatus.Banned)
                throw new ForbiddenException("You have been banned from this organization");

            var existingVote = await _unitOfWork.Votes.FindOneAsync(
                v => v.VoterId == userId && v.ElectionCategoryId == category.Id);

            if (existingVote != null)
            {
                var oldCandidateId = existingVote.CandidateId;
                existingVote.CandidateId = dto.CandidateId;
                existingVote.UpdateTimestamps();
                _unitOfWork.Votes.Update(existingVote);
                await _unitOfWork.SaveChangesAsync();

                await _messageBus.PublishAsync("vote-changed", new VoteChangedEvent
                {
                    VoterId = userId,
                    ElectionId = election.Id,
                    ElectionName = election.Name,
                    ElectionCategoryId = category.Id,
                    OldCandidateId = oldCandidateId,
                    NewCandidateId = dto.CandidateId,
                    VoterDisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}",
                    VoterEmail = user.Email ?? string.Empty
                });

                return ApiResponse<bool>.SuccessResponse(true, "Vote changed successfully");
            }

            var vote = new Vote
            {
                VoterId = userId,
                CandidateId = dto.CandidateId,
                ElectionCategoryId = category.Id,
                ElectionId = election.Id,
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
                ElectionId = election.Id,
                ElectionName = election.Name,
                ElectionCategoryId = category.Id,
                ElectionCategoryName = category.Name,
                CandidateId = dto.CandidateId,
                CandidateFirstName = candidate.FirstName,
                CandidateLastName = candidate.LastName,
                IsPrivate = election.IsPrivate
            });

            return ApiResponse<bool>.SuccessResponse(true, "Vote cast successfully");
        }
    }
}
