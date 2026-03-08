using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interfaces.Repositories;
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

        public async Task<Election?> GetFullElectionAsync(Guid electionId)
        {
            return await _dbSet
                .Include(e => e.Candidates)
                    .ThenInclude(c => c.Votes)
                .Include(e => e.Organization)
                .FirstOrDefaultAsync(e => e.Id == electionId);
        }

        public async Task<(IEnumerable<Election> Items, int TotalCount)> GetOrganizationElectionsAsync(
            Guid organizationId,
            int page = 1,
            int pageSize = 20)
        {
            return await GetPagedAsync(
                predicate: e => e.OrganizationId == organizationId,
                page: page,
                pageSize: pageSize
            );
        }

        public async Task<Election?> GetWithCandidatesAsync(Guid electionId)
        {
            return await _dbSet
                .Include(e => e.Candidates)
                .FirstOrDefaultAsync(e => e.Id == electionId);
        }

        public async Task<Election?> GetWithVotesAsync(Guid electionId)
        {
            return await _dbSet
                .Include(e => e.Votes)
                .FirstOrDefaultAsync(e => e.Id == electionId);
        }
    }
}