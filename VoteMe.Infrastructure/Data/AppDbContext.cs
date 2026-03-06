using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoteMe.Domain.Entities;

namespace VoteMe.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationMember> OrganizationMembers { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Organization>()
                .HasQueryFilter(o => !o.IsDeleted);

            builder.Entity<Election>()
                .HasQueryFilter(e => !e.IsDeleted);

            builder.Entity<Candidate>()
                .HasQueryFilter(c => !c.IsDeleted);

            builder.Entity<Vote>()
                .HasQueryFilter(v => !v.IsDeleted);

            builder.Entity<OrganizationMember>()
                .HasQueryFilter(om => !om.IsDeleted);

            builder.Entity<AuditLog>()
                .HasQueryFilter(a => !a.IsDeleted);

            builder.Entity<AppUser>()
                .HasQueryFilter(u => !u.IsDeleted);

            // Unique key
            builder.Entity<Organization>()
                .HasIndex(o => o.UniqueKey)
                .IsUnique();

            builder.Entity<Organization>()
                .HasIndex(o => o.Email)
                .IsUnique();

            builder.Entity<Election>()
                .HasIndex(e => e.OrganizationId);

            builder.Entity<Election>()
                .HasIndex(e => e.Status);

            // Checking if a user already voted in an election (very frequent)
            builder.Entity<Vote>()
                .HasIndex(v => new { v.VoterId, v.ElectionId })
                .IsUnique(); // prevents double voting at database level

            builder.Entity<OrganizationMember>()
                .HasIndex(om => om.OrganizationId);

            builder.Entity<Candidate>()
                .HasIndex(c => c.ElectionId);

            // Cascade restrictions
            builder.Entity<Vote>()
                .HasOne(v => v.Candidate)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vote>()
                .HasOne(v => v.Election)
                .WithMany(e => e.Votes)
                .HasForeignKey(v => v.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vote>()
                .HasOne(v => v.Voter)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.VoterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}