using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using SafePharma.AI.Contracts;
using SafePharma.AI.Rag;
using SafePharma.AI.Tools;
using System.ClientModel;
using System.ComponentModel;

namespace SafePharma.AI.Agent
{
    public class PatientSafetyAgent : IPatientSafetyAgent
    {
        private readonly AzureOpenAiSettings _settings;
        private readonly IMedicalSourceTool _medicalSourceTool;
        private readonly DailyMedTool _dailyMedTool;
        private readonly IDrugNameNormalizer _drugNameNormalizer;
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorStore _vectorStore;
        private readonly ILogger<PatientSafetyAgent> _logger;

        public PatientSafetyAgent(
            AzureOpenAiSettings settings,
            IMedicalSourceTool medicalSourceTool,
            DailyMedTool dailyMedTool,
            IDrugNameNormalizer drugNameNormalizer,
            IEmbeddingService embeddingService,
            IVectorStore vectorStore,
            ILogger<PatientSafetyAgent> logger)
        {
            _settings = settings;
            _medicalSourceTool = medicalSourceTool;
            _dailyMedTool = dailyMedTool;
            _drugNameNormalizer = drugNameNormalizer;
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
            _logger = logger;
        }

        [Description("Normalizes a drug name (which may be in an international/INN form like 'Paracetamol') to its canonical US active-ingredient name (like 'Acetaminophen') for use with other lookup tools.")]
        private async Task<string> NormalizeDrugName(
            [Description("The drug name as given, e.g. 'Paracetamol'")] string drugName)
        {
            _logger.LogInformation("[Agent Tool Call] NormalizeDrugName invoked with: {DrugName}", drugName);

            var result = await _drugNameNormalizer.NormalizeAsync(drugName);

            _logger.LogInformation(
                "[Agent Tool Call] NormalizeDrugName result — Found: {Found}, NormalizedName: {NormalizedName}",
                result.Found, result.NormalizedName ?? "(none)");

            return result.Found
                ? $"Canonical name: {result.NormalizedName}"
                : $"Could not normalize '{drugName}' — use the original name as-is.";
        }

        [Description("Looks up official drug safety evidence (warnings, contraindications, interactions) for a given drug by its scientific/active ingredient name, from OpenFDA.")]
        private async Task<string> LookupDrugEvidence(
            [Description("The scientific name or active ingredient of the drug, e.g. 'ibuprofen'")] string drugName)
        {
            // 1. فحص الـ RAG (Vector Store) الأول
            var queryEmbedding = await _embeddingService.GetEmbeddingAsync(drugName);
            var cachedResults = await _vectorStore.SearchSimilarAsync(queryEmbedding, topK: 2, minSimilarityScore: 0.85);
            var cached = cachedResults.FirstOrDefault(r => r.Source == "OpenFDA");

            if (cached is not null)
            {
                _logger.LogInformation("[RAG] Cache hit for {DrugName} (OpenFDA), score={Score}", drugName, cached.SimilarityScore);
                return $"Source: OpenFDA (cached)\n{cached.Content}";
            }

            // 2. مفيش تطابق قوي — روح للمصدر الحي
            _logger.LogInformation("[RAG] Cache miss for {DrugName} (OpenFDA), calling live source", drugName);
            var result = await _medicalSourceTool.QueryAsync(drugName);

            // 3. خزّن النتيجة الجديدة للمرة الجاية
            if (result.Found)
            {
                var embedding = await _embeddingService.GetEmbeddingAsync(drugName);
                await _vectorStore.UpsertAsync(new VectorStoreEntry
                {
                    Id = $"{drugName}-openfda".Replace(" ", "-").ToLowerInvariant(),
                    DrugName = drugName,
                    Content = result.RawText!,
                    Source = "OpenFDA",
                    Embedding = embedding
                });
            }

            return result.Found
                ? $"Source: {_medicalSourceTool.SourceName}\n{result.RawText}"
                : $"No evidence found in {_medicalSourceTool.SourceName} for '{drugName}'.";
        }

        [Description("Looks up official drug safety evidence (warnings, contraindications, boxed warnings, drug interactions) from DailyMed's structured product labels, by drug name.")]
        private async Task<string> LookupDailyMedEvidence(
            [Description("The scientific name or active ingredient of the drug, e.g. 'ibuprofen'")] string drugName)
        {
            // 1. فحص الـ RAG الأول
            var queryEmbedding = await _embeddingService.GetEmbeddingAsync(drugName);
            var cachedResults = await _vectorStore.SearchSimilarAsync(queryEmbedding, topK: 2, minSimilarityScore: 0.85);
            var cached = cachedResults.FirstOrDefault(r => r.Source == "DailyMed");

            if (cached is not null)
            {
                _logger.LogInformation("[RAG] Cache hit for {DrugName} (DailyMed), score={Score}", drugName, cached.SimilarityScore);
                return $"Source: DailyMed (cached)\n{cached.Content}";
            }

            // 2. مفيش تطابق قوي — روح للمصدر الحي
            _logger.LogInformation("[RAG] Cache miss for {DrugName} (DailyMed), calling live source", drugName);
            var result = await _dailyMedTool.QueryAsync(drugName);

            // 3. خزّن النتيجة الجديدة
            if (result.Found)
            {
                var embedding = await _embeddingService.GetEmbeddingAsync(drugName);
                await _vectorStore.UpsertAsync(new VectorStoreEntry
                {
                    Id = $"{drugName}-dailymed".Replace(" ", "-").ToLowerInvariant(),
                    DrugName = drugName,
                    Content = result.RawText!,
                    Source = "DailyMed",
                    Embedding = embedding
                });
            }

            return result.Found
                ? $"Source: {_dailyMedTool.SourceName}\n{result.RawText}"
                : $"No evidence found in {_dailyMedTool.SourceName} for '{drugName}'.";
        }

        private record AgentOutput(
            string OverallDecision,
            int RiskScore,
            List<AgentIssue> Issues,
            List<AgentAlternative> SuggestedAlternatives,
            string Recommendation
        );

        private record AgentIssue(
            string Type,
            string Severity,
            string Reason,
            List<string> RelatedDrugRefs
        );

        private record AgentAlternative(
            string DrugName,
            string? Reason
        );

        private const string Instructions = """
            You are a pharmacist safety assistant. Use LookupDrugEvidence (OpenFDA) and
            LookupDailyMedEvidence (DailyMed) to check official evidence before making
            any claim about a drug's safety. Never invent warnings or interactions not
            supported by at least one tool's output.

            For every drug being checked, call BOTH LookupDrugEvidence and
            LookupDailyMedEvidence — do not rely on a single source. If the two
            sources agree, note that agreement strengthens confidence. If they
            conflict or one finds evidence the other doesn't, note this explicitly
            in your reasoning and prefer the source that found evidence over "no
            evidence found" from the other.

            RiskScore MUST be an integer from 0 to 100, where:
            - 0-30 = Low risk (safe to dispense)
            - 31-65 = Medium risk (warn pharmacist, suggest alternative)
            - 66-100 = High risk (block dispensing)
            Choose the score based on the highest-severity issue found, not an average.

            When there is more than one drug to check for the same patient (in
            "Drugs to check" and/or "Current medications"), you MUST explicitly check
            every pairwise combination for Drug-Drug interactions — not just each
            drug individually against the patient's allergies/conditions. Look up
            evidence for each drug separately, then reason about how they interact
            with each other. Report each interaction found as its own Issue with
            Type "Drug-Drug" and RelatedDrugRefs containing BOTH drugs involved.

            When classifying an Issue's Type, actively cross-check each piece of evidence
            against the patient's specific profile before defaulting to "General Warning":
            - If the evidence mentions an allergy/hypersensitivity and the patient has a
              matching or related allergy, use "Drug-Allergy" (not General Warning).
            - If the evidence mentions organ-related toxicity (liver/kidney/heart) and the
              patient has a matching organ impairment, use "Organ Function".
            - If the evidence mentions interaction with a drug class and the patient's
              CurrentMedications or DrugsToCheck contains a drug from that class, use
              "Drug-Drug".
            Only use "General Warning" for evidence that does NOT match anything in this
            specific patient's profile.

            Each Issue's Type MUST be exactly one of the following values — do not
            invent new type names or use any other wording:
            - "Drug-Drug"        (interaction between two or more of the patient's drugs)
            - "Drug-Disease"      (conflict with a chronic condition)
            - "Drug-Allergy"      (conflict with a documented allergy)
            - "Dosage"            (dose-related risk, e.g. overdose/toxicity thresholds)
            - "Pregnancy"         (pregnancy-related risk)
            - "Lactation"         (breastfeeding-related risk)
            - "Organ Function"    (risk related to kidney/liver/heart impairment)
            - "General Warning"   (a serious label warning about the drug itself that
                                   doesn't fit any category above, e.g. a boxed warning
                                   not tied to a specific patient factor)
            If a warning is about the drug in general rather than this specific
            patient's profile, use "General Warning" — never invent a new category.

            RelatedDrugRefs MUST contain ONLY the exact ClientRef value(s) given in the
            input (e.g. "item-1"), copied exactly — never append the drug name or any
            other text to it.

            If a tool finds no evidence for a combination drug name (containing
            "/" or "and"), try calling it again with just the first active ingredient
            before concluding evidence is unavailable.

            If you suggest an alternative medication, put it ONLY in
            SuggestedAlternatives (as its own DrugName + short Reason) — never name a
            specific alternative drug inside Recommendation itself.

            Respond ONLY in the required JSON structure. Keep Recommendation to
            1-2 short sentences stating the decision and the main reason only —
            no drug names, no alternatives. The Issues list carries the detail,
            and SuggestedAlternatives carries any alternative drug names.

            If both LookupDrugEvidence and LookupDailyMedEvidence return "No evidence
            found" for a drug, do NOT assume it's safe. Treat it as insufficient
            evidence: set OverallDecision based on the *rest* of the patient's risk
            factors (allergies, conditions), and explicitly state in Recommendation
            that official evidence was unavailable for this specific drug name.

            Before calling either evidence tool, always call NormalizeDrugName first
            to get the canonical US name — some drug names (e.g. international names)
            won't match US-based sources otherwise.
            """;

        public async Task<PatientSafetyCheckResponse> CheckAsync(
            PatientSafetyCheckRequest request,
            CancellationToken cancellationToken = default)
        {
            using var throttle = new SemaphoreSlim(_settings.MaxConcurrentPatientChecks);

            var tasks = request.Patients.Select(async patient =>
            {
                await throttle.WaitAsync(cancellationToken);
                try
                {
                    return await CheckSinglePatientAsync(patient, cancellationToken);
                }
                finally
                {
                    throttle.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            return new PatientSafetyCheckResponse { Results = results.ToList() };
        }

        public async IAsyncEnumerable<PatientSafetyStreamEvent> CheckStreamAsync(
            PatientSafetyCheckRequest request,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var channel = System.Threading.Channels.Channel.CreateUnbounded<PatientSafetyStreamEvent>();
            using var throttle = new SemaphoreSlim(_settings.MaxConcurrentPatientChecks);

            var producerTask = Task.Run(async () =>
            {
                var tasks = request.Patients.Select(async patient =>
                {
                    await throttle.WaitAsync(cancellationToken);
                    try
                    {
                        await channel.Writer.WriteAsync(new PatientSafetyStreamEvent
                        {
                            PatientRef = patient.Profile.PatientRef,
                            Type = PatientSafetyStreamEventType.Progress,
                            Message = "Checking drug safety..."
                        }, cancellationToken);

                        var result = await CheckSinglePatientAsync(patient, cancellationToken);

                        await channel.Writer.WriteAsync(new PatientSafetyStreamEvent
                        {
                            PatientRef = patient.Profile.PatientRef,
                            Type = PatientSafetyStreamEventType.Result,
                            Result = result
                        }, cancellationToken);
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
                channel.Writer.Complete();
            }, cancellationToken);

            await foreach (var evt in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return evt;
            }

            await producerTask;
        }

        private async Task<PatientSafetyResult> CheckSinglePatientAsync(
            PatientCheckGroup patient,
            CancellationToken cancellationToken)
        {
            const int maxRetries = 2;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await RunAgentForPatientAsync(patient, cancellationToken);
                }
                catch (ClientResultException ex) when (ex.Status == 429 && attempt < maxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt + 1));
                    await Task.Delay(delay, cancellationToken);
                }
                catch (Exception ex)
                {
                    return new PatientSafetyResult
                    {
                        PatientRef = patient.Profile.PatientRef,
                        CheckSucceeded = false,
                        FailureReason = ex.Message
                    };
                }
            }

            return new PatientSafetyResult
            {
                PatientRef = patient.Profile.PatientRef,
                CheckSucceeded = false,
                FailureReason = "Rate limit exceeded after retries."
            };
        }

        private async Task<PatientSafetyResult> RunAgentForPatientAsync(
            PatientCheckGroup patient,
            CancellationToken cancellationToken)
        {
            var client = new OpenAIClient(
                new ApiKeyCredential(_settings.ApiKey),
                new OpenAIClientOptions { Endpoint = new Uri(_settings.Endpoint) });

            var chatClient = client.GetChatClient(_settings.DeploymentName);

            AIAgent agent = chatClient.AsAIAgent(
                instructions: Instructions,
                name: "PatientSafetyAgent",
                tools:
                [
                    AIFunctionFactory.Create(LookupDrugEvidence),
                    AIFunctionFactory.Create(LookupDailyMedEvidence),
                    AIFunctionFactory.Create(NormalizeDrugName)
                ]);

            var drugRefs = patient.DrugsToCheck.Select(d => $"{d.ClientRef}: {d.ScientificName}");
            var prompt = $"""
                Patient allergies: {string.Join(", ", patient.Profile.Allergies)}
                Drugs to check: {string.Join("; ", drugRefs)}
                Assess safety and respond in the required JSON structure.
                """;

            var result = await agent.RunAsync<AgentOutput>(prompt, cancellationToken: cancellationToken);
            var output = result.Result;

            return new PatientSafetyResult
            {
                PatientRef = patient.Profile.PatientRef,
                CheckSucceeded = true,
                OverallDecision = output.OverallDecision,
                RiskScore = output.RiskScore,
                Recommendation = output.Recommendation,
                Issues = output.Issues.Select(i => new SafetyIssueDto
                {
                    Type = i.Type,
                    Severity = i.Severity,
                    Reason = i.Reason,
                    RelatedDrugRefs = i.RelatedDrugRefs
                }).ToList(),
                SuggestedAlternatives = output.SuggestedAlternatives.Select(a => new SuggestedAlternativeDto
                {
                    DrugName = a.DrugName,
                    Reason = a.Reason
                }).ToList(),
                Sources = [_medicalSourceTool.SourceName, _dailyMedTool.SourceName]
            };
        }
    }
}