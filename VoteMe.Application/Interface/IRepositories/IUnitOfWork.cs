namespace VoteMe.Application.Interface.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IOrganizationRepository Organizations { get; }
        IOrganizationMemberRepository OrganizationMembers { get; }
        IElectionCategoryRepository ElectionCategories { get; }
        IElectionRepository Elections { get; }
        ICandidateRepository Candidates { get; }
        IVoteRepository Votes { get; }
        IAuditLogRepository AuditLogs { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}