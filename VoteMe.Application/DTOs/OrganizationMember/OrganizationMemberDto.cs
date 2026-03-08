namespace VoteMe.Application.DTOs.OrganizationMember
{
    public class OrganizationMemberDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
