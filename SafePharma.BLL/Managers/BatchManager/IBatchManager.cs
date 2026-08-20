using SafePharma.BLL;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface IBatchManager
    {
        Task<GeneralResult<Batch>> CreateBatch(BatchCreateDto batchDto);
        Task<GeneralResult<IEnumerable<BatchReadDto>>> GetAllBatches();
        Task<GeneralResult> DeleteBatch(Guid id);
        Task<GeneralResult> UpdateBatchQuantity(BatchQtyDto dto);
    }
}
