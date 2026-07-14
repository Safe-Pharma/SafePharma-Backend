using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IMedicineSearchService
    {
        Task<GeneralResult<PagedResult<MedicineSearchResultDto>>> SearchAsync(Guid pharmacyId, MedicineSearchRequestDto requestDto);
    }
}