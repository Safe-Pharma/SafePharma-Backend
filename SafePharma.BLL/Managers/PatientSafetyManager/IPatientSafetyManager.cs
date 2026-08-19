using SafePharma.Common;
using SafePharma.AI.Contracts;

namespace SafePharma.BLL;

public interface IPatientSafetyManager
{
    Task<GeneralResult<PatientProfileDto>> LoadPatientProfileAsync(Guid customerId);
    Task<GeneralResult<PatientSafetyCheckResponse>> CheckAsync(
        Guid pharmacyId,
        Guid customerId,
        IEnumerable<(Guid PharmacyMedicineId, Guid SaleItemId)> itemsToCheck,
        string language = "en");

    /// <summary>
    /// Multi-patient version of CheckAsync — checks every patient group in a single
    /// call to the AI agent (one LLM run instead of one per patient). A patient whose
    /// profile can't be loaded, or who has no valid medicines to check, is reported
    /// back as a failed PatientSafetyResult instead of failing the whole request.
    /// </summary>
    Task<GeneralResult<PatientSafetyCheckResponse>> CheckAsync(
        Guid pharmacyId,
        IEnumerable<PatientCheckRequestGroup> patients,
        string language = "en");

    IAsyncEnumerable<PatientSafetyStreamEvent> CheckStreamAsync(
    Guid pharmacyId,
    Guid customerId,
    IEnumerable<(Guid PharmacyMedicineId, Guid SaleItemId)> itemsToCheck,
    string language = "en",
    CancellationToken cancellationToken = default);
}