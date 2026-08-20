using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AuditController : ControllerBase
    {
        private readonly IAuditManager _auditManager;

        public AuditController(IAuditManager auditManager)
        {
            _auditManager = auditManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAudit()
        {

            var res =await  _auditManager.GetAllAudit();
            if (!res.Success)
                return BadRequest(res);
            return Ok(res);
        }
        //[HttpPost]
        //public async Task<ActionResult> CreateAudit([FromBody] AuditCreateDto auditCreateDto)
        //{
        //    var res = await _auditManager.CreateAudit(auditCreateDto);
        //    return Ok(res);
        //}
    }
}
