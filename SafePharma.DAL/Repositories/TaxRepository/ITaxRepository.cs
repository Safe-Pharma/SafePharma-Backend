namespace SafePharma.DAL
{
    public interface ITaxRepository : IGenircRepository<Tax>
    {
        Task<bool> NameExists(string name, Guid? excludeId = null);
        Task<IEnumerable<Tax>> Search(string? query);
    }
}
