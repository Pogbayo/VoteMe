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
        public async Task<IEnumerable<ElectionCategory>> GetElectionCategoriesAsync(Guid electionId)
        {
            return await _dbSet.Where(ec => ec.ElectionId == electionId)
                .Select(ec => new ElectionCategory
                {
                    Id = ec.Id,
                    Name = ec.Name,
                    Description = ec.Description,
                    ElectionId = ec.ElectionId
                })
                .ToListAsync();
        }

        public Task<ElectionCategory?> GetWithCandidatesAsync(Guid categoryId)
        {
            return _dbSet
                .Include(ec => ec.Candidates)
                .FirstOrDefaultAsync(ec => ec.Id == categoryId);
        }

        public Task<ElectionCategory?> GetWithVotesAsync(Guid categoryId)
        {
            return _dbSet
                .Include(ec => ec.Votes)
                .FirstOrDefaultAsync(ec => ec.Id == categoryId);
        }
    }
}
