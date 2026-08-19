using SafePharma.AI.Contracts;

namespace SafePharma.AI.Agent
{
    /// <summary>
    /// The single entry point BLL (PatientSafetyManager) calls into.
    /// Everything about Microsoft Agent Framework, tools, and prompting
    /// stays hidden behind this interface.
    /// </summary>
    public interface IPatientSafetyAgent
    {
        Task<PatientSafetyCheckResponse> CheckAsync(
            PatientSafetyCheckRequest request,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<PatientSafetyStreamEvent> CheckStreamAsync(
            PatientSafetyCheckRequest request,
            CancellationToken cancellationToken = default);
    }
}