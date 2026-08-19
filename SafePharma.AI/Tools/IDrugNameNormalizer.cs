namespace SafePharma.AI.Tools
{
    /// <summary>
    /// Normalizes a drug name to its canonical US-based active-ingredient name
    /// (via RxNorm), so other sources like OpenFDA — which index by US naming
    /// (e.g. "acetaminophen") — can find a match even when the input uses an
    /// international name (e.g. "paracetamol").
    /// </summary>
    public interface IDrugNameNormalizer
    {
        Task<DrugNameNormalizationResult> NormalizeAsync(string drugName, CancellationToken cancellationToken = default);
    }

    public record DrugNameNormalizationResult
    {
        public required bool Found { get; init; }
        public string? NormalizedName { get; init; }
        public string? Rxcui { get; init; }
    }
}