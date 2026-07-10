using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SafePharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BarcodeController : ControllerBase
    {
        private readonly IBarcodeManager _barcodeManager;

        public BarcodeController(IBarcodeManager barcodeManager)
        {
            _barcodeManager = barcodeManager;
        }

       
        [HttpPost("manufacturer")]
        public async Task<IActionResult> AddManufacturerBarcode([FromBody] AddManufacturerBarcodeDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Success = false, Message = "Invalid request body. DTO cannot be null." });
            }
            var result = await _barcodeManager.AddManufacturerBarcodeAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

      
        [HttpPost("pharmacy")]
        public async Task<IActionResult> AddPharmacyBarcode([FromBody] AddPharmacyBarcodeDto dto)
        {
            var result = await _barcodeManager.AddPharmacyBarcodeAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

     
        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody] ScanBarcodeDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Success = false, Message = "Invalid request body. DTO cannot be null." });
            }
            var result = await _barcodeManager.ScanAsync(dto);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}