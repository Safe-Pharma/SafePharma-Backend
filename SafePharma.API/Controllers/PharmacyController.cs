using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL.Managers.PharmacyManager;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok(res);

        }
    }
}
