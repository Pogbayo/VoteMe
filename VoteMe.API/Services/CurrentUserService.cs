using System.Security.Claims;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId =>
            Guid.Parse(_httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        public string Email =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public IEnumerable<string> Roles =>
            _httpContextAccessor.HttpContext?.User
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public string DisplayName =>
       _httpContextAccessor.HttpContext?.User
           .FindFirstValue("displayName") ?? string.Empty;

        public string FirstName =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

        public string LastName =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

        public int TokenVersion =>
            int.TryParse(
                _httpContextAccessor.HttpContext?.User.FindFirstValue("tokenVersion"),
                out var version) ? version : 0;
    }
}