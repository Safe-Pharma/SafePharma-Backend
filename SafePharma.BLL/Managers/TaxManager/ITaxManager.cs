namespace SafePharma.BLL
{
    public interface ITaxManager
    {
        Task<IEnumerable<TaxDto>> GetAllTaxes(string? search = null);
        Task<TaxDto?> GetTaxById(Guid id);
        Task<TaxStatsDto> GetStats();
        Task<TaxCreateResult> CreateTax(TaxCreateDto dto);
        Task<TaxUpdateResult> UpdateTax(Guid id, TaxUpdateDto dto);
        Task<bool> DeleteTax(Guid id);
        Task<TaxDto?> ToggleStatus(Guid id);
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
