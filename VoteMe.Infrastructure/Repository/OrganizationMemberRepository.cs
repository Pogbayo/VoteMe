using Microsoft.EntityFrameworkCore;
using VoteMe.Application.DTOs.OrganizationMember;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;
namespace VoteMe.Infrastructure.Repository
{
    public class OrganizationMemberRepository : GenericRepository<OrganizationMember>, IOrganizationMemberRepository
    {
        public OrganizationMemberRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrganizationMember>> GetUserMembershipsWithOrgsAsync(Guid userId)
        {
            return await _dbSet
                .Include(m => m.Organization)
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }

        public async Task<OrganizationRole?> GetUserRoleAsync(Guid userId, Guid organizationId)
        {
            var membership = await _context.OrganizationMembers
                .FirstOrDefaultAsync(m => m.UserId == userId && m.OrganizationId == organizationId);

            return membership?.Role; 
        }
        public async Task<OrganizationMember?> GetMemberAsync(Guid organizationId, Guid userId)
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
        public async Task<int> GetApprovedMembersCount(Guid organizationId,Guid userId)
        {
            return await _dbSet
                .Where(om => om.OrganizationId == organizationId && om.UserId == userId && om.Status == MembershipStatus.Approved)
                .CountAsync();
        }
        public async Task<int> GetPendingMembersCount(Guid organizationId,Guid userId)
        {
            return await _dbSet
                .Where(om => om.OrganizationId == organizationId && om.UserId == userId && om.Status == MembershipStatus.Pending)
                .CountAsync();
        }

        public async Task<IEnumerable<OrganizationMember>> GetMembersByStatusAsync(
            Guid organizationId, MembershipStatus status)
        {
            return await _dbSet
                .Include(m => m.User)
                .Where(m => m.OrganizationId == organizationId
                         && m.Status == status)
                .ToListAsync();
        }
        public async Task<IEnumerable<OrganizationMember>> GetUserOrganizationsAsync(Guid userId)
        {
            return await _dbSet
                .Include(om => om.Organization)
                .Where(om => om.UserId == userId)
                .ToListAsync();
        }

        //public async Task<bool> IsAdminAsync(Guid userId, Guid organizationId)
        //{
        //    return await _dbSet
        //        .AnyAsync(om => om.UserId == userId
        //            && om.OrganizationId == organizationId
        //            && om.Role == OrganizationRole.Admin);
        //}

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

        public async Task<bool> JoinOrganizationAsync(Guid userId, JoinOrgDto dto, Guid organizationId)
        {
            var member = new OrganizationMember
            {
                UserId = userId,
                OrganizationId = organizationId,
                DisplayName = dto.DisplayName,
                Role = OrganizationRole.Member,
                JoinedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(member);
            return true;
        }
    }
}
