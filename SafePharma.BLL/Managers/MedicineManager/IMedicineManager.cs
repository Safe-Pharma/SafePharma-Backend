using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface IMedicineManager
    {
        // Pharmacy-scoped (admin/staff)
        Task<IEnumerable<MedicineDto>> GetAllMedicines(Guid pharmacyId, string? search = null, string? category = null, bool includeInactive = false);
        Task<MedicineDto?> GetMedicineById(Guid pharmacyId, Guid id);
        Task<MedicineStatsDto> GetStats(Guid pharmacyId);
        Task<IEnumerable<GlobalMedicineSearchResultDto>> SearchGlobalCatalog(Guid pharmacyId, string? query);
        Task<LinkExistingResult> LinkExistingMedicine(Guid pharmacyId, LinkExistingMedicineDto dto);
        Task<MedicineCreateResult> CreateLocalMedicine(Guid pharmacyId, MedicineCreateDto dto);
        Task<MedicineUpdateResult> UpdatePharmacyMedicine(Guid pharmacyId, Guid id, PharmacyMedicineUpdateDto dto);
        Task<bool> DeleteMedicine(Guid pharmacyId, Guid id);
        Task<MedicineDto?> ToggleStatus(Guid pharmacyId, Guid id);
        Task<MedicineDetailsDto?> GetMedicineDetails(Guid pharmacyId, Guid id);

        // Global (owner) — no pharmacyId involved anywhere
        Task<GlobalMedicineCreateResult> CreateGlobalMedicine(GlobalMedicineCreateDto dto);
        Task<GlobalMedicineUpdateResult> UpdateGlobalMedicine(Guid id, GlobalMedicineUpdateDto dto);
        Task<Medicine?> ToggleGlobalStatus(Guid id);
    }

    public class MedicineCreateResult
    {
        public MedicineDto? Medicine { get; set; }
        public bool ExistingMedicineFound { get; set; }
        public Guid? ExistingMedicineId { get; set; }
        public bool DuplicateTradeNameInPharmacy { get; set; }
        public bool InvalidTaxIds { get; set; }
        public bool DuplicateSku { get; set; }
    }

    public class GlobalMedicineCreateResult
    {
        public GlobalMedicineDto? Medicine { get; set; }
        public bool ExistingMedicineFound { get; set; }
        public Guid? ExistingMedicineId { get; set; }
    }

    public class LinkExistingResult
    {
        public MedicineDto? Medicine { get; set; }
        public bool MedicineNotFound { get; set; }
        public bool AlreadyLinked { get; set; }
        public bool InvalidTaxIds { get; set; }
        public bool DuplicateSku { get; set; }
    }

    public class MedicineUpdateResult
    {
        public MedicineDto? Medicine { get; set; }
        public bool NotFound { get; set; }
        public bool InvalidTaxIds { get; set; }
        public bool DuplicateSku { get; set; }
    }

    public class GlobalMedicineUpdateResult
    {
        public bool NotFound { get; set; }
        public bool DuplicateTradeName { get; set; }
        public Medicine? Medicine { get; set; }
    }
}