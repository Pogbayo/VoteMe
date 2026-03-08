using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VoteMe.Application.Interface.Services;
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


        public async Task<string> GenerateAccessToken(AppUser user)
        {
            if (user == null)
            {
                _logger.LogWarning("User object is null");
                throw new ArgumentNullException(nameof(user));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("tokenVersion", user.Tokenversion.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expiryMinutes = _jwtSettings.ExpiryMinutes * 60;

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
