using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierManager _manager;
        private readonly IValidator<SupplierCreateDto> _createValidator;
        private readonly IValidator<SupplierUpdateDto> _updateValidator;

        public SuppliersController(
            ISupplierManager manager,
            IValidator<SupplierCreateDto> createValidator,
            IValidator<SupplierUpdateDto> updateValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetAllSuppliers(pharmacyId, search);
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
            var result = await _manager.GetSupplierById(pharmacyId, id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.CreateSupplier(pharmacyId, dto);

            if (result.DuplicateName)
            {
                return Conflict(new { message = $"A supplier named \"{dto.Name}\" already exists." });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Supplier!.Id }, result.Supplier);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SupplierUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.UpdateSupplier(pharmacyId, id, dto);

            if (result.NotFound)
            {
                return NotFound();
            }

            if (result.DuplicateName)
            {
                return Conflict(new { message = $"A supplier named \"{dto.Name}\" already exists." });
            }

            return Ok(result.Supplier);
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
            var deleted = await _manager.DeleteSupplier(pharmacyId, id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
