using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using System.Security.Claims;
using System.Text.Json;
using SafePharma.AI.Contracts;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientSafetyController : ControllerBase
    {
        private readonly IPatientSafetyManager _patientSafetyManager;

        public PatientSafetyController(IPatientSafetyManager patientSafetyManager)
        {
            _patientSafetyManager = patientSafetyManager;
        }

        public record CheckRequestDto(
            List<CheckPatientDto> Patients,
            string Language = "en");

        public record CheckPatientDto(Guid CustomerId, List<CheckItemDto> Items);

        public record CheckItemDto(Guid PharmacyMedicineId, Guid SaleItemId);

        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] CheckRequestDto dto)
        {
            var pharmacyIdClaim = User.FindFirst("PharmacyId")?.Value;
            if (!Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                return Unauthorized("Pharmacy context not found.");
            }

            var patients = dto.Patients.Select(p => new PatientCheckRequestGroup(
                p.CustomerId,
                p.Items.Select(i => (i.PharmacyMedicineId, i.SaleItemId)).ToList()));

            var result = await _patientSafetyManager.CheckAsync(pharmacyId, patients, dto.Language);

            if (!result.Success)
            {
                return result.Message == "Customer not found."
                    ? NotFound(result)
                    : BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("check-stream")]
        public async Task CheckStream([FromBody] CheckStreamRequestDto dto, CancellationToken cancellationToken)
        {
            var pharmacyIdClaim = User.FindFirst("PharmacyId")?.Value;
            if (!Guid.TryParse(pharmacyIdClaim, out var pharmacyId))
            {
                Response.StatusCode = 401;
                return;
            }

            Response.ContentType = "text/event-stream";
            Response.Headers.Append("Cache-Control", "no-cache");

            var items = dto.Items.Select(i => (i.PharmacyMedicineId, i.SaleItemId));

            await foreach (var evt in _patientSafetyManager.CheckStreamAsync(
                pharmacyId, dto.CustomerId, items, dto.Language, cancellationToken))
            {
                var json = JsonSerializer.Serialize(evt);
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }

        // check-stream keeps the old single-customer shape — streaming a merged
        // multi-patient response isn't supported yet, so this DTO is kept separate
        // from the array-based CheckRequestDto used by "check".
        public record CheckStreamRequestDto(
            Guid CustomerId,
            List<CheckItemDto> Items,
            string Language = "en");
    }
}