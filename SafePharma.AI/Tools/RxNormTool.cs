using System.Text.Json;

namespace SafePharma.AI.Tools
{
    public class RxNormTool : IDrugNameNormalizer
    {
        private readonly HttpClient _httpClient;

        public RxNormTool(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DrugNameNormalizationResult> NormalizeAsync(string drugName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(drugName))
                return new DrugNameNormalizationResult { Found = false };

            try
            {
                var rxcui = await FindRxcuiAsync(drugName, cancellationToken)
                    ?? await FindRxcuiByApproximateMatchAsync(drugName, cancellationToken);

                if (rxcui is null)
                    return new DrugNameNormalizationResult { Found = false };

                var canonicalName = await GetCanonicalNameAsync(rxcui, cancellationToken);

                return new DrugNameNormalizationResult
                {
                    Found = canonicalName is not null,
                    NormalizedName = canonicalName,
                    Rxcui = rxcui
                };
            }
            catch (Exception)
            {
                // Normalization is a best-effort helper — if RxNorm itself fails,
                // callers should fall back to the original name, not crash.
                return new DrugNameNormalizationResult { Found = false };
            }
        }

        private async Task<string?> FindRxcuiAsync(string drugName, CancellationToken cancellationToken)
        {
            var query = Uri.EscapeDataString(drugName);
            using var response = await _httpClient.GetAsync($"rxcui.json?name={query}&search=2", cancellationToken);
            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (doc.RootElement.TryGetProperty("idGroup", out var idGroup) &&
                idGroup.TryGetProperty("rxnormId", out var ids) &&
                ids.GetArrayLength() > 0)
            {
                return ids[0].GetString();
            }

            return null;
        }

        private async Task<string?> FindRxcuiByApproximateMatchAsync(string drugName, CancellationToken cancellationToken)
        {
            var query = Uri.EscapeDataString(drugName);
            using var response = await _httpClient.GetAsync($"approximateTerm.json?term={query}&maxEntries=1", cancellationToken);
            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (doc.RootElement.TryGetProperty("approximateGroup", out var group) &&
                group.TryGetProperty("candidate", out var candidates) &&
                candidates.GetArrayLength() > 0 &&
                candidates[0].TryGetProperty("rxcui", out var rxcui))
            {
                return rxcui.GetString();
            }

            return null;
        }

        private async Task<string?> GetCanonicalNameAsync(string rxcui, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"rxcui/{rxcui}/properties.json", cancellationToken);
            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (doc.RootElement.TryGetProperty("properties", out var props) &&
                props.TryGetProperty("name", out var name))
            {
                return name.GetString();
            }

            return null;
        }
    }
}