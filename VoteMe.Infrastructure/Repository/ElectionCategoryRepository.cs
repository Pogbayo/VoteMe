using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class ElectionCategoryRepository : GenericRepository<ElectionCategory>, IElectionCategoryRepository
    {
        public ElectionCategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ElectionCategory?> GetElectionCategoryAsync(Guid electionCategoryId)
        {
            return await _context.ElectionCategories
                .Include(ec => ec.Candidates)
                .FirstOrDefaultAsync(ec => ec.Id == electionCategoryId);
        }

        public async Task<IEnumerable<ElectionCategory>> GetElectionCategoriesAsync(Guid electionId)
        {
            return await _context.ElectionCategories
                .Include(ec => ec.Candidates)
                .Where(c => c.ElectionId == electionId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ElectionCategory?> GetElectionCategoryResultsAsync(Guid categoryId)
        {
            return await _context.ElectionCategories
                .Include(c => c.Candidates!)
                    .ThenInclude(cand => cand.Votes)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}
