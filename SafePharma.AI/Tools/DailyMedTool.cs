using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace SafePharma.AI.Tools
{
    public class DailyMedTool : IMedicalSourceTool
    {
        private static readonly XNamespace Hl7Ns = "urn:hl7-org:v3";


        private static readonly string[] RelevantSectionCodes =
        [
            "34066-1", // Boxed Warning
            "34070-3", // Contraindications
            "34071-1", // Warnings and Precautions
            "34073-7"  // Drug Interactions
        ];

        private static readonly string[] RelevantTitleKeywords =
        [
            "warning", "contraindication", "interaction", "precaution", "boxed"
        ];

        private readonly HttpClient _httpClient;
        private readonly ILogger<DailyMedTool> _logger;

        public string SourceName => "DailyMed";

        public DailyMedTool(HttpClient httpClient, ILogger<DailyMedTool> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<MedicalEvidenceResult> QueryAsync(string drugName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(drugName))
                return new MedicalEvidenceResult { Found = false };

            try
            {
                var setId = await FindSetIdAsync(drugName, cancellationToken);
                _logger.LogInformation("[DailyMed] FindSetIdAsync({DrugName}) → SetId: {SetId}", drugName, setId ?? "(null)");

                if (setId is null)
                    return new MedicalEvidenceResult { Found = false };

                return await FetchRelevantSectionsAsync(setId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DailyMed] QueryAsync failed for {DrugName}", drugName);
                return new MedicalEvidenceResult { Found = false, Error = ex.Message };
            }
        }

        private async Task<string?> FindSetIdAsync(string drugName, CancellationToken cancellationToken)
        {
            var query = Uri.EscapeDataString(drugName);
            using var response = await _httpClient.GetAsync($"spls.json?drug_name={query}&pagesize=1", cancellationToken);

            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (doc.RootElement.TryGetProperty("data", out var data) &&
                data.GetArrayLength() > 0 &&
                data[0].TryGetProperty("setid", out var setIdElement))
            {
                return setIdElement.GetString();
            }

            return null;
        }

        private async Task<MedicalEvidenceResult> FetchRelevantSectionsAsync(string setId, CancellationToken cancellationToken)
        {

            using var response = await _httpClient.GetAsync($"spls/{setId}.xml", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[DailyMed] Label fetch returned {Status} for SetId {SetId}", response.StatusCode, setId);
                return new MedicalEvidenceResult { Found = false };
            }

            var xml = await response.Content.ReadAsStringAsync(cancellationToken);
            var xdoc = XDocument.Parse(xml);

            var sections = xdoc.Descendants(Hl7Ns + "section");
            var snippets = new List<string>();

            foreach (var section in sections)
            {
                var code = section.Element(Hl7Ns + "code")?.Attribute("code")?.Value;
                var title = section.Element(Hl7Ns + "title")?.Value?.Trim() ?? "";

                var isRelevant =
                    (code is not null && RelevantSectionCodes.Contains(code)) ||
                    RelevantTitleKeywords.Any(k => title.Contains(k, StringComparison.OrdinalIgnoreCase));

                if (!isRelevant) continue;

                var textElement = section.Element(Hl7Ns + "text");
                if (textElement is not null)
                {
         
                    var text = textElement.Value.Trim();
                    if (!string.IsNullOrWhiteSpace(text))
                        snippets.Add($"[{title}] {text}");
                }
            }

            if (snippets.Count == 0)
            {
                _logger.LogWarning("[DailyMed] No relevant sections matched for SetId {SetId}", setId);
                return new MedicalEvidenceResult { Found = false };
            }

            return new MedicalEvidenceResult
            {
                Found = true,
                RawText = string.Join("\n\n", snippets),
                SourceUrl = $"https://dailymed.nlm.nih.gov/dailymed/drugInfo.cfm?setid={setId}"
            };
        }
    }
}