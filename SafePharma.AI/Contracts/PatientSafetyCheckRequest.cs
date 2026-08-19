namespace SafePharma.AI.Contracts
{
    /// <summary>
    /// Top-level request sent from BLL (PatientSafetyManager) to the AI layer.
    /// Supports checking multiple patients in one call (e.g. a "Check All" on
    /// an invoice with items for different family members).
    /// </summary>
    public record PatientSafetyCheckRequest
    {
        /// <summary>
        /// Optional — lets BLL correlate the whole check back to a Sale for auditing.
        /// Just a string on this side; the AI layer doesn't know it's a Guid.
        /// </summary>
        public string? SaleRef { get; init; }
        /// <summary>
        /// "ar" or "en" — controls the language of free-text output
        /// (SafetyIssueDto.Reason, PatientSafetyResult.Recommendation) only.
        /// Enum-like fields (Type, Severity, OverallDecision) always stay
        /// as fixed English codes and are localized client-side/BLL-side.
        /// </summary>
        public required string Language { get; init; }
        public required IReadOnlyList<PatientCheckGroup> Patients { get; init; }
    }

    /// <summary>
    /// One patient's worth of work: their profile (which already carries
    /// CurrentMedications) plus the new drug(s) being checked right now.
    /// Kept separate from PatientProfileDto so a single agent run can reason
    /// about drug-drug interactions among DrugsToCheck AND against
    /// CurrentMedications in the profile.
    /// </summary>
    public record PatientCheckGroup
    {
        public required PatientProfileDto Profile { get; init; }

        /// <summary>
        /// The drug(s) being scanned/added right now — each with its own
        /// ClientRef (e.g. SaleItemId) so results map back to the right invoice line.
        /// </summary>
        public required IReadOnlyList<DrugInfoDto> DrugsToCheck { get; init; }
    }
}