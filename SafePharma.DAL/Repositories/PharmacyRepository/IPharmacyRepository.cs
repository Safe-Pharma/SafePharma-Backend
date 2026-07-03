namespace SafePharma.DAL
{
    public interface IPharmacyRepository : IGenircRepository<Pharmacy>
    {
        Task<bool> BusinessEmailExists(string email);
    }
}