using Microsoft.AspNetCore.Identity;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class UserLanguageManager : IUserLanguageManager
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLanguageManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<GeneralResult> UpdateLanguageAsync(string userId, UpdateLanguageDto dto)
        {
            if (userId is null) return GeneralResult.FailResult("User not found.");
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return GeneralResult.NotFound();

            user.PreferredLanguage = dto.Language;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return GeneralResult.FailResult("Failed to update language.");
            return GeneralResult.SuccessResult("Language updated successfully.");
        }
        public async Task<GeneralResult<string?>> GetLanguageAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return GeneralResult<string?>.NotFound();
            return GeneralResult<string?>.SuccessResult(user.PreferredLanguage ?? "en");
        }

    }
}
