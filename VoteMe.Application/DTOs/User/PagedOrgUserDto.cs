namespace VoteMe.Application.DTOs.User
{
    public class PagedOrgUserDto
    {
        public List<OrganizationUserDto> Users { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
