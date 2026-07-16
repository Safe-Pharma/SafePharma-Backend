using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using System.Security.Claims;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly ISaleManager _manager;

        public SaleController(ISaleManager manager)
        {
            _manager = manager;
        }

        [HttpPost("{saleId}/items")]
        public async Task<IActionResult> AddItem(Guid saleId, CreateSaleItemsDto dto)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _manager.AddItemToSale(saleId, dto, pharmacyId , userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{saleId}/items/{itemId}")]
        public async Task<IActionResult> UpdateItem(Guid saleId, Guid itemId, UpdateSaleItemDto dto)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _manager.UpdateSaleItem(saleId, itemId, dto, pharmacyId, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{saleId}/items/{itemId}")]
        public async Task<IActionResult> RemoveItem(Guid saleId, Guid itemId)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }
            var result = await _manager.RemoveSaleItem(saleId, itemId, pharmacyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDraftSale(CreateDraftSaleDto dto)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _manager.CreateDraftSale(dto, pharmacyId, userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        
    }
}