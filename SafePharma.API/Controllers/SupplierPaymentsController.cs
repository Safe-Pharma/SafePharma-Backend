using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using System.Security.Claims;

namespace SafePharma.API.Controllers
{
    [Route("api/supplier-payments")]
    [ApiController]
    [Authorize]
    public class SupplierPaymentsController : ControllerBase
    {
        private readonly ISupplierPaymentManager _manager;
        private readonly IValidator<RecordSupplierPaymentDto> _validator;

        public SupplierPaymentsController(
            ISupplierPaymentManager manager,
            IValidator<RecordSupplierPaymentDto> validator)
        {
            _manager = manager;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetHistory(pharmacyId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RecordPayment([FromBody] RecordSupplierPaymentDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var pharmacyId = User.GetPharmacyId();
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _manager.RecordPayment(pharmacyId, userId, dto);

            if (result.SupplierNotFound)
            {
                return NotFound(new { message = "Supplier not found." });
            }

            if (result.AmountExceedsBalance)
            {
                return BadRequest(new { message = "Amount exceeds the supplier's outstanding balance." });
            }

            return Ok(result.Payment);
        }

    }
}
