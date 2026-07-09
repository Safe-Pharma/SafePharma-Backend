using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.BLL.DTOs;
using SafePharma.BLL.Managers;
using System.Security.Claims;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        public AuthenticationController(IAuthManager authManager)
        {
            _authManager = authManager;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authManager.LoginAsync(dto);
            if (!result.Success)
                return Unauthorized(result);
            return Ok(result);

        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _authManager.ChangePasswordAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }



        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword(SetPasswordDTO dto)
        {
            var result = await _authManager.SetPasswordAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
