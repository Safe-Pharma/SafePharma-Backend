namespace SafePharma.AI.Contracts
{
    /// <summary>
    /// Snapshot of a patient's clinical profile, assembled by BLL (PatientSafetyManager)
    /// from the database and handed to the AI layer as plain data.
    /// This type has no dependency on DAL entities — BLL is responsible for the mapping.
    /// </summary>
    public record PatientProfileDto
    {
        public required string PatientRef { get; init; } // CustomerId as string — used to correlate results back

        public string? Name { get; init; }
        public int? Age { get; init; }
        public string? Gender { get; init; }
        public decimal? WeightKg { get; init; }

        public bool IsPregnant { get; init; }
        public string? PregnancyTrimester { get; init; } // e.g. "First", "Second", "Third" — nullable until IsPregnant
        public bool IsLactating { get; init; }

        public IReadOnlyList<string> Allergies { get; init; } = [];
        public IReadOnlyList<string> ChronicConditions { get; init; } = [];
        public IReadOnlyList<OrganFunctionDto> OrganImpairments { get; init; } = [];

        public IReadOnlyList<DrugInfoDto> CurrentMedications { get; init; } = [];
    }

    /// <summary>
    /// One organ's impairment level for a patient (from CustomerOrganFunction).
    /// </summary>
    public record OrganFunctionDto
    {
        public required string OrganName { get; init; }       // e.g. "Kidney", "Liver", "Heart"
        public required string ImpairmentLevel { get; init; } // e.g. "Mild", "Moderate", "Severe"
    }
}