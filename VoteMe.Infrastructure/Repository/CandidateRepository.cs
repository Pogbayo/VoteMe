
using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interfaces.Repositories;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
    {

        public CandidateRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Candidate>> GetElectionCandidatesAsync(Guid electionId)
        {
            return await _context.Elections
                  .Where(e => e.Id == electionId)
                  .SelectMany(e => e.Candidates)
                  .ToListAsync();
        }

        public async Task<int> GetVoteCountAsync(Guid candidateId, Guid electionId)
        {
            return await _context.Votes
                .CountAsync(v => v.CandidateId == candidateId && v.ElectionId == electionId);
        }

        public async Task<Candidate?> GetWithVotesAsync(Guid candidateId, Guid electionId)
        {
            return await _dbSet
                .Include(c => c.Votes)
                .FirstOrDefaultAsync(c => c.Id == candidateId && c.ElectionId == electionId);
        }
    }
}
