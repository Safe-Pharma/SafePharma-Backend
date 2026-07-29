using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CustomerRelativeController : ControllerBase
    {
        private ICustomerRelativesManager _customerRelativesManager;

        public CustomerRelativeController(ICustomerRelativesManager customerRelativesManager)
        {
            _customerRelativesManager = customerRelativesManager;
        }

        [HttpPost]
        public async Task<ActionResult> CreateRelation([FromBody] CustomerRelativeCreateDto dto)

        {
            var res = await _customerRelativesManager.CreateRelation(dto);
            return Ok(res);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult> GetCustomerRelations([FromRoute] Guid id )

        {
            var res = await _customerRelativesManager.GetRelations(id);
            return Ok(res);
        }
        [HttpGet("getChilds/{id:Guid}")]
        public async Task<ActionResult> GetCustomerChilds([FromRoute] Guid id)

        {
            var res = await _customerRelativesManager.GetChilds(id);
            return Ok(res);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> RemoveRelation([FromRoute] Guid id)
        {
            var res = await _customerRelativesManager.RemoveRelation(id);
            return Ok(res);
        }
    }
}
