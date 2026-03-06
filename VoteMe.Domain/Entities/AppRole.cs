using Microsoft.AspNetCore.Identity;

namespace VoteMe.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
