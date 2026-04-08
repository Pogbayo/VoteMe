using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class ElectionRepository : GenericRepository<Election>, IElectionRepository
    {
        public ElectionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Election>> GetActiveElectionsAsync()
        {
            return await _dbSet
                .Where(e => e.Status == ElectionStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Election>> GetElectionsByStatusAsync(ElectionStatus status)
        {
            return await _dbSet
                .Where(e => e.Status == status)
                .ToListAsync();
        }

        public async Task<Election?> GetWithCategoriesAsync(Guid electionId, int page = 1,
            int pageSize = 20)
        {
            return await _dbSet
                .Include(e => e.Categories!)
                    .ThenInclude(ec => ec.Candidates)
                .Include(e => e.Organization)
                .FirstOrDefaultAsync(e => e.Id == electionId);
        }

        public async Task<(IEnumerable<Election> Items, int TotalCount)> GetOrganizationElectionsAsync(
            Guid organizationId,
            int page = 1,
            int pageSize = 20)
        {
            var query = _dbSet.AsNoTracking()
                 .Where(e => e.OrganizationId == organizationId)
                 .Include(e => e.Categories)
                 .OrderByDescending(e => e.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Election?> GetFullElectionAsync(Guid electionId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(e => e.Categories!)
                    .ThenInclude(ec => ec.Candidates!)
                        .ThenInclude(c => c.Votes)
                .FirstOrDefaultAsync(e => e.Id == electionId);
        }

     
    }
}