using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface ISaleManager
    {
        Task<GeneralResult<ReadSaleDto>> CreateSale(CreateSaleDto createSaleDto, Guid pharmacyId, Guid userId);
    }
}