using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    // Customers are global (shared across all pharmacies on the platform).
    // Medicine history is also global — it links to the global Medicine catalog
    // (or a free-text scientific name when the medicine isn't in the catalog),
    // not to any specific pharmacy's inventory.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerManager _manager;
        private readonly IValidator<CustomerCreateDto> _createValidator;
        private readonly IValidator<CustomerUpdateDto> _updateValidator;
        private readonly IValidator<CreateCustomerMedicineHistoryDto> _historyValidator;

        public CustomersController(
            ICustomerManager manager,
            IValidator<CustomerCreateDto> createValidator,
            IValidator<CustomerUpdateDto> updateValidator,
            IValidator<CreateCustomerMedicineHistoryDto> historyValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _historyValidator = historyValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await _manager.GetAllCustomers(search);
            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _manager.GetStats();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _manager.GetCustomerById(id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _manager.CreateCustomer(dto);

            if (result.DuplicatePhone)
            {
                return Conflict(new { message = $"A customer with phone \"{dto.Phone}\" already exists." });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Customer!.Id }, result.Customer);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _manager.UpdateCustomer(id, dto);

            if (result.NotFound)
            {
                return NotFound();
            }

            if (result.DuplicatePhone)
            {
                return Conflict(new { message = $"A customer with phone \"{dto.Phone}\" already exists." });
            }

            return Ok(result.Customer);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var result = await _manager.ToggleStatus(id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _manager.DeleteCustomer(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        // ---- Medicine history (global — linked to the Medicine catalog, or free-text) ----

        [HttpGet("{customerId:guid}/medicine-history")]
        public async Task<IActionResult> GetMedicineHistory(Guid customerId, [FromQuery] bool? isActive)
        {
            var result = await _manager.GetMedicineHistory(customerId, isActive);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("{customerId:guid}/medicine-history")]
        public async Task<IActionResult> AddMedicineHistory(Guid customerId, [FromBody] CreateCustomerMedicineHistoryDto dto)
        {
            var validationResult = await _historyValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _manager.AddMedicineHistory(customerId, dto);

            if (result.CustomerNotFound)
            {
                return NotFound(new { message = "Customer not found." });
            }

            if (result.MedicineNotFound)
            {
                return NotFound(new { message = "Medicine not found in the global catalog." });
            }

            return CreatedAtAction(nameof(GetMedicineHistory), new { customerId }, result.History);
        }

        [HttpPatch("{customerId:guid}/medicine-history/{historyId:guid}/toggle-active")]
        public async Task<IActionResult> ToggleMedicineActive(Guid customerId, Guid historyId)
        {
            var result = await _manager.ToggleMedicineActive(customerId, historyId);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{customerId:guid}/medicine-history/{historyId:guid}")]
        [Authorize(Policy = AuthPolicies.OwnerOnly)]
        public async Task<IActionResult> DeleteMedicineHistory(Guid customerId, Guid historyId)
        { 
            var deleted = await _manager.DeleteMedicineHistory(customerId, historyId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
