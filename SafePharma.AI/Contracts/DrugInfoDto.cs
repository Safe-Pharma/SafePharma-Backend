namespace SafePharma.AI.Contracts
{
    /// <summary>
    /// Info about a single medicine — used both for the drug(s) being checked
    /// and for a patient's current medications list.
    /// Built by BLL from Medicine/PharmacyMedicine, sent as plain data.
    /// </summary>
    public record DrugInfoDto
    {
        /// <summary>
        /// Correlation id supplied by BLL (e.g. the SaleItemId) so a result/issue
        /// can be traced back to the exact invoice line it came from.
        /// Null for entries coming from CurrentMedications (they aren't tied to a sale line).
        /// </summary>
        public string? ClientRef { get; init; }

        public required string TradeName { get; init; }
        public required string ScientificName { get; init; }

        /// <summary>
        /// Distinct from ScientificName where available — falls back to ScientificName
        /// today since the DB doesn't have a dedicated ActiveIngredient field yet.
        /// </summary>
        public string? ActiveIngredient { get; init; }

        public string? Strength { get; init; }
        public string? DosageForm { get; init; }
        public string? Route { get; init; }
        public string? AtcClassification { get; init; }
    }
}