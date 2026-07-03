
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IUserLanguageManager
    {
        Task<GeneralResult<string?>> GetLanguageAsync(string userId);
        Task<GeneralResult> UpdateLanguageAsync(string userId, UpdateLanguageDto dto);
    }
}