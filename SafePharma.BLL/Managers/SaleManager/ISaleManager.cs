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
    }
}