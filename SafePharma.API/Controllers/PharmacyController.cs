using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.BLL.Managers.PharmacyManager;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.OwnerOnly)]

    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyManager _pharmacyManager;

        public PharmacyController(IPharmacyManager pharmacyManager)
        {
            _pharmacyManager = pharmacyManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetPharmacies()
        {
            var res = await _pharmacyManager.GetAllPharmacies();
            if (!res.Success)
                return NotFound(res);

            return Ok(res);

        }

        [HttpPatch("{id:Guid}/status")]
        public async Task<ActionResult> UpdatePharmacyStatus(Guid id)
        {
            var res = await _pharmacyManager.UpdatePharmacyStatus(id);

            if (!res.Success)
            {
                return BadRequest(res);
            }
            return Ok(res);

        }
    }
}
