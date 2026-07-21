using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SafePharma.BLL.DTOs;
using SafePharma.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SafePharma.BLL
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public TokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            if (jwtSettings == null || jwtSettings.Value == null)
                throw new InvalidOperationException("JWT settings are not configured.");

            _jwtSettings = jwtSettings.Value;
        }

        public TokenDto GenerateToken(List<Claim> claims)
        {
            if (string.IsNullOrWhiteSpace(_jwtSettings.Key))
                throw new InvalidOperationException("JWT signing key is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var duration = _jwtSettings.DurationInMinutes > 0 ? _jwtSettings.DurationInMinutes : 60;
            var expires = DateTime.UtcNow.AddMinutes(duration);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenDto(tokenString, duration);
        }
    }
}