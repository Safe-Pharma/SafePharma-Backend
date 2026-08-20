using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.BLL.DTOs.Audit;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

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

            var pharmacyId = User.GetPharmacyId();

            var res =await  _auditManager.GetAllAudit( );
            return Ok(res);
        }
        // GET api/Audit/recent?take=6 — short feed for the dashboard "Recent activity" widget.
        [HttpGet("recent")]
        public async Task<ActionResult> GetRecent([FromQuery] int take = 6)
        {
            var res = await _auditManager.GetRecentActivity(take);
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
