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
        private readonly IValidator<LinkExistingMedicineDto> _linkValidator;
        private readonly IValidator<PharmacyMedicineUpdateDto> _pharmacyUpdateValidator;
        private readonly IValidator<GlobalMedicineUpdateDto> _globalUpdateValidator;

        public MedicinesController(
            IMedicineManager manager,
            IValidator<MedicineCreateDto> createValidator,
            IValidator<LinkExistingMedicineDto> linkValidator,
            IValidator<PharmacyMedicineUpdateDto> pharmacyUpdateValidator,
            IValidator<GlobalMedicineUpdateDto> globalUpdateValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _linkValidator = linkValidator;
            _pharmacyUpdateValidator = pharmacyUpdateValidator;
            _globalUpdateValidator = globalUpdateValidator;
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

        // STEP 1: Search First
        [HttpGet("global/search")]
        public async Task<IActionResult> SearchGlobal([FromQuery] string? query)
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

        // STEP 2: Existing Medicine Found -> "Add to Pharmacy"
        [HttpPost("link-existing")]
        public async Task<IActionResult> LinkExisting([FromBody] LinkExistingMedicineDto dto)
        {
            var validationResult = await _linkValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.LinkExistingMedicine(pharmacyId, dto);

            if (result.MedicineNotFound) return NotFound(new { message = "Global medicine not found." });
            if (result.AlreadyLinked) return Conflict(new { message = "This medicine is already in your pharmacy." });

            return CreatedAtAction(nameof(GetById), new { id = result.Medicine!.Id }, result.Medicine);
        }

        // STEP 3: Medicine Not Found -> "Create & Add to Pharmacy"
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicineCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.CreateMedicine(pharmacyId, dto);

            if (result.ExistingMedicineFound)
            {
                return Conflict(new
                {
                    message = $"\"{dto.TradeNameEn}\" already exists in the global catalog. Use link-existing instead.",
                    existingMedicineId = result.ExistingMedicineId
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Medicine!.Id }, result.Medicine);
        }

        // Pharmacist edit: pharmacy-specific fields only
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PharmacyMedicineUpdateDto dto)
        {
            var validationResult = await _pharmacyUpdateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.UpdatePharmacyMedicine(pharmacyId, id, dto);

            if (result.NotFound) return NotFound();
            return Ok(result.Medicine);
        }

        // Admin-only: edit global catalog data
        [HttpPut("global/{id:guid}")]
        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        public async Task<IActionResult> UpdateGlobal(Guid id, [FromBody] GlobalMedicineUpdateDto dto)
        {
            var validationResult = await _globalUpdateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var result = await _manager.UpdateGlobalMedicine(id, dto);

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