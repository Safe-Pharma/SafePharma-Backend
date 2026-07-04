using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // GET api/taxes?search=vat
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await _manager.GetAllTaxes(search);
            return Ok(result);
        }

        // GET api/taxes/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _manager.GetStats();
            return Ok(result);
        }

        // GET api/taxes/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _manager.GetTaxById(id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/taxes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaxCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _manager.CreateTax(dto);

            if (result.DuplicateName)
            {
                return Conflict(new { message = $"A tax named \"{dto.Name}\" already exists." });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Tax!.Id }, result.Tax);
        }

        // PUT api/taxes/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaxUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _manager.UpdateTax(id, dto);

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

        // PATCH api/taxes/{id}/status  -> toggles Active/Inactive (matches "Activate"/"Deactivate" menu item)
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

        // DELETE api/taxes/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _manager.DeleteTax(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
