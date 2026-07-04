using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class CountryRepository : GenircRepository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Country>> GetAllWithCitiesAsync()
        {
            return await _db.Countries
                .Include(c => c.Cities)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Country?> GetByNameAsync(string name)
        {
            return await _db.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}