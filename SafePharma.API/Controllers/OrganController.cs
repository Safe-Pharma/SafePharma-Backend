using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganController : ControllerBase
    {
        private readonly IOrganManager _organManager;

        public OrganController(IOrganManager organManager)
        {
            _organManager = organManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _organManager.GetAll();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrganDto dto)
        {
            var result = await _organManager.Create(dto);

            return Ok(result);
        }
    }
}
