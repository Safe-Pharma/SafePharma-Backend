using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SafePharma.BLL.DTOs;
using SafePharma.Common;
using SafePharma.DAL;
using System.Security.Claims;
using System.Text;

namespace SafePharma.BLL.Managers.AuthenticationManager
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IValidator<ChangePasswordDTO> _changePasswordValidator;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthManager(
            UserManager<ApplicationUser> userManager,
            IValidator<LoginDTO> loginValidator,
            IValidator<ChangePasswordDTO> changePasswordValidator,
            ITokenGenerator tokenGenerator
        )
        {
            _userManager = userManager;
            _loginValidator = loginValidator;
            _changePasswordValidator = changePasswordValidator;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<GeneralResult<TokenDto>> LoginAsync(LoginDTO dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );

                return GeneralResult<TokenDto>.FailResult(errors, "Validation failed");
            }

            var user = await _userManager.Users
                     .Include(u => u.Pharmacy)
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return GeneralResult<TokenDto>.FailResult("Invalid email or password.");

            if (!user.IsActive || user.IsDeleted)
                return GeneralResult<TokenDto>.FailResult("User is not allowed.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
                return GeneralResult<TokenDto>.FailResult("Invalid email or password.");

            var claims = await GenerateClaims(user);

            var token = _tokenGenerator.GenerateToken(claims);

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return GeneralResult<TokenDto>.SuccessResult(token, "Login successful");
        }

        public async Task<GeneralResult> ChangePasswordAsync(string userId, ChangePasswordDTO dto)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                   .GroupBy(e => e.PropertyName)
                   .ToDictionary(
                       g => g.Key,
                       g => g.Select(e => new Error
                       {
                           ErrorCode = e.ErrorCode,
                           ErrorMessage = e.ErrorMessage
                       }).ToList()
                   );

                return GeneralResult<TokenDto>.FailResult(errors, "Validation failed");
            }

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
                new Claim("PharmacyId", user.PharmacyId.ToString()),
                new Claim("PharmacyName", user.Pharmacy?.Name ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<GeneralResult> SetPasswordAsync(SetPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return GeneralResult.FailResult("User not found");

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(dto.Token));

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.Code,
                            ErrorMessage = e.Description
                        }).ToList());

                return GeneralResult.FailResult(
                    errors,
                    "Failed to set password");
            }

            return GeneralResult.SuccessResult(
                "Password created successfully");
        }
    }
}