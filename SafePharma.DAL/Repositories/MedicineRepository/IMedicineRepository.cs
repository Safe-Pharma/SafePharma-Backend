namespace SafePharma.DAL
{
    public interface IMedicineRepository : IGenircRepository<Medicine>
    {
        Task<bool> TradeNameExists(string tradeNameEn, Guid? excludeId = null);
        Task<Medicine?> GetByTradeNameEn(string tradeNameEn);
        Task<IEnumerable<Medicine>> SearchGlobal(string? query);
    }
}