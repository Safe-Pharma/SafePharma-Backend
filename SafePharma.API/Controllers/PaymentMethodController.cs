using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/admin/payment-methods")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.OwnerOnly)]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodManager _manager;
        private readonly IValidator<PaymentMethodUpsertDto> _validator;

        public PaymentMethodController(IPaymentMethodManager manager, IValidator<PaymentMethodUpsertDto> validator)
        {
            _manager = manager;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _manager.GetAllMethods());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentMethodUpsertDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid) return BadRequest(validation.Errors);

            var result = await _manager.CreateMethod(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PaymentMethodUpsertDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid) return BadRequest(validation.Errors);

            var result = await _manager.UpdateMethod(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _manager.DeleteMethod(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}