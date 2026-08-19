namespace SafePharma.AI.Tools
{
    /// <summary>
    /// A trusted external medical source the agent can query for raw evidence
    /// about a drug (warnings, contraindications, interactions...).
    /// One implementation per source (OpenFDA today; DailyMed/RxNorm later).
    /// </summary>
    public interface IMedicalSourceTool
    {
        /// <summary>Name used in PatientSafetyResult.Sources, e.g. "OpenFDA".</summary>
        string SourceName { get; }

        /// <summary>
        /// Looks up raw evidence for a drug by its scientific/active-ingredient name.
        /// Found = false is a normal outcome (source has no data on this drug) —
        /// per the "insufficient evidence, don't hallucinate" rule, this is NOT an error.
        /// Error is set only for real failures (timeout, source unreachable, bad response).
        /// </summary>
        Task<MedicalEvidenceResult> QueryAsync(string drugName, CancellationToken cancellationToken = default);
    }

    public record MedicalEvidenceResult
    {
        public required bool Found { get; init; }
        public string? RawText { get; init; }
        public string? SourceUrl { get; init; }
        public string? Error { get; init; }
    }
}