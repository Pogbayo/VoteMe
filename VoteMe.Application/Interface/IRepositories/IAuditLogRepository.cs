using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetUserLogsAsync(Guid userId, int page, int pageSize);
        Task LogAsync(Guid userId, AuditAction action, string details);
        Task<IEnumerable<AuditLog>> GetOrganizationLogsAsync(
             Guid organizationId,
             int page = 1,
             int pageSize = 20);
        Task<IEnumerable<AuditLog>> GeAllLogsAsync(
             int page = 1,
             int pageSize = 20);
    }
}