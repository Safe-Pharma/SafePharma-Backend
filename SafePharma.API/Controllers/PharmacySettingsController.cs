using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;

namespace SafePharma.API.Controllers
{
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
            var result = await _manager.GetSettings();
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

            var result = await _manager.updatePharamcySettings(dto);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
