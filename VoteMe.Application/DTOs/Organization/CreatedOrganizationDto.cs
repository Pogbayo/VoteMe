namespace VoteMe.Application.DTOs.Organization
{
    public class CreatedOrganizationDto
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string UniqueKey { get; set; } = string.Empty;
        public string AdminFirstName { get; set; } = string.Empty;
        public string AdminLastName { get; set; } = string.Empty;
        public string AdminDisplayName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
