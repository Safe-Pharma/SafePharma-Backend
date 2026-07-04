namespace SafePharma.DAL
{
    public interface ICountryRepository : IGenircRepository<Country>
    {
        Task<List<Country>> GetAllWithCitiesAsync();
        Task<Country?> GetByNameAsync(string name);
    }
}