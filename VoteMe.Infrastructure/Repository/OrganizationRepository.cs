using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class OrganizationRepository : GenericRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Organization?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(o => o.Email == email);
        }

        public async Task<Organization?> GetByUniqueKeyAsync(string uniqueKey)
        {
            return await _dbSet
                .FirstOrDefaultAsync(o => o.UniqueKey == uniqueKey);
        }

        public Task<Organization?> GetFullOrganization(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task<Organization?> GetWithElectionsAsync(Guid organizationId)
        {
            return await _dbSet
                .Include(o => o.Elections)
                .FirstOrDefaultAsync(o => o.Id == organizationId);
        }

        public async Task<Organization?> GetWithMembersAsync(Guid organizationId)
        {
            return await _dbSet
                .Include(o => o.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(o => o.Id == organizationId);
        }

        public async Task<bool> UniqueKeyExistsAsync(string uniqueKey)
        {
            return await _dbSet
                .AnyAsync(o => o.UniqueKey == uniqueKey);
        }
    }
}