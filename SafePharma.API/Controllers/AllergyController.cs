using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergyController : ControllerBase
    {
        private readonly IAllergyManager _manager;

        public AllergyController(IAllergyManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _manager.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAllergyDto dto)
        {
            return Ok(await _manager.Create(dto));
        }
    }
}
