using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    // Owner-only. Manages the GLOBAL medicine catalog.
    // No PharmacyId is ever read here — the owner's token has none.
    [Route("api/global-medicines")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.OwnerOnly)]
    public class OwnerMedicinesController : ControllerBase
    {
        private readonly IMedicineManager _manager;
        private readonly IValidator<GlobalMedicineCreateDto> _createValidator;
        private readonly IValidator<GlobalMedicineUpdateDto> _updateValidator;

        public OwnerMedicinesController(
            IMedicineManager manager,
            IValidator<GlobalMedicineCreateDto> createValidator,
            IValidator<GlobalMedicineUpdateDto> updateValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // Create a brand-new global medicine. Pharmacies later use
        // POST /api/pharmacy-medicines/link-existing to bring it into their own catalog.
        [HttpPost]
        public async Task<IActionResult> CreateGlobal([FromBody] GlobalMedicineCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var result = await _manager.CreateGlobalMedicine(dto);

            if (result.ExistingMedicineFound)
            {
                return Conflict(new
                {
                    message = $"\"{dto.TradeNameEn}\" already exists in the global catalog.",
                    existingMedicineId = result.ExistingMedicineId
                });
            }

            return CreatedAtAction(nameof(CreateGlobal), new { id = result.Medicine!.Id }, result.Medicine);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateGlobal(Guid id, [FromBody] GlobalMedicineUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var result = await _manager.UpdateGlobalMedicine(id, dto);

            if (result.NotFound) return NotFound();
            if (result.DuplicateTradeName)
                return Conflict(new { message = $"\"{dto.TradeNameEn}\" is already in use." });

            return Ok(result.Medicine!.ToGlobalDto());
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ToggleGlobalStatus(Guid id)
        {
            var medicine = await _manager.ToggleGlobalStatus(id);
            if (medicine is null) return NotFound();
            return Ok(medicine.ToGlobalDto());
        }
    }
}