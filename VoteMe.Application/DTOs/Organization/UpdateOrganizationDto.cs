namespace VoteMe.Application.DTOs.Organization
{
    public class UpdateOrganizationDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
    }
}
