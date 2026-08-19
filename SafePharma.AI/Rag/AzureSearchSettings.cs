namespace SafePharma.AI.Rag
{
    public class AzureSearchSettings
    {
        public required string Endpoint { get; init; }
        public required string ApiKey { get; init; }
        public string IndexName { get; init; } = "drug-evidence-index";
    }
}