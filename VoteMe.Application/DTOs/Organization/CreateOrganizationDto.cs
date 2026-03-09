namespace VoteMe.Application.DTOs.Organization
{
    public class CreateOrganizationDto
    {
        public string OrganizationName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string AdminFirstName { get; set; } = string.Empty;
        public string AdminLastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
