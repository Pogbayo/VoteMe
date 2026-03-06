
using VoteMe.Application.Interfaces.Repositories;

namespace VoteMe.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IOrganizationRepository Organizations { get; }
        public IOrganizationMemberRepository OrganizationMembers { get; }
        public IElectionRepository Elections { get; }
        public ICandidateRepository Candidates { get; }
        public IVoteRepository Votes { get; }
        public IAuditLogRepository AuditLogs { get; }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}