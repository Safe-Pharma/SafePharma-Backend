using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;
using System.Security.Claims;

namespace SafePharma.API.Controllers
{
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

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var result = await _purchaseReceiptManager.CreatePurchaseReceipt(dto,userId,purchaseOrderId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
