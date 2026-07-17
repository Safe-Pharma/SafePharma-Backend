using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using System.Security.Claims;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MedicineSearchController : ControllerBase
    {
        private readonly IMedicineSearchService _searchService;

        public MedicineSearchController(IMedicineSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] MedicineSearchRequestDto request)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");

            if (!Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
                return Unauthorized();

            var result = await _searchService.SearchAsync(pharmacyId, request);

            return Ok(result);
        }
    }
}
