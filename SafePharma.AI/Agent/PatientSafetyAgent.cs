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

            The patient profile given to you is the ONLY source of truth about this
            patient. Never assert, imply, or assume that the patient has an allergy,
            chronic condition, organ impairment, or current medication that is not
            explicitly listed in the profile — even if a drug's official evidence
            mentions that condition/allergy in general (e.g. a label warning about
            "patients with penicillin allergy"). A general warning in the evidence is
            about the drug; it only becomes a "Drug-Allergy", "Drug-Disease", or
            "Organ Function" issue for THIS patient if their profile explicitly lists
            the matching allergy/condition/impairment. If the profile does not list it,
            classify the same evidence as "General Warning" instead, and do not
            describe it in Reason as something this patient has or has been diagnosed
            with — describe it as a precaution relevant to the general population that
            could not be confirmed against this patient's on-file profile.

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
            "Drugs to check"), you MUST explicitly check every pairwise combination
            for Drug-Drug interactions — not just each drug individually against the
            patient's allergies/conditions. Look up evidence for each drug
            separately, then reason about how they interact with each other. Report
            each interaction found as its own Issue with Type "Drug-Drug" and
            RelatedDrugRefs containing BOTH drugs involved. Only consider drugs
            listed in "Drugs to check" for this — do not factor in any medication
            history, since the patient profile you're given deliberately does not
            include one.

            When classifying an Issue's Type, actively cross-check each piece of evidence
            against the patient's specific profile before defaulting to "General Warning":
            - If the evidence mentions an allergy/hypersensitivity and the patient has a
              matching or related allergy, use "Drug-Allergy" (not General Warning).
            - If the evidence mentions organ-related toxicity (liver/kidney/heart) and the
              patient has a matching organ impairment, use "Organ Function".
            - If the evidence mentions interaction with a drug class and "Drugs to
              check" contains a drug from that class, use "Drug-Drug".
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

            The rule above is about missing DRUG evidence — it is a completely
            separate situation from a patient having no risk factors on file. When the
            patient's profile shows no allergies, no chronic conditions, no organ
            impairments, no current medications, and no drug-drug interaction is found
            among the drugs being checked, AND official evidence WAS found for the
            drug(s) (from either source) with nothing beyond routine, non-patient-specific
            precautions (e.g. storage instructions, "keep out of reach of children",
            generic label boilerplate), then set OverallDecision to "Approve", use a
            low RiskScore, and state plainly in Recommendation that the medicine is
            safe to dispense based on the information available — do not hedge or
            recommend "verify" in this case just because the patient's history is short.
            Only fall back to caution here if the evidence itself could not be
            retrieved at all (see the paragraph above).

            Keep every Issue's Reason concise — 1 to 2 short sentences, plain
            language a pharmacist can read in a few seconds. Do not write
            multi-sentence clinical essays explaining what evidence was or wasn't
            found; just state the finding and why it matters for this dispensing
            decision.

            Do not create an Issue at all for evidence that is purely generic,
            non-patient-specific boilerplate with no bearing on this dispensing
            decision (e.g. "keep out of reach of children", storage/handling
            instructions, flammability warnings for topical use, or other routine
            label text that doesn't reflect an actual risk for this patient or this
            drug's use here). Omit these from the Issues list entirely rather than
            reporting them as a "General Warning" — a low-value warning clutters the
            pharmacist's view and erodes trust in the real warnings. Only include an
            Issue when it reflects either a genuine patient-specific match (per the
            rules above) or a clinically significant general warning about the drug
            (e.g. a boxed warning, a serious contraindication) that a pharmacist
            would actually want to know about regardless of this specific patient's
            profile. When nothing meets that bar, Issues should simply be an empty
            list — an empty list is a normal, good outcome, not something to fill.

            For a general (non-patient-specific) warning that describes a risk from
            MISUSE or exceeding the recommended dose (e.g. a maximum daily dose
            caution, an overdose threshold, "avoid combining with other products
            containing the same ingredient") — this is routine safety counseling that
            applies to essentially every sale of that drug, not a reason to escalate
            risk on its own. Cap this kind of warning at Medium risk (31-65) with
            OverallDecision "Warn" (dispense with counseling), UNLESS the patient's
            own profile explicitly shows a factor that makes misuse more likely or
            more dangerous for THIS patient specifically (e.g. a documented liver
            condition, another product containing the same active ingredient already
            in "Drugs to check", or documented heavy alcohol use in the profile) —
            only then escalate toward High risk ("Block").

            Reserve High risk / "Block" for: a real patient-specific contraindication
            (a match against the profile per the classification rules above), or a
            rare, severe, dose-independent risk described as a boxed warning or
            serious contraindication in the evidence. Do not use "Block" for routine
            dosing precautions that apply to normal, appropriately-dosed use of an
            over-the-counter medicine — those belong at "Warn", not "Block".

            Do not add "verify [condition/history] before proceeding" caveats about
            anything that is not in the patient profile you were given. The profile
            is the only source of truth for this patient (see the rule above) — if a
            condition isn't listed there, you have no grounds to ask the pharmacist to
            go re-check it; simply don't mention it.

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
                    return await CheckSinglePatientAsync(patient, request.Language, cancellationToken);
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

                        var result = await CheckSinglePatientAsync(patient, request.Language, cancellationToken);

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
            string language,
            CancellationToken cancellationToken)
        {
            const int maxRetries = 2;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await RunAgentForPatientAsync(patient, language, cancellationToken);
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
            string language,
            CancellationToken cancellationToken)
        {
            var client = new OpenAIClient(
                new ApiKeyCredential(_settings.ApiKey),
                new OpenAIClientOptions { Endpoint = new Uri(_settings.Endpoint) });

            var chatClient = client.GetChatClient(_settings.DeploymentName);

            AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = "PatientSafetyAgent",
                ChatOptions = new ChatOptions
                {
                    Instructions = Instructions,
                    Tools =
                    [
                        AIFunctionFactory.Create(LookupDrugEvidence),
                        AIFunctionFactory.Create(LookupDailyMedEvidence),
                        AIFunctionFactory.Create(NormalizeDrugName)
                    ]
                    // NOTE: Temperature is intentionally NOT set here. This deployment
                    // is a reasoning-family model (GPT-5 mini) which only supports the
                    // default temperature (1) and rejects any other value with a 400
                    // ("Unsupported value: 'temperature' does not support 0 with this
                    // model"). Consistency across repeated checks has to come from the
                    // Instructions being explicit enough to remove ambiguity (see the
                    // risk-tier rules above), not from a temperature setting.
                }
            });

            var drugRefs = patient.DrugsToCheck.Select(d => $"{d.ClientRef}: {d.ScientificName}");

            // Every list is rendered explicitly as "None known"/"None reported" when
            // empty — never left blank — so the model can't read an empty section as
            // "unknown/unconfirmed" and fill the gap with an assumption instead of a
            // fact from this patient's actual profile.
            var allergiesText = patient.Profile.Allergies.Count > 0
                ? string.Join(", ", patient.Profile.Allergies)
                : "None known";
            var conditionsText = patient.Profile.ChronicConditions.Count > 0
                ? string.Join(", ", patient.Profile.ChronicConditions)
                : "None known";
            var organText = patient.Profile.OrganImpairments.Count > 0
                ? string.Join(", ", patient.Profile.OrganImpairments.Select(o => $"{o.OrganName}: {o.ImpairmentLevel}"))
                : "None known";
            var pregnancyText = patient.Profile.IsPregnant
                ? $"Pregnant ({patient.Profile.PregnancyTrimester ?? "trimester unspecified"})"
                : "Not pregnant";
            var lactationText = patient.Profile.IsLactating ? "Lactating" : "Not lactating";

            // Only the free-text fields (Reason, Recommendation) switch language —
            // Type/Severity/OverallDecision must stay as the fixed English codes
            // from the Instructions, since BLL/the client localize those by code,
            // not by re-translating arbitrary text.
            var languageInstruction = language == "ar"
                ? "Arabic (Modern Standard Arabic, natural for a pharmacist to read)"
                : "English";

            var prompt = $"""
                Patient profile (this is the ONLY source of truth about this patient —
                do not assume or infer any allergy, condition, medication, or history
                beyond what is explicitly listed below). Medication history is
                deliberately not included here — base the check only on allergies,
                chronic conditions, and organ impairments, plus interactions among
                the drugs listed in "Drugs to check" below:
                Age: {patient.Profile.Age?.ToString() ?? "Unknown"}
                Gender: {patient.Profile.Gender ?? "Unknown"}
                Allergies: {allergiesText}
                Chronic conditions: {conditionsText}
                Organ impairments: {organText}
                Pregnancy status: {pregnancyText}
                Lactation status: {lactationText}

                Drugs to check: {string.Join("; ", drugRefs)}

                Write the free-text fields — SafetyIssueDto.Reason and
                PatientSafetyResult.Recommendation — in {languageInstruction}. Keep
                Type, Severity, and OverallDecision as the fixed English codes defined
                in your instructions regardless of this language setting — do not
                translate those.

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