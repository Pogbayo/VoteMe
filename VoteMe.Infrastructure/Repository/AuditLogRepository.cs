using Microsoft.EntityFrameworkCore;
using VoteMe.Application.Interfaces.Repositories;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLog>> GetUserLogsAsync(Guid userId)
        {
            return await _dbSet
                 .Where(a => a.UserId == userId)
                 .OrderByDescending(a => a.Timestamp)
                 .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetEntityLogsAsync(string entity)
        {
           return await _dbSet
                .Where(a => a.Entity == entity)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task LogAsync(Guid userId, string action, string entity, string details)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Entity = entity,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            await _dbSet.AddAsync(log);
        }
    }
}
