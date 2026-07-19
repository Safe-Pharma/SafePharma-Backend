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
        private readonly IValidator<RecordCustomerPaymentDto> _paymentValidator;

        public CustomersController(
            ICustomerManager manager,
            IValidator<CustomerCreateDto> createValidator,
            IValidator<CustomerUpdateDto> updateValidator,
            IValidator<CreateCustomerMedicineHistoryDto> historyValidator,
            IValidator<RecordCustomerPaymentDto> paymentValidator)
        {
            _manager = manager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _historyValidator = historyValidator;
            _paymentValidator = paymentValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.GetAllCustomers(pharmacyId, search);
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
            var result = await _manager.GetCustomerById(pharmacyId, id);
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
            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.ToggleStatus(pharmacyId, id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // Records a payment from this customer AT THIS PHARMACY. Any pharmacist can do
        // this — it's routine, and it's additive (no destructive effect on other pharmacies).
        [HttpPost("{id:guid}/payments")]
        public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordCustomerPaymentDto dto)
        {
            var validationResult = await _paymentValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var pharmacyId = User.GetPharmacyId();
            var result = await _manager.RecordPayment(pharmacyId, id, dto.Amount);

            if (result.CustomerNotFound)
            {
                return NotFound();
            }

            return Ok(result.Customer);
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

        [HttpGet("{customerId:guid}/allergies")]
        public async Task<IActionResult> GetAllergies(Guid customerId)
        {
            var result = await _manager.GetAllergies(customerId);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{customerId:guid}/allergies")]
        public async Task<IActionResult> AssignAllergy(Guid customerId, [FromBody] AssignAllergyDto dto)
        {
            var result = await _manager.AssignAllergy(customerId, dto.AllergyId);
            if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
            if (result.ReferenceNotFound) return NotFound(new { message = "Allergy not found." });
            if (result.AlreadyAssigned) return Conflict(new { message = "This allergy is already assigned to the customer." });
            return NoContent();
        }

        [HttpDelete("{customerId:guid}/allergies/{allergyId:guid}")]
        public async Task<IActionResult> RemoveAllergy(Guid customerId, Guid allergyId)
        {
            var deleted = await _manager.RemoveAllergy(customerId, allergyId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("{customerId:guid}/chronic-conditions")]
        public async Task<IActionResult> GetChronicConditions(Guid customerId)
        {
            var result = await _manager.GetChronicConditions(customerId);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{customerId:guid}/chronic-conditions")]
        public async Task<IActionResult> AssignChronicCondition(Guid customerId, [FromBody] AssignChronicConditionDto dto)
        {
            var result = await _manager.AssignChronicCondition(customerId, dto.ChronicConditionId);
            if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
            if (result.ReferenceNotFound) return NotFound(new { message = "Chronic condition not found." });
            if (result.AlreadyAssigned) return Conflict(new { message = "This condition is already assigned to the customer." });
            return NoContent();
        }

        [HttpDelete("{customerId:guid}/chronic-conditions/{chronicConditionId:guid}")]
        public async Task<IActionResult> RemoveChronicCondition(Guid customerId, Guid chronicConditionId)
        {
            var deleted = await _manager.RemoveChronicCondition(customerId, chronicConditionId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // ---- Organ functions ----

        [HttpGet("{customerId:guid}/organ-functions")]
        public async Task<IActionResult> GetOrganFunctions(Guid customerId)
        {
            var result = await _manager.GetOrganFunctions(customerId);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // Also used to UPDATE an existing organ's impairment level — see AssignOrganFunction in the manager.
        [HttpPost("{customerId:guid}/organ-functions")]
        public async Task<IActionResult> AssignOrganFunction(Guid customerId, [FromBody] AssignOrganFunctionDto dto)
        {
            var result = await _manager.AssignOrganFunction(customerId, dto);
            if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
            if (result.OrganNotFound) return NotFound(new { message = "Organ not found." });
            if (result.ImpairmentLevelNotFound) return NotFound(new { message = "Impairment level not found." });
            return Ok(result.OrganFunction);
        }

        [HttpDelete("{customerId:guid}/organ-functions/{organFunctionId:guid}")]
        public async Task<IActionResult> RemoveOrganFunction(Guid customerId, Guid organFunctionId)
        {
            var deleted = await _manager.RemoveOrganFunction(customerId, organFunctionId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}