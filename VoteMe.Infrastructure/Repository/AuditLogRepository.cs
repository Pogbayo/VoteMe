using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLog>> GetUserLogsAsync(
             Guid userId,
             int page = 1,
             int pageSize = 20)
        {
            if (page < 1) page = 1;
            return await _dbSet
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetOrganizationLogsAsync(
             Guid organizationId,
             int page = 1,
             int pageSize = 20)
        {
            if (page < 1) page = 1;
            return await _dbSet
                .Where(a => a.OrganizationId == organizationId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GeAllLogsAsync(
             int page = 1,
             int pageSize = 20)
        {
            if (page < 1) page = 1;
            return await _dbSet
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task LogAsync(Guid userId, AuditAction action, string details)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            await _dbSet.AddAsync(log);
        }
    }
}
