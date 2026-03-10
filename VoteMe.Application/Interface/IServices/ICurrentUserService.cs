namespace VoteMe.Application.Interface.IServices
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Email { get; }
        IEnumerable<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}