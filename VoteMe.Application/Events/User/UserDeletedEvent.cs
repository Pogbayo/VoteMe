namespace VoteMe.Application.Events.User;

public class UserDeletedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid DeletedByUserId { get; set; }
    public DateTime DeletedAt { get; set; }
}