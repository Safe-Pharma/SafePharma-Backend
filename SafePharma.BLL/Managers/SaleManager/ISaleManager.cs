using SafePharma.Common;

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
    }
}