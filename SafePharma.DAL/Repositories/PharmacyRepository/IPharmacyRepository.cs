namespace SafePharma.DAL
{
    public interface IPharmacyRepository : IGenircRepository<Pharmacy>
    {
        Task<bool> BusinessEmailExists(string email);
        Task<bool> TaxNumberExists(string taxNumber);
        Task<bool> CommercialRegistrationExists(string commercialRegistration);
    }
}