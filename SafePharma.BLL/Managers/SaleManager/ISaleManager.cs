using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface ISaleManager
    {
        Task<GeneralResult<ReadSaleDto>> CreateDraftSale(CreateDraftSaleDto dto, Guid pharmacyId,
        Guid userId);
        Task<GeneralResult<ReadSaleDto>> AddItemToSale(Guid saleId, CreateSaleItemsDto dto, Guid pharmacyId, Guid userId);
        Task<GeneralResult<ReadSaleDto>> UpdateSaleItem(Guid saleId, Guid itemId, UpdateSaleItemDto dto, Guid pharmacyId, Guid userId);
        Task<GeneralResult<ReadSaleDto>> RemoveSaleItem(Guid saleId, Guid itemId, Guid pharmacyId);

        Task<GeneralResult<ReadSaleDto>> ApplyTax(Guid saleId, ApplySaleTaxDto dto, Guid pharmacyId);
        Task<GeneralResult<ReadSaleDto>> ApplyDiscount(Guid saleId, ApplySaleDiscountDto dto, Guid pharmacyId);
        Task<GeneralResult<ReadSaleDto>> Pay(Guid saleId, PaySaleDto dto, Guid pharmacyId, Guid userId);
        Task<GeneralResult<ReadSaleDto>> CancelSale(Guid saleId, Guid pharmacyId);
        Task<GeneralResult<ReadSaleDto>> SetCustomer(Guid saleId, SetSaleCustomerDto dto, Guid pharmacyId);

        Task<GeneralResult<ReadSaleDto>> GetSaleById(Guid saleId, Guid pharmacyId);
        Task<GeneralResult<IEnumerable<ReadSaleDto>>> GetAllSales(Guid pharmacyId, SaleStatus? status = null, string? search = null);
        Task<GeneralResult<SaleStatsDto>> GetStats(Guid pharmacyId);
        Task<GeneralResult<IEnumerable<SalesTrendPointDto>>> GetTrend(Guid pharmacyId, int days = 7);
        Task<GeneralResult<IEnumerable<CategoryMixDto>>> GetCategoryMix(Guid pharmacyId);

        Task<GeneralResult<IEnumerable<ReadSaleDto>>> GetCustomerSales(
    Guid customerId,
    string? search = null,
    Guid? pharmacyId = null,
    SaleStatus? status = null,
    DateTime? from = null,
    DateTime? to = null,
    int page = 1,
    int pageSize = 10);

        Task<GeneralResult<ReadSaleDto>> GetCustomerSaleById(Guid saleId, Guid customerId);



    }
}