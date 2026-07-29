using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;
using System.Security.Claims;

namespace SafePharma.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReceiptController : ControllerBase
    {
        private readonly IPurchaseReceiptManager _purchaseReceiptManager;
        private readonly IValidator<CreatePurchaseReceiptDto> _validator;


        public PurchaseReceiptController(IPurchaseReceiptManager purchaseReceiptManager, IValidator<CreatePurchaseReceiptDto> validator)
        {
            _purchaseReceiptManager = purchaseReceiptManager;
            _validator = validator;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _purchaseReceiptManager.GetAllPurchaseReceipts(pharmacyId);

            return Ok(result);
        }


        [HttpPost("{purchaseOrderId:guid}")]
        public async Task<IActionResult> Receive(Guid purchaseOrderId, [FromBody] CreatePurchaseReceiptDto dto)
        {

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );
                return BadRequest(GeneralResult.FailResult(errors));
            }

            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var result = await _purchaseReceiptManager.CreatePurchaseReceipt(dto, userId, purchaseOrderId, pharmacyId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("item/{id:guid}")]
        public async Task<IActionResult> UpdateReceiptItem(Guid id, [FromBody] UpdatePurchaseReceiptItemDto dto)
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");
            if (string.IsNullOrEmpty(pharmacyIdClaim) ||
                !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized();
            }

            var result = await _purchaseReceiptManager.UpdateReceiptItem(id, dto, pharmacyId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}