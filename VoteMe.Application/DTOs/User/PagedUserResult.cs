
namespace VoteMe.Application.DTOs.User
{
    public class PagedUserResult
    {
        public List<UserDto> Users { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
