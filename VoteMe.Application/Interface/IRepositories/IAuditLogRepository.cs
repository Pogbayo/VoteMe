using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetUserLogsAsync(Guid userId, int page, int pageSize);
        Task LogAsync(Guid userId, string action, string entity, string details);
    }
}