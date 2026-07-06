namespace SafePharma.BLL
{
    public interface ISupplierManager
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliers(Guid pharmacyId, string? search = null);
        Task<SupplierDto?> GetSupplierById(Guid pharmacyId, Guid id);
        Task<SupplierStatsDto> GetStats(Guid pharmacyId);
        Task<SupplierCreateResult> CreateSupplier(Guid pharmacyId, SupplierCreateDto dto);
        Task<SupplierUpdateResult> UpdateSupplier(Guid pharmacyId, Guid id, SupplierUpdateDto dto);
        Task<bool> DeleteSupplier(Guid pharmacyId, Guid id);
        Task<SupplierDto?> ToggleStatus(Guid pharmacyId, Guid id);
    }

    public class SupplierCreateResult
    {
        public SupplierDto? Supplier { get; set; }
        public bool DuplicateName { get; set; }
    }

    public class SupplierUpdateResult
    {
        public SupplierDto? Supplier { get; set; }
        public bool NotFound { get; set; }
        public bool DuplicateName { get; set; }
    }
}
