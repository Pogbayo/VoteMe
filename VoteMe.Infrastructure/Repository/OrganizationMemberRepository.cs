using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class OrganizationMemberRepository : GenericRepository<OrganizationMember>, IOrganizationMemberRepository
    {
        public OrganizationMemberRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<OrganizationMember?> GetMemberAsync(Guid userId, Guid organizationId)
        {
            return await _dbSet
                .Include(om => om.User)
                .Include(om => om.Organization)
                .FirstOrDefaultAsync(om => om.UserId == userId && om.OrganizationId == organizationId);
        }

        public async Task<IEnumerable<OrganizationMember>> GetOrganizationMembersAsync(Guid organizationId)
        {
            return await _dbSet
                .Include(om => om.User)
                .Where(om => om.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrganizationMember>> GetUserOrganizationsAsync(Guid userId)
        {
            return await _dbSet
                .Include(om => om.Organization)
                .Where(om => om.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsAdminAsync(Guid userId, Guid organizationId)
        {
            return await _dbSet
                .AnyAsync(om => om.UserId == userId
                    && om.OrganizationId == organizationId
                    && om.IsAdmin == true);
        }

        public async Task<bool> IsMemberAsync(Guid userId, Guid organizationId)
        {
            return await _dbSet
                .AnyAsync(om => om.UserId == userId
                    && om.OrganizationId == organizationId);
        }
    }
}
