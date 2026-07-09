using SafePharma.BLL.DTOs;
using SafePharma.Common;

namespace SafePharma.BLL.Managers
{
    public interface IAuthManager
    {
        Task<GeneralResult<TokenDto>> LoginAsync(LoginDTO dto);
        Task<GeneralResult> ChangePasswordAsync(string userId, ChangePasswordDTO dto);
        Task<GeneralResult> SetPasswordAsync(SetPasswordDTO dto);

    }
}
