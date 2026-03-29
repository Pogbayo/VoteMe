namespace VoteMe.Infrastructure.Jwt
{
    public class JwtSettings
    {
        public required string Key { get; set; } 
        public required int ExpiryMinutes { get; set; }
        public required string Issuer { get; set; } 
        public required string Audience { get; set; }
    }
}
