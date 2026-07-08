namespace SafePharma.BLL
{
    public interface IMedicineManager
    {
        Task<IEnumerable<MedicineDto>> GetAllMedicines(Guid pharmacyId, string? search = null, string? category = null);
        Task<MedicineDto?> GetMedicineById(Guid pharmacyId, Guid id);
        Task<MedicineStatsDto> GetStats(Guid pharmacyId);
        Task<MedicineCreateResult> CreateMedicine(Guid pharmacyId, MedicineCreateDto dto);
        Task<MedicineUpdateResult> UpdateMedicine(Guid pharmacyId, Guid id, MedicineUpdateDto dto);
        Task<bool> DeleteMedicine(Guid pharmacyId, Guid id);
        Task<MedicineDto?> ToggleStatus(Guid pharmacyId, Guid id);
    }

    public class MedicineCreateResult
    {
        public MedicineDto? Medicine { get; set; }
        public bool DuplicateTradeName { get; set; }
    }

    public class MedicineUpdateResult
    {
        public MedicineDto? Medicine { get; set; }
        public bool NotFound { get; set; }
        public bool DuplicateTradeName { get; set; }
    }
}