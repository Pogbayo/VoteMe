using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.Jwt;

namespace VoteMe.Infrastructure.Services
{
    public class TokenService : ITokenService
    {

        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<TokenService> _logger;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IOptions<JwtSettings> jwtOptions, UserManager<AppUser> userManager, ILogger<TokenService> logger)
        {
            _jwtSettings = jwtOptions.Value;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                //new Claim("globalDisplayName", user.GlobalDisplayName ?? $"{user.FirstName} {user.LastName}"),
                new Claim("tokenVersion", user.TokenVersion.ToString()),
                new Claim("isSuperAdmin", user.IsSuperAdmin.ToString().ToLower())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
