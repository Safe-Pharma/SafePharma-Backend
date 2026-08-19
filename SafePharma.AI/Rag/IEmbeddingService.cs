namespace SafePharma.AI.Rag
{
    /// <summary>
    /// Converts text into a numeric vector (embedding) that captures its
    /// semantic meaning, for storage in and querying against the vector store.
    /// </summary>
    public interface IEmbeddingService
    {
        Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    }
}