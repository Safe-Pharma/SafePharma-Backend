namespace SafePharma.DAL
{
    public class DuplicateSkuException : Exception
    {
        public DuplicateSkuException(Exception inner)
            : base("A duplicate SKU was rejected by the database.", inner)
        {
        }
    }
}