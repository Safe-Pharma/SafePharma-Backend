using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

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
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] PharmacySettingsUpdateDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _manager.updatePharamcySettings(dto);
            return Ok(result);
        }
    }
}
