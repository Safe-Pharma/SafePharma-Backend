namespace SafePharma.DAL
{
    public interface IPrimaryContactRepository : IGenircRepository<PrimaryContact>
    {
        Task<bool> EmailExists(string email);
        Task<PrimaryContact?> GetByPharmacyId(Guid pharmacyId);
    }
}