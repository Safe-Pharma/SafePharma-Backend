using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface IBatchManager
    {
        Task<GeneralResult<Batch>> CreateBatch(BatchCreateDto batchDto);
        Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllBatches();
    }
}