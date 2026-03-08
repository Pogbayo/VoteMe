namespace VoteMe.Application.Events.Auth
{
    public class PasswordChangedEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
