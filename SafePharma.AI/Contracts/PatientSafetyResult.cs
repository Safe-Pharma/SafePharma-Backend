namespace SafePharma.AI.Contracts
{
    /// <summary>
    /// Response for the whole check — one entry per patient that was checked.
    /// </summary>
    public record PatientSafetyCheckResponse
    {
        public required IReadOnlyList<PatientSafetyResult> Results { get; init; }
    }

    /// <summary>
    /// Outcome for a single patient. CheckSucceeded distinguishes a real result
    /// from "we couldn't complete this patient's check" (e.g. an external source
    /// timed out) — per the partial-failure handling we agreed on: one patient
    /// failing must never block the others.
    /// </summary>
    public record PatientSafetyResult
    {
        public required string PatientRef { get; init; }

        public required bool CheckSucceeded { get; init; }
        public string? FailureReason { get; init; } // set only when CheckSucceeded == false

        public string? OverallDecision { get; init; }   // "Approve" | "Warn" | "Block" — null if CheckSucceeded == false
        public int? RiskScore { get; init; }             // 0–100, null if CheckSucceeded == false
        public string? Confidence { get; init; }

        public IReadOnlyList<SafetyIssueDto> Issues { get; init; } = [];
        public string? Recommendation { get; init; }
        public IReadOnlyList<SuggestedAlternativeDto> SuggestedAlternatives { get; init; } = [];

        public IReadOnlyList<string> Sources { get; init; } = [];
    }
}