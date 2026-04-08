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

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public string Email =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public bool IsSuperAdmin =>
                bool.TryParse(_httpContextAccessor.HttpContext?.User
                    .FindFirstValue("isSuperAdmin"), out var isSuper) && isSuper;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

       // public string GlobalDisplayName =>
       //_httpContextAccessor.HttpContext?.User
       //    .FindFirstValue("globalDisplayName") ?? string.Empty;

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