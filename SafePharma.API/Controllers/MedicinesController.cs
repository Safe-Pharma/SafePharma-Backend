using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineManager _manager;
        private readonly IValidator<MedicineCreateDto> _createValidator;
        private readonly IValidator<MedicineUpdateDto> _updateValidator;

        public MedicinesController(
            IMedicineManager manager,
            IValidator<MedicineCreateDto> createValidator,
            IValidator<MedicineUpdateDto> updateValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? category)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetAllMedicines(pharmacyId, search, category);
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
            var result = await _manager.GetMedicineById(pharmacyId, id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicineCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.CreateMedicine(pharmacyId, dto);

            if (result.DuplicateTradeName)
                return Conflict(new { message = $"\"{dto.TradeNameEn}\" is already in your catalog." });

            return CreatedAtAction(nameof(GetById), new { id = result.Medicine!.Id }, result.Medicine);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MedicineUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.UpdateMedicine(pharmacyId, id, dto);

            if (result.NotFound) return NotFound();
            if (result.DuplicateTradeName)
                return Conflict(new { message = $"\"{dto.TradeNameEn}\" is already in use." });

            return Ok(result.Medicine);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.ToggleStatus(pharmacyId, id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var pharmacyId = User.GetPharmacyId();
            var deleted = await _manager.DeleteMedicine(pharmacyId, id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}