using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class VoteRepository : GenericRepository<Vote>, IVoteRepository
    {
        public VoteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Vote?> ChangeVoteAsync(Guid userId, Guid electionId, Guid newCandidateId)
        {
            var existingVote = await _dbSet
                .FirstOrDefaultAsync(v => v.VoterId == userId && v.ElectionId == electionId);

            if (existingVote == null)
                return null;

            existingVote.CandidateId = newCandidateId;
            existingVote.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(existingVote);

            return existingVote;
        }

        public async Task<IEnumerable<Vote>> GetElectionVotesAsync(Guid electionId)
        {
            return await _dbSet
                .Where(v => v.ElectionId == electionId)
                .ToListAsync();
        }

        public async Task<int> GetTotalVotesAsync(Guid electionId)
        {
            return await _dbSet
                .CountAsync(v => v.ElectionId == electionId);
        }

        public async Task<Dictionary<Guid, int>> GetVoteCountsAsync(Guid electionId)
        {
            return await _dbSet
                .Where(v => v.ElectionId == electionId)
                .GroupBy(v => v.CandidateId)
                .Select(g => new { CandidateId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CandidateId, x => x.Count);
        }

        public async Task<bool> HasUserVotedAsync(Guid userId, Guid electionId)
        {
            return await _dbSet
                .AnyAsync(v => v.VoterId == userId && v.ElectionId == electionId);
        }

        Task IVoteRepository.ChangeVoteAsync(Guid userId, Guid electionCategoryId, Guid newCandidateId)
        {
            return ChangeVoteAsync(userId, electionCategoryId, newCandidateId);
        }
    }
}