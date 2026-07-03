namespace SafePharma.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public PaginationMetaData Metadata { get; set; } = new();
    }
}
