namespace SafePharma.BLL
{
    public interface ITaxManager
    {
        Task<IEnumerable<TaxDto>> GetAllTaxes(Guid pharmacyId, string? search = null);
        Task<TaxDto?> GetTaxById(Guid pharmacyId, Guid id);
        Task<TaxStatsDto> GetStats(Guid pharmacyId);
        Task<TaxCreateResult> CreateTax(Guid pharmacyId, TaxCreateDto dto);
        Task<TaxUpdateResult> UpdateTax(Guid pharmacyId, Guid id, TaxUpdateDto dto);
        Task<bool> DeleteTax(Guid pharmacyId, Guid id);
        Task<TaxDto?> ToggleStatus(Guid pharmacyId, Guid id);
    }

    public class TaxCreateResult
    {
        public TaxDto? Tax { get; set; }
        public bool DuplicateName { get; set; }
    }

    public class TaxUpdateResult
    {
        public TaxDto? Tax { get; set; }
        public bool NotFound { get; set; }
        public bool DuplicateName { get; set; }
    }
}