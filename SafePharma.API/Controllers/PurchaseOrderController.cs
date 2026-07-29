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
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderManager _manager;
        private readonly IValidator<PurchaseOrderCreateDto> _validator;

        public PurchaseOrderController(IPurchaseOrderManager manager, IValidator<PurchaseOrderCreateDto> validator)
        {
            _manager = manager;
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

            var result = await _manager.GetAllAsync(pharmacyId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderCreateDto dto)
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

            var result = await _manager.CreateAsync(dto, pharmacyId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
