using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class BatchController : ControllerBase
    {
        private  IBatchManager _batchManager;

        public BatchController(IBatchManager batchManager)
        {
            _batchManager = batchManager;
        }
        [HttpGet]
        public async Task<ActionResult> GetBatches()
        {
            var res = await _batchManager.GetAllBatches();
            return Ok(res);

        }
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteBatch([FromRoute]Guid id)
        {
            var result = await _batchManager.DeleteBatch(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateBatchQuantity([FromRoute] Guid id, [FromQuery] int newStock)
        {
            var result = await _batchManager.UpdateBatchQuantitiy(id,newStock);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBatch([FromBody] BatchCreateDto createDto)
        {
            var res = await _batchManager.CreateBatch(createDto);
            return Ok(res);

        }
    }
}
