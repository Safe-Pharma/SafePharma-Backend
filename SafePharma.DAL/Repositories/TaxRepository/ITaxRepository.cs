namespace SafePharma.DAL
{
    public interface ITaxRepository : IGenircRepository<Tax>
    {
        Task<bool> NameExists(Guid pharmacyId, string name, Guid? excludeId = null);
        Task<IEnumerable<Tax>> Search(Guid pharmacyId, string? query);
        Task<IEnumerable<Tax>> GetAllForPharmacy(Guid pharmacyId);
    }
}
