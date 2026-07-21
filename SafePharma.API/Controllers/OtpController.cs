using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.BLL.DTOs;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpManager _otpManager;

        public OtpController(IOtpManager otpManager)
        {
            _otpManager = otpManager;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestOtp(RequestOtpDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _otpManager.RequestOtpAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _otpManager.VerifyOtpAsync(dto);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}