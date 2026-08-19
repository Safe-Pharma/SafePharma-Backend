using System.Net;
using System.Text.Json;

namespace SafePharma.AI.Tools
{
    /// <summary>
    /// Queries OpenFDA's Drug Label endpoint for warnings, contraindications,
    /// and interaction text by active ingredient / substance name.
    /// Free, no API key required for reasonable usage volumes.
    /// </summary>
    public class OpenFdaTool : IMedicalSourceTool
    {
        private static readonly string[] RelevantSections =
        [
            "warnings", "contraindications", "drug_interactions", "warnings_and_cautions"
        ];

        private readonly HttpClient _httpClient;

        public string SourceName => "OpenFDA";

        public OpenFdaTool(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MedicalEvidenceResult> QueryAsync(string drugName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(drugName))
                return new MedicalEvidenceResult { Found = false };

            var query = Uri.EscapeDataString($"openfda.substance_name:\"{drugName}\"");
            var url = $"drug/label.json?search={query}&limit=1";

            try
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new MedicalEvidenceResult { Found = false }; // OpenFDA: no match, not an error

                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

                if (!doc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                    return new MedicalEvidenceResult { Found = false };

                var record = results[0];
                var snippets = new List<string>();

                foreach (var section in RelevantSections)
                {
                    if (record.TryGetProperty(section, out var value) && value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in value.EnumerateArray())
                            snippets.Add(item.GetString() ?? string.Empty);
                    }
                }

                if (snippets.Count == 0)
                    return new MedicalEvidenceResult { Found = false };

                return new MedicalEvidenceResult
                {
                    Found = true,
                    RawText = string.Join("\n\n", snippets),
                    SourceUrl = $"https://api.fda.gov/drug/label.json?search={query}"
                };
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                return new MedicalEvidenceResult { Found = false, Error = ex.Message };
            }
        }
    }
}