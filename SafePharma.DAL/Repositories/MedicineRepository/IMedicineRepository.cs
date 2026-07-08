namespace SafePharma.DAL
{
    public interface IMedicineRepository : IGenircRepository<Medicine>
    {
        Task<bool> TradeNameExists(string tradeNameEn, Guid? excludeId = null);
        Task<Medicine?> GetByTradeNameEn(string tradeNameEn);
    }
}