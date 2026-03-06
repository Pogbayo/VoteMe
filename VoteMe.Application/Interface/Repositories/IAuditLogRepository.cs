using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetUserLogsAsync(Guid userId);
        Task<IEnumerable<AuditLog>> GetEntityLogsAsync(string entity);
        Task LogAsync(Guid userId, string action, string entity, string details);
    }
}