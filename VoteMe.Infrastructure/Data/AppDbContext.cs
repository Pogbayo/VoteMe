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
        public DbSet<ElectionCategory> ElectionCategories { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Global query filters
            builder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted);
            builder.Entity<Election>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<ElectionCategory>().HasQueryFilter(ec => !ec.IsDeleted);
            builder.Entity<Candidate>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Vote>().HasQueryFilter(v => !v.IsDeleted);
            builder.Entity<OrganizationMember>().HasQueryFilter(om => !om.IsDeleted);
            builder.Entity<AuditLog>().HasQueryFilter(a => !a.IsDeleted);
            builder.Entity<AppUser>().HasQueryFilter(u => !u.IsDeleted);

            // Organization indexes
            builder.Entity<Organization>()
                .HasIndex(o => o.UniqueKey).IsUnique();
            builder.Entity<Organization>()
                .HasIndex(o => o.Email).IsUnique();

            // Election indexes
            builder.Entity<Election>()
                .HasIndex(e => e.OrganizationId);
            builder.Entity<Election>()
                .HasIndex(e => e.Status);

            // ElectionCategory relationships and indexes
            builder.Entity<ElectionCategory>()
                .HasOne(ec => ec.Election)
                .WithMany(e => e.Categories)
                .HasForeignKey(ec => ec.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ElectionCategory>()
                .HasIndex(ec => ec.ElectionId);

            // Candidate relationships and indexes
            builder.Entity<Candidate>()
                .HasOne(c => c.ElectionCategory)
                .WithMany(ec => ec.Candidates)
                .HasForeignKey(c => c.ElectionCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Candidate>()
                .HasIndex(c => c.ElectionCategoryId);

            // Vote - prevent double voting per category
            builder.Entity<Vote>()
                .HasIndex(v => new { v.VoterId, v.ElectionCategoryId })
                .IsUnique();

            // Vote relationships
            builder.Entity<Vote>()
                .HasOne(v => v.Candidate)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Vote>()
            //    .HasOne(v => v.ElectionCategory)
            //    .WithMany(ec => ec.Votes)
            //    .HasForeignKey(v => v.ElectionCategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vote>()
                .HasOne(v => v.Voter)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrganizationMember indexes
            builder.Entity<OrganizationMember>()
                .HasIndex(om => om.OrganizationId);
        }
    }
}
