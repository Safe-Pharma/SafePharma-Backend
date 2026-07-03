using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionManager _manager;
        private readonly IValidator<CreateSubscriptionDto> _validator;

        public SubscriptionController(ISubscriptionManager manager, IValidator<CreateSubscriptionDto> validator)
        {
            _manager = manager;
            _validator = validator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _manager.CreateSubscription(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}