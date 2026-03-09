using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(AppUser user);
    }
}
