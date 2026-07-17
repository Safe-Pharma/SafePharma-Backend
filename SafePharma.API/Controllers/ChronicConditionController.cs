using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChronicConditionController : ControllerBase
    {
        private readonly IChronicConditionManager _manager;

        public ChronicConditionController(IChronicConditionManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _manager.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateChronicConditionDto dto)
        {
            return Ok(await _manager.Create(dto));
        }
    }
}
