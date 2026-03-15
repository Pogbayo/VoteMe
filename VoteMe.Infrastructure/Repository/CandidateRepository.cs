using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interface.IRepositories;
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

        public async Task<IEnumerable<Candidate>> GetCategoryCanidatesAsync(Guid electionCategoryId)
        {
            return await _dbSet
                           .Where(c => c.ElectionCategoryId == electionCategoryId)
                           .ToListAsync();
        }

        //public async Task<IEnumerable<Election>> GetElectionCandidatesAsync(Guid electionId)
        //{
        //    return await _context.Elections
        //        .Include(e => e.Categories)
        //          .ThenInclude(c => c.Candidates)
        //          .ToListAsync();
        //}

        public async Task<int> GetVoteCountAsync(Guid candidateId, Guid electionCategoryId)
        {
            return await _context.Votes
                .CountAsync(v => v.CandidateId == candidateId && v.ElectionCategoryId == electionCategoryId);
        }

        public async Task<Candidate?> GetWithVotesAsync(Guid candidateId)
        {
            return await _dbSet
                .Include(c => c.Votes)
                .FirstOrDefaultAsync(c => c.Id == candidateId);
        }
    }
}