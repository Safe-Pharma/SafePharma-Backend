using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private  IBatchManager _batchManager;

        public BatchController(IBatchManager batchManager)
        {
            _batchManager = batchManager;
        }

        [HttpPost]
        public async Task<ActionResult> CreateBatch([FromBody] BatchCreateDto createDto)
        {
            var res = await _batchManager.CreateBatch(createDto);
            return Ok(res);

        }
    }
}
