using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    // Any authenticated pharmacy staff. Everything here is scoped to
    // the caller's own pharmacy via User.GetPharmacyId() — never global.
    [Route("api/pharmacy-medicines")]
    [ApiController]
    [Authorize]
    public class PharmacyMedicinesController : ControllerBase
    {
        private readonly IMedicineManager _manager;
        private readonly IValidator<MedicineCreateDto> _createValidator;
        private readonly IValidator<LinkExistingMedicineDto> _linkValidator;
        private readonly IValidator<PharmacyMedicineUpdateDto> _pharmacyUpdateValidator;

        public PharmacyMedicinesController(
            IMedicineManager manager,
            IValidator<MedicineCreateDto> createValidator,
            IValidator<LinkExistingMedicineDto> linkValidator,
            IValidator<PharmacyMedicineUpdateDto> pharmacyUpdateValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _linkValidator = linkValidator;
            _pharmacyUpdateValidator = pharmacyUpdateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? category, [FromQuery] bool includeInactive = false)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetAllMedicines(pharmacyId, search, category, includeInactive);
            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetStats(pharmacyId);
            return Ok(result);
        }

        // STEP 1: Search the global catalog before deciding link-existing vs local.
        [HttpGet("catalog-search")]
        public async Task<IActionResult> SearchGlobalCatalog([FromQuery] string? query)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.SearchGlobalCatalog(pharmacyId, query);
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

        // STEP 2: Global medicine found -> link it into this pharmacy's catalog.
        [HttpPost("link-existing")]
        public async Task<IActionResult> LinkExisting([FromBody] LinkExistingMedicineDto dto)
        {
            var validationResult = await _linkValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.LinkExistingMedicine(pharmacyId, dto);

            if (result.MedicineNotFound) return NotFound(new { message = "Global medicine not found." });
            if (result.AlreadyLinked) return Conflict(new { message = "This medicine is already in your pharmacy." });
            if (result.InvalidTaxIds) return BadRequest(new { message = "One or more taxes are invalid for this pharmacy." });

            return CreatedAtAction(nameof(GetById), new { id = result.Medicine!.Id }, result.Medicine);
        }

        // STEP 3: Not found anywhere -> create a medicine scoped ONLY to this pharmacy.
        [HttpPost("local")]
        [Authorize(Policy = AuthPolicies.AdminOrOwner)]
        public async Task<IActionResult> CreateLocal([FromBody] MedicineCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.CreateLocalMedicine(pharmacyId, dto);

            if (result.ExistingMedicineFound)
            {
                return Conflict(new
                {
                    message = $"\"{dto.TradeNameEn}\" already exists. Use link-existing instead.",
                    existingMedicineId = result.ExistingMedicineId
                });
            }
            if (result.DuplicateTradeNameInPharmacy)
            {
                return Conflict(new { message = $"\"{dto.TradeNameEn}\" already exists in your pharmacy." });
            }
            if (result.InvalidTaxIds) return BadRequest(new { message = "One or more taxes are invalid for this pharmacy." });
            if (result.DuplicateSku) return Conflict(new { message = "That SKU is already in use in your pharmacy." });

            return CreatedAtAction(nameof(GetById), new { id = result.Medicine!.Id }, result.Medicine);
        }

        // Admin/Owner edit: pharmacy-specific fields only (price, tax, stock, SKU).
        [HttpPut("{id:guid}")]
        [Authorize(Policy = AuthPolicies.AdminOrOwner)]
        public async Task<IActionResult> Update(Guid id, [FromBody] PharmacyMedicineUpdateDto dto)
        {
            var validationResult = await _pharmacyUpdateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.UpdatePharmacyMedicine(pharmacyId, id, dto);

            if (result.NotFound) return NotFound();
            if (result.InvalidTaxIds) return BadRequest(new { message = "One or more taxes are invalid for this pharmacy." });
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

        [HttpGet("{id:guid}/details")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetMedicineDetails(pharmacyId, id);
            if (result is null) return NotFound();
            return Ok(result);
        }
    }
}