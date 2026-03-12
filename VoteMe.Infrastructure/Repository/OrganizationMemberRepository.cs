using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using VoteMe.Infrastructure.Repositories;
using VoteMe.Application.Interface.IRepositories;
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

        public async Task<IEnumerable<OrganizationMember>> GetOrganizationMembersAsync(Guid organizationId,int page = 1,int pageSize = 20)
        {
            return await _dbSet
                .Include(om => om.User)
                .Where(om => om.OrganizationId == organizationId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

        public async Task<IEnumerable<string>> GetOrganizationMemberEmailsAsync(Guid organizationId)
        {
            return await _dbSet
                .Include(m => m.User)
                .Where(m => m.OrganizationId == organizationId)
                .Select(m => m.User.Email!)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrganizationMember>> GetUserMembershipsAsync(Guid userId)
        {
            return await _dbSet
                .Include(m => m.Organization)
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }

        public async Task JoinOrganizationAsync(Guid userId, Guid organizationId)
        {
            var member = new OrganizationMember
            {
                UserId = userId,
                OrganizationId = organizationId,
                IsAdmin = false,
                JoinedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(member);
        }
    }
}
