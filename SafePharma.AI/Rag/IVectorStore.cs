namespace SafePharma.AI.Rag
{
    /// <summary>
    /// Stores drug evidence text with its embedding, and retrieves the most
    /// semantically similar entries for a given query embedding.
    /// </summary>
    public interface IVectorStore
    {
        Task UpsertAsync(VectorStoreEntry entry, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VectorSearchResult>> SearchSimilarAsync(
            float[] queryEmbedding,
            int topK = 3,
            double minSimilarityScore = 0.7,
            CancellationToken cancellationToken = default);
    }

    public record VectorStoreEntry
    {
        public required string Id { get; init; }          
        public required string DrugName { get; init; }
        public required string Content { get; init; }        
        public required string Source { get; init; }          
        public required float[] Embedding { get; init; }
    }

    public record VectorSearchResult
    {
        public required string DrugName { get; init; }
        public required string Content { get; init; }
        public required string Source { get; init; }
        public required double SimilarityScore { get; init; } 
    }
}