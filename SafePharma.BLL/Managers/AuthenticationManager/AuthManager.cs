using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SafePharma.BLL.DTOs;
using SafePharma.Common;
using SafePharma.DAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SafePharma.BLL.Managers.AuthenticationManager
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;

    public AuthManager(
        UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            if (jwtSettings == null || jwtSettings.Value == null)
                throw new InvalidOperationException("JWT settings are not configured. Ensure JwtSettings are bound from configuration.");

            _jwtSettings = jwtSettings.Value;
        }

        public async Task<GeneralResult<TokenDto>> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return GeneralResult<TokenDto>.FailResult("Invalid email or password.");

            if (!user.IsActive || user.IsDeleted)
                return GeneralResult<TokenDto>.FailResult("User is not allowed.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
                return GeneralResult<TokenDto>.FailResult("Invalid email or password.");

            var claims = await GenerateClaims(user);

            var token = GenerateJwtToken(claims);

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return GeneralResult<TokenDto>.SuccessResult(token, "Login successful");
        }
        public async Task<GeneralResult> ChangePasswordAsync(string userId, ChangePasswordDTO dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return GeneralResult.FailResult("User not found");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error { ErrorCode = e.Code, ErrorMessage = e.Description }).ToList()
                    );

                return GeneralResult.FailResult(errors, "Failed to change password");
            }

            return GeneralResult.SuccessResult("Password changed successfully");

}


        private async Task<List<Claim>> GenerateClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("FullName", user.FullName),
            new Claim("PharmacyId", user.PharmacyId.ToString())
        };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private TokenDto GenerateJwtToken(List<Claim> claims)
        {
            if (string.IsNullOrWhiteSpace(_jwtSettings.Key))
                throw new InvalidOperationException("JWT signing key is missing. Set 'JWT:Key' in configuration.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key)
            );

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

            return new TokenDto(
                tokenString,
                duration
            );
        }
    }

}
