using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/subscription-plans")]
    [ApiController]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanManager _manager;
        private readonly IValidator<SubscriptionPlanUpsertDto> _validator;

        public SubscriptionPlanController(ISubscriptionPlanManager manager, IValidator<SubscriptionPlanUpsertDto> validator)
        {
            _manager = manager;
            _validator = validator;
        }

        [HttpGet]
        [AllowAnonymous] // used by the public subscribe page
        public async Task<IActionResult> GetActive() => Ok(await _manager.GetActivePlans());

        [HttpGet("admin")]
        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        public async Task<IActionResult> GetAll() => Ok(await _manager.GetAllPlans());

        [HttpPost]
        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        public async Task<IActionResult> Create([FromBody] SubscriptionPlanUpsertDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid) return BadRequest(validation.Errors);

            var result = await _manager.CreatePlan(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionPlanUpsertDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid) return BadRequest(validation.Errors);

            var result = await _manager.UpdatePlan(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _manager.DeletePlan(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}