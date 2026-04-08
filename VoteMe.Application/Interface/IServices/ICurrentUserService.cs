namespace VoteMe.Application.Interface.IServices
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Email { get; }
        //string GlobalDisplayName { get; }
        string FirstName { get; }
        string LastName { get; }
        int TokenVersion { get; }
        //IEnumerable<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}