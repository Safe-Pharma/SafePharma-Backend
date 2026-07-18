using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganImpairmentLevelController : ControllerBase
    {
        private readonly IOrganImpairmentLevelManager _manager;

        public OrganImpairmentLevelController(IOrganImpairmentLevelManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _manager.GetAll();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrganImpairmentLevelDto dto)
        {
            var result = await _manager.Create(dto);

            return Ok(result);
        }
    }
}
