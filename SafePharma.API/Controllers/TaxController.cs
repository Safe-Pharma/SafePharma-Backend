using FluentValidation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaxesController : ControllerBase
    {
        private readonly ITaxManager _manager;
        private readonly IValidator<TaxCreateDto> _createValidator;
        private readonly IValidator<TaxUpdateDto> _updateValidator;

        public TaxesController(
            ITaxManager manager,
            IValidator<TaxCreateDto> createValidator,
            IValidator<TaxUpdateDto> updateValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetAllTaxes(pharmacyId, search);
            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetStats(pharmacyId);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetTaxById(pharmacyId, id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaxCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.CreateTax(pharmacyId, dto);

            if (result.DuplicateName)
            {
                return Conflict(new { message = $"A tax named \"{dto.Name}\" already exists." });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Tax!.Id }, result.Tax);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaxUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.UpdateTax(pharmacyId, id, dto);

            if (result.NotFound)
            {
                return NotFound();
            }

            if (result.DuplicateName)
            {
                return Conflict(new { message = $"A tax named \"{dto.Name}\" already exists." });
            }

            return Ok(result.Tax);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.ToggleStatus(pharmacyId, id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var pharmacyId = User.GetPharmacyId();
            var deleted = await _manager.DeleteTax(pharmacyId, id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}