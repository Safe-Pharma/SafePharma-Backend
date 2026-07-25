using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;

namespace SafePharma.API.Controllers
{
    // Anonymous — the pharmacy owner has no JWT yet at this stage.
    // Secured only by knowledge of the SubscriptionId GUID.
    [Route("api/subscriptions/{subscriptionId:guid}/payment")]
    [ApiController]
    [AllowAnonymous]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentManager _manager;
        private readonly IValidator<SubmitPaymentProofDto> _validator;

        public PaymentController(IPaymentManager manager, IValidator<SubmitPaymentProofDto> validator)
        {
            _manager = manager;
            _validator = validator;
        }

        [HttpGet("instructions")]
        public async Task<IActionResult> GetInstructions(Guid subscriptionId)
        {
            var result = await _manager.GetPaymentInstructions(subscriptionId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("proof")]
        public async Task<IActionResult> SubmitProof(Guid subscriptionId, [FromForm] SubmitPaymentProofDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _manager.SubmitPaymentProof(subscriptionId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus(Guid subscriptionId)
        {
            var result = await _manager.GetLatestVerificationStatus(subscriptionId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(Guid subscriptionId)
        {
            var result = await _manager.GetVerificationHistory(subscriptionId);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }

    [Route("api/admin/payment-verifications")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.OwnerOnly)]
    public class AdminPaymentVerificationController : ControllerBase
    {
        private readonly IPaymentManager _manager;
        private readonly ICurrentUserContext _currentUser;
        private readonly IValidator<RejectPaymentDto> _rejectValidator;

        public AdminPaymentVerificationController(
            IPaymentManager manager,
            ICurrentUserContext currentUser,
            IValidator<RejectPaymentDto> rejectValidator)
        {
            _manager = manager;
            _currentUser = currentUser;
            _rejectValidator = rejectValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _manager.GetAllVerifications());

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
            => Ok(await _manager.GetPendingVerifications());

        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _manager.ApprovePayment(id, _currentUser.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectPaymentDto dto)
        {
            var validationResult = await _rejectValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _manager.RejectPayment(id, _currentUser.Id, dto.RejectionReason);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}


