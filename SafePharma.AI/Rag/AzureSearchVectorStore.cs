using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using FieldBuilder = Azure.Search.Documents.Indexes.FieldBuilder;
using Microsoft.VisualBasic.FileIO;
using System.Reflection.Emit;

namespace SafePharma.AI.Rag
{
    public class AzureSearchVectorStore : IVectorStore
    {
        private readonly SearchClient _searchClient;
        private readonly SearchIndexClient _indexClient;
        private readonly string _indexName;

        public AzureSearchVectorStore(AzureSearchSettings settings)
        {
            var credential = new AzureKeyCredential(settings.ApiKey);
            _indexName = settings.IndexName;

            _indexClient = new SearchIndexClient(new Uri(settings.Endpoint), credential);
            _searchClient = new SearchClient(new Uri(settings.Endpoint), _indexName, credential);
        }

        public async Task EnsureIndexExistsAsync(CancellationToken cancellationToken = default)
        {
            var fields = new FieldBuilder().Build(typeof(DrugEvidenceDocument));

            var vectorSearch = new VectorSearch
            {
                Profiles = { new VectorSearchProfile("default-profile", "default-algorithm") },
                Algorithms = { new HnswAlgorithmConfiguration("default-algorithm") }
            };

            var index = new SearchIndex(_indexName, fields) { VectorSearch = vectorSearch };

            await _indexClient.CreateOrUpdateIndexAsync(index, cancellationToken: cancellationToken);
        }

        public async Task UpsertAsync(VectorStoreEntry entry, CancellationToken cancellationToken = default)
        {
            var doc = new DrugEvidenceDocument
            {
                Id = entry.Id,
                DrugName = entry.DrugName,
                Content = entry.Content,
                Source = entry.Source,
                ContentVector = entry.Embedding
            };

            await _searchClient.MergeOrUploadDocumentsAsync(new[] { doc }, cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<VectorSearchResult>> SearchSimilarAsync(
    float[] queryEmbedding,
    int topK = 3,
    double minSimilarityScore = 0.1,
    CancellationToken cancellationToken = default)
        {
            var searchOptions = new SearchOptions
            {
                VectorSearch = new()
                {
                    Queries = { new VectorizedQuery(queryEmbedding) { KNearestNeighborsCount = topK, Fields = { "ContentVector" } } }
                },
                Size = topK
            };

            var response = await _searchClient.SearchAsync<DrugEvidenceDocument>(null, searchOptions, cancellationToken);

            var results = new List<VectorSearchResult>();
            await foreach (var result in response.Value.GetResultsAsync())
            {
                var score = result.Score ?? 0;

                if (score < minSimilarityScore) continue;

                results.Add(new VectorSearchResult
                {
                    DrugName = result.Document.DrugName,
                    Content = result.Document.Content,
                    Source = result.Document.Source,
                    SimilarityScore = score
                });
            }

            return results;
        }

    }
    
    public class DrugEvidenceDocument
    {
        [SimpleField(IsKey = true)]
        public string Id { get; set; } = "";

        [SearchableField]
        public string DrugName { get; set; } = "";

        [SearchableField]
        public string Content { get; set; } = "";

        [SimpleField(IsFilterable = true)]
        public string Source { get; set; } = "";

        [VectorSearchField(VectorSearchDimensions = 1536, VectorSearchProfileName = "default-profile")]
        public float[] ContentVector { get; set; } = [];
    }
}