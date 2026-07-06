namespace SafePharma.DAL
{
    public interface ISupplierRepository : IGenircRepository<Supplier>
    {
        Task<bool> NameExists(Guid pharmacyId, string name, Guid? excludeId = null);
        Task<IEnumerable<Supplier>> Search(Guid pharmacyId, string? query);
        Task<IEnumerable<Supplier>> GetAllForPharmacy(Guid pharmacyId);
        Task<Supplier?> GetByIdWithCountry(Guid id);
    }
}
