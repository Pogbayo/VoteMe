using Microsoft.Extensions.Logging;
using VoteMe.Application.DTOs.Candidate;
using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Application.Events.Election;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Enum;

namespace VoteMe.Infrastructure.Jobs
{
    public class ElectionJobService : IElectionJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBus _messageBus;
        private readonly ILogger<ElectionJobService> _logger;

        public ElectionJobService(
            IUnitOfWork unitOfWork,
            IMessageBus messageBus,
            ILogger<ElectionJobService> logger)
        {
            _unitOfWork = unitOfWork;
            _messageBus = messageBus;
            _logger = logger;
        }

        public async Task OpenElectionAsync(Guid electionId)
        {
            var election = await _unitOfWork.Elections.GetWithCategoriesAsync(electionId);
            if (election == null)
            {
                _logger.LogWarning("OpenElectionJob: Election {ElectionId} not found", electionId);
                return;
            }

            if (election.Status != ElectionStatus.Pending)
            {
                _logger.LogWarning("OpenElectionJob: Election {ElectionId} is not Pending — skipping", electionId);
                return;
            }

            election.Status = ElectionStatus.Active;
            election.UpdateTimestamps();

            _unitOfWork.Elections.Update(election);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Election '{Name}' automatically opened", election.Name);

            var memberEmails = await _unitOfWork.OrganizationMembers
                .GetOrganizationMemberEmailsAsync(election.OrganizationId);

            var categoryNames = election.Categories
                .Select(c => c.Name)
                .ToList();

            await _messageBus.PublishAsync("election-opened", new ElectionOpenedEvent
            {
                ElectionId = election.Id,
                OpenedByUserId = Guid.Empty,
                ElectionName = election.Name,
                OrganizationName = string.Empty,
                ElectionCategoryNames = categoryNames,
                MemberEmails = memberEmails.ToList()
            });
        }

        public async Task CloseElectionAsync(Guid electionId)
        {
            var election = await _unitOfWork.Elections.GetFullElectionAsync(electionId);
            if (election == null)
            {
                _logger.LogWarning("CloseElectionJob: Election {ElectionId} not found", electionId);
                return;
            }

            if (election.Status != ElectionStatus.Active)
            {
                _logger.LogWarning("CloseElectionJob: Election {ElectionId} is not Active — skipping", electionId);
                return;
            }

            election.Status = ElectionStatus.Closed;
            election.UpdateTimestamps();

            _unitOfWork.Elections.Update(election);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Election '{Name}' automatically closed", election.Name);

            var memberEmails = await _unitOfWork.OrganizationMembers
                .GetOrganizationMemberEmailsAsync(election.OrganizationId);

            var totalVotes = election.Categories.Sum(c => c.Votes.Count);

            var categoryResults = election.Categories.Select(c =>
            {
                var catTotalVotes = c.Votes.Count;
                var winner = c.Candidates
                    .OrderByDescending(cand => cand.Votes.Count)
                    .FirstOrDefault();

                return new ElectionCategoryResultDto
                {
                    ElectionCategoryId = c.Id,
                    ElectionCategoryName = c.Name,
                    TotalVotes = catTotalVotes,
                    Winner = winner == null ? null : new WinnerDto
                    {
                        CandidateId = winner.Id,
                        FirstName = winner.FirstName,
                        LastName = winner.LastName,
                        DisplayName = winner.DisplayName,
                        VoteCount = winner.Votes.Count,
                        Percentage = catTotalVotes == 0 ? 0 :
                            Math.Round((double)winner.Votes.Count / catTotalVotes * 100, 2)
                    }
                };
            }).ToList();

            await _messageBus.PublishAsync("election-closed", new ElectionClosedEvent
            {
                ElectionId = election.Id,
                ClosedByUserId = Guid.Empty,
                ElectionName = election.Name,
                OrganizationName = string.Empty,
                TotalVotes = totalVotes,
                CategoryResults = categoryResults,
                MemberEmails = memberEmails.ToList()
            });
        }
    }
}
