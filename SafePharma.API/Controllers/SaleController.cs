using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.DAL;
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

        // GET /api/Sale?status=Completed&search=Ahmed
        [HttpGet]
        public async Task<IActionResult> GetAll(
           [FromQuery] SaleStatus? status,
           [FromQuery] string? search)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.GetAllSales(pharmacyId, status, search);
            return Ok(result);
        }

        // GET api/Sale/{saleId}
        [HttpGet("{saleId}")]
        public async Task<IActionResult> GetById(Guid saleId)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.GetSaleById(saleId, pharmacyId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
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

        // GET /api/Sale/availability/{pharmacyMedicineId} — read-only stock/price
        // preview for a locally-held cart line. Never touches Sales/SaleItems.
        [HttpGet("availability/{pharmacyMedicineId}")]
        public async Task<IActionResult> GetAvailability(Guid pharmacyMedicineId)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.GetAvailability(pharmacyMedicineId, pharmacyId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // POST /api/Sale/checkout — creates the Sale, adds every item, applies
        // the sale-level discount/tax, and records payment, all in one atomic
        // call. Nothing about the cart touches the database before this point.
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutDto dto)
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

            var result = await _manager.Checkout(dto, pharmacyId, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{saleId}/tax")]
        public async Task<IActionResult> ApplyTax(Guid saleId, ApplySaleTaxDto dto)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.ApplyTax(saleId, dto, pharmacyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{saleId}/discount")]
        public async Task<IActionResult> ApplyDiscount(Guid saleId, ApplySaleDiscountDto dto)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.ApplyDiscount(saleId, dto, pharmacyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{saleId}/pay")]
        public async Task<IActionResult> Pay(Guid saleId, PaySaleDto dto)
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

            var result = await _manager.Pay(saleId, dto, pharmacyId, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{saleId}/cancel")]
        public async Task<IActionResult> CancelSale(Guid saleId)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.CancelSale(saleId, pharmacyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Hard delete — only for an untouched Open draft (e.g. closing an empty
        // POS tab with the X button). Use POST {saleId}/cancel instead for a
        // sale you deliberately want to keep in the record as "Cancelled".
        [HttpDelete("{saleId}")]
        public async Task<IActionResult> DeleteDraft(Guid saleId)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.DeleteDraftSale(saleId, pharmacyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{saleId}/customer")]
        public async Task<IActionResult> SetCustomer(Guid saleId, SetSaleCustomerDto dto)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.SetCustomer(saleId, dto, pharmacyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        //GET /api/Sale/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.GetStats(pharmacyId);
            return Ok(result);
        }

        // GET /api/Sale/trend?days=7
        [HttpGet("trend")]
        public async Task<IActionResult> GetTrend([FromQuery] int days = 7)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.GetTrend(pharmacyId, days);
            return Ok(result);
        }

        // GET /api/Sale/category-mix
        [HttpGet("category-mix")]
        public async Task<IActionResult> GetCategoryMix()
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _manager.GetCategoryMix(pharmacyId);
            return Ok(result);
        }

    }
}