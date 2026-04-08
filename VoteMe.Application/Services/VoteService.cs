using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
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
        public async Task<ApiResponse<bool>> CastVoteAsync(Guid candidateId)
        {
            if (candidateId == Guid.Empty)
                throw new BadRequestException("Vote data is required");

            var userId = _currentUserService.UserId;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new UnauthorizedException("Your account is not found or has been deleted");

            var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
            if (candidate == null)
                throw new NotFoundException("Candidate not found");

            var category = await _unitOfWork.ElectionCategories.GetByIdAsync(candidate.ElectionCategoryId);
            if (category == null)
                throw new NotFoundException("Election category not found");

            var election = await _unitOfWork.Elections.GetByIdAsync(category.ElectionId);
            if (election == null)
                throw new NotFoundException("Election not found");

            //if (election.EndDate <= DateTime.UtcNow)
            //    throw new BadRequestException("Election has already closed");

            if (election.Status != ElectionStatus.Active)
                throw new BadRequestException("Voting is not currently open for this election");

            var membership = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(election.OrganizationId,userId);
            var role = membership.Role;

            if (membership == null)
                throw new ForbiddenException("You are not a member of this organization");

            if (membership.Status == MembershipStatus.Pending)
                throw new ForbiddenException("Your membership is pending admin approval");

            if (membership.Status == MembershipStatus.Rejected)
                throw new ForbiddenException("Your membership request was rejected");

            //if (membership.Status == MembershipStatus.Banned)
            //    throw new ForbiddenException("You have been banned from this organization");

            var existingVote = await _unitOfWork.Votes.FindOneAsync(
                v => v.VoterId == userId && v.ElectionCategoryId == category.Id);

            if (existingVote != null)
            {
                if (existingVote.CandidateId == candidateId)
                    throw new BadRequestException("You have already voted for this candidate");

                var oldCandidateId = existingVote.CandidateId;
                existingVote.CandidateId = candidateId;

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
                    NewCandidateId = candidateId,
                    VoterDisplayName = membership.DisplayName,
                    VoterEmail = user.Email ?? string.Empty
                });

                return ApiResponse<bool>.SuccessResponse(true, "Vote changed successfully");
            }

            var vote = new Vote
            {
                VoterId = userId,
                CandidateId = candidateId,
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
                VoterDisplayName = membership.DisplayName,
                VoterEmail = user.Email ?? string.Empty,
                ElectionId = election.Id,
                ElectionName = election.Name,
                ElectionCategoryId = category.Id,
                ElectionCategoryName = category.Name,
                CandidateId = candidateId,
                CandidateFirstName = candidate.FirstName,
                CandidateLastName = candidate.LastName,
                IsPrivate = election.IsPrivate
            });

            return ApiResponse<bool>.SuccessResponse(true, "Vote cast successfully");
        }

        public async Task<ApiResponse<int>> GetOrganizationVotesCount(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                throw new BadRequestException("Bad request: OrganizationId needed");
            var organization = await _unitOfWork.Organizations.FindOneAsync
                (
                 predicate: o => o.Id == organizationId
                );

            if (organization == null)
                throw new NotFoundException("Org not found");

            var totalCount = await _unitOfWork.Votes.GetOrganizationVotesCount(organizationId);

            return ApiResponse<int>.SuccessResponse(totalCount, "Total Vote count retrieved successfully");
        }
    }
}
