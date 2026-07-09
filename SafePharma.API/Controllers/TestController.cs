using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> Send()
        {
            await _emailService.SendEmailAsync(
                "mostafa.mamdouh1002@gmail.com",
                "Brevo Test",
                "<h1>Hello from .NET 10 🚀</h1>");

            return Ok("Email sent");
        }
    }
}
