namespace SafePharma.AI.Contracts
{
    /// <summary>
    /// A single safety finding for one patient (e.g. one drug-drug interaction,
    /// one drug-disease conflict, one dosage concern...).
    /// </summary>
    public record SafetyIssueDto
    {
        public required string Type { get; init; }       // "Drug-Drug" | "Drug-Disease" | "Drug-Allergy" | "Dosage" | "Pregnancy" | "Lactation" | "OrganFunction"
        public required string Severity { get; init; }    // "Minor" | "Moderate" | "Major"
        public required string Reason { get; init; }

        /// <summary>
        /// ClientRef(s) of the drug(s) involved (from DrugInfoDto.ClientRef).
        /// One entry for a single-drug issue (e.g. Drug-Allergy), two for
        /// a Drug-Drug interaction.
        /// </summary>
        public required IReadOnlyList<string> RelatedDrugRefs { get; init; }


    }

    /// <summary>
    /// A suggested alternative medication, kept separate from the free-text
    /// Recommendation so the UI can render it as its own element (e.g. a chip
    /// or button) instead of parsing it out of a sentence.
    /// </summary>
    public record SuggestedAlternativeDto
    {
        public required string DrugName { get; init; }
        public string? Reason { get; init; } // short — why this is safer, e.g. "No cross-reactivity with NSAIDs"
    }
}