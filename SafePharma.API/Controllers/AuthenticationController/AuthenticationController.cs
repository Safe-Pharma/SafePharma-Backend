using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL.DTOs;
using SafePharma.BLL.Managers;

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
    }
}
