
namespace VoteMe.Application.DTOs.ElectionCategory
{
    public class UpdateElectionCategoryDto
    {
        public Guid ElectionCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

}
