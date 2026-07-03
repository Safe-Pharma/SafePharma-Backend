using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private IAuditManager _auditManager;

        public AuditController(IAuditManager auditManager)
        {
            _auditManager = auditManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAudit()
        {
            var res =await  _auditManager.GetAllAudit();
            return Ok(res);
        }
    }
}
