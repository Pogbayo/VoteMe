using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IToken
{
    public interface ITokenGenerator
    {
        Task<string> GenerateToken(AppUser user);
    }
}
