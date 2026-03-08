using Microsoft.EntityFrameworkCore.Storage;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Infrastructure.Data;

namespace VoteMe.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IOrganizationRepository Organizations { get; }
        public IOrganizationMemberRepository OrganizationMembers { get; }
        public IElectionRepository Elections { get; }
        public ICandidateRepository Candidates { get; }
        public IVoteRepository Votes { get; }
        public IAuditLogRepository AuditLogs { get; }

        public UnitOfWork(
            AppDbContext context,
            IOrganizationRepository organizations,
            IOrganizationMemberRepository organizationMembers,
            IElectionRepository elections,
            ICandidateRepository candidates,
            IVoteRepository votes,
            IAuditLogRepository auditLogs)
        {
            _context = context;
            Organizations = organizations;
            OrganizationMembers = organizationMembers;
            Elections = elections;
            Candidates = candidates;
            Votes = votes;
            AuditLogs = auditLogs;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}