using OpenAI.Embeddings;
using System.ClientModel;
using OpenAI;
using SafePharma.AI.Agent;

namespace SafePharma.AI.Rag
{
    public class AzureOpenAiEmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;

        public AzureOpenAiEmbeddingService(AzureOpenAiSettings settings)
        {
            var client = new OpenAIClient(
                new ApiKeyCredential(settings.ApiKey),
                new OpenAIClientOptions { Endpoint = new Uri(settings.Endpoint) });

            _embeddingClient = client.GetEmbeddingClient(settings.EmbeddingDeploymentName);
        }

        public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            var result = await _embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
            return result.Value.ToFloats().ToArray();
        }
    }
}