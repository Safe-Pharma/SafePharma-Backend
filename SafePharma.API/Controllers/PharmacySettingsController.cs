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
    public class PharmacySettingsController : ControllerBase
    {
        private readonly IPharmacySettingManager _manager;
        private readonly IValidator<PharmacySettingsUpdateDto> _validator;

        public PharmacySettingsController(IPharmacySettingManager manager, IValidator<PharmacySettingsUpdateDto> validator)
        {
            _manager = manager;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var pharmacyIdClaim = User.FindFirstValue("PharmacyId");

            if (string.IsNullOrEmpty(pharmacyIdClaim) || !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
                return Unauthorized();

            var result = await _manager.GetSettings(pharmacyId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromForm] PharmacySettingsUpdateDto dto)
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

            if (string.IsNullOrEmpty(pharmacyIdClaim) || !Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
                return Unauthorized();

            var result = await _manager.updatePharamcySettings(dto, pharmacyId);

            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
