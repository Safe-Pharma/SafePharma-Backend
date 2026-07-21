using SafePharma.BLL.DTOs;
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IOtpManager
    {
        Task<GeneralResult<string>> RequestOtpAsync(RequestOtpDto dto);
        Task<GeneralResult<TokenDto>> VerifyOtpAsync(VerifyOtpDto dto);
    }
}