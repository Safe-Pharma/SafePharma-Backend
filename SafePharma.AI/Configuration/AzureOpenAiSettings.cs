namespace SafePharma.AI.Agent
{
    public class AzureOpenAiSettings
    {
        public required string Endpoint { get; init; }
        public required string ApiKey { get; init; }
        public required string DeploymentName { get; init; }
        public required string EmbeddingDeploymentName { get; init; }

        /// <summary>
        /// Max number of patients checked in parallel within a single "Check All"
        /// request. Based on the Azure OpenAI deployment's rate limits
        /// (currently 250 requests/min, 250k tokens/min) — tune down if the
        /// deployment's limits change or if we see 429s in practice.
        /// </summary>
        public int MaxConcurrentPatientChecks { get; init; } = 5;
    }
}