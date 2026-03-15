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

        public async Task<Vote?> ChangeVoteAsync(Guid userId, Guid electionCategoryId, Guid newCandidateId)
        {
            var existingVote = await _dbSet
                .FirstOrDefaultAsync(v => v.VoterId == userId && v.ElectionCategoryId == electionCategoryId);

            if (existingVote == null)
                return null;

            existingVote.CandidateId = newCandidateId;
            existingVote.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(existingVote);

            return existingVote;
        }


        public async Task<int> GetCandidateTotalVotesInAnElectionCategoryAsync(Guid candidateId, Guid electionCategoryId)
        {
            return await _context.Candidates
                .CountAsync(v => v.Id == candidateId);
        }

        public async Task<Dictionary<Guid, int>> GetVoteCountsAsync(Guid electionId)
        {
            return await _dbSet
                .Where(v => v.ElectionId == electionId)
                .GroupBy(v => v.CandidateId)
                .Select(g => new { CandidateId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CandidateId, x => x.Count);
        }

        public async Task<bool> HasUserVotedAsync(Guid userId, Guid electionCategoryId, Guid electionId)
        {
            return await _dbSet
                .AnyAsync(v => v.VoterId == userId && v.ElectionId == electionId && v.ElectionCategoryId == electionCategoryId );
        }
    }
}