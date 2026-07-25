using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionManager _manager;
        private readonly IValidator<CreateSubscriptionDto> _validator;
        private readonly IValidator<UploadLogoDto> _logoValidator;

        public SubscriptionController(
            ISubscriptionManager manager,
            IValidator<CreateSubscriptionDto> validator,
            IValidator<UploadLogoDto> logoValidator)
        {
            _manager = manager;
            _validator = validator;
            _logoValidator = logoValidator;
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

        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
    => Ok(await _manager.GetAllSubscriptions());

        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _manager.GetSubscriptionById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubscriptionDto dto, [FromServices] IValidator<UpdateSubscriptionDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var result = await _manager.UpdateSubscription(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromServices] ICurrentUserContext currentUser)
        {
            var result = await _manager.CancelSubscription(id, currentUser.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("logo")]
        public async Task<IActionResult> UploadLogo([FromForm] UploadLogoDto dto, [FromServices] ICloudinaryService cloudinaryService)
        {
            var validationResult = await _logoValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var url = await cloudinaryService.UploadImageAsync(dto.Logo);
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest(GeneralResult<string>.FailResult("Logo upload failed. Please try again."));

            return Ok(GeneralResult<string>.SuccessResult(url, "Logo uploaded."));
        }
    }
}