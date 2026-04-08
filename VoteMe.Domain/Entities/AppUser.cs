using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VoteMe.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public required string FirstName { get; set; } 
        public required string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public int TokenVersion { get; set; } = 1;
        public ICollection<OrganizationMember> OrganizationMembers { get; set; } = new List<OrganizationMember>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public bool IsSuperAdmin { get; set; } = false;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void UpdateTimestamps()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
