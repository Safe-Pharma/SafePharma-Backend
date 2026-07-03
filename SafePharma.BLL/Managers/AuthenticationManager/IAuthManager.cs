using SafePharma.BLL.DTOs;
using SafePharma.Common;

namespace SafePharma.BLL.Managers
{
    public interface IAuthManager
    {
        Task<GeneralResult<TokenDto>> LoginAsync(LoginDTO dto);
    }
}
