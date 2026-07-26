using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.BLL.Authentication;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerPortalController : ControllerBase
{
    private readonly ICustomerManager _customerManager;
    private readonly ICurrentUserContext _currentUser;
    private readonly ISaleManager _saleManager;
    private ICustomerRelativesManager _customerRelativesManager;


    public CustomerPortalController(
        ICustomerManager customerManager,
        ICurrentUserContext currentUser,
        ISaleManager saleManager

        
,
        ICustomerRelativesManager customerRelativesManager

        )
    {
        _customerManager = customerManager;
        _currentUser = currentUser;
        _saleManager = saleManager;
        _customerRelativesManager = customerRelativesManager;
    }

    [HttpGet("getMe")]
    public async Task<IActionResult> GetMyPersonalInfo()
    {
        var result = await _customerManager.GetMe(_currentUser.Id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("medicine-history")]
    public async Task<IActionResult> GetMyMedicineHistory()
    {
        var result = await _customerManager.GetMedicineHistory(_currentUser.Id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("editMe")]
    public async Task<IActionResult> EditMyPersonalInfo(CustomerUpdatePortalDto dto)
    {
        var result = await _customerManager.UpdateCustomerPortal(_currentUser.Id, dto);

        return Ok(result);
    }

    [HttpGet("GetMyAllergies")]

    public async Task<IActionResult> GetAllergies()
    {
        var result = await _customerManager.GetAllergies(_currentUser.Id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost("AddMyAllergies")]
    public async Task<IActionResult> AssignMyAllergy(  AssignAllergyDto dto)
    {
        var result = await _customerManager.AssignAllergy(_currentUser.Id, dto.AllergyId);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.ReferenceNotFound) return NotFound(new { message = "Allergy not found." });
        if (result.AlreadyAssigned) return Conflict(new { message = "This allergy is already assigned to the customer." });
        return NoContent();
    }



    [HttpDelete("allergies/{allergyId:guid}")]
    public async Task<IActionResult> RemoveAllergy(Guid allergyId)
    {
        var deleted = await _customerManager.RemoveAllergy(_currentUser.Id, allergyId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("chronic-conditions")]
    public async Task<IActionResult> GetChronicConditions()
    {
        var result = await _customerManager.GetChronicConditions(_currentUser.Id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost("chronic-conditions")]
    public async Task<IActionResult> AssignChronicCondition( [FromBody] AssignChronicConditionDto dto)
    {
        var result = await _customerManager.AssignChronicCondition(_currentUser.Id, dto.ChronicConditionId);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.ReferenceNotFound) return NotFound(new { message = "Chronic condition not found." });
        if (result.AlreadyAssigned) return Conflict(new { message = "This condition is already assigned to the customer." });
        return NoContent();
    }

    [HttpDelete("chronic-conditions/{chronicConditionId:guid}")]
    public async Task<IActionResult> RemoveChronicCondition( Guid chronicConditionId)
    {
        var deleted = await _customerManager.RemoveChronicCondition(_currentUser.Id, chronicConditionId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ---- Organ functions ----

    [HttpGet("organ-functions")]
    public async Task<IActionResult> GetOrganFunctions()
    {
        var result = await _customerManager.GetOrganFunctions(_currentUser.Id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    // Also used to UPDATE an existing organ's impairment level — see AssignOrganFunction in the manager.
    [HttpPost("organ-functions")]
    public async Task<IActionResult> AssignOrganFunction( [FromBody] AssignOrganFunctionDto dto)
    {
        var result = await _customerManager.AssignOrganFunction(_currentUser.Id, dto);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.OrganNotFound) return NotFound(new { message = "Organ not found." });
        if (result.ImpairmentLevelNotFound) return NotFound(new { message = "Impairment level not found." });
        return Ok(result.OrganFunction);
    }

    [HttpDelete("organ-functions/{organFunctionId:guid}")]
    public async Task<IActionResult> RemoveOrganFunction( Guid organFunctionId)
    {
        var deleted = await _customerManager.RemoveOrganFunction(_currentUser.Id, organFunctionId);
        if (!deleted) return NotFound();
        return NoContent();
    }

   
    [HttpGet("sales")]
    public async Task<IActionResult> GetMySales(
    [FromQuery] string? search,
    [FromQuery] Guid? pharmacyId,
    [FromQuery] SaleStatus? status,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        var result = await _saleManager.GetCustomerSales(
            _currentUser.Id,
            search,
            pharmacyId,
            status,
            from,
            to,
            page,
            pageSize);

        return Ok(result);
    }

    [HttpGet("sales/{saleId:guid}")]
    public async Task<IActionResult> GetMySaleById(Guid saleId)
    {
        var result = await _saleManager.GetCustomerSaleById(saleId, _currentUser.Id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }


    [HttpGet("MyRelatives")]
    public async Task<ActionResult> GetCustomerRelations()

    {
        var res = await _customerRelativesManager.GetRelations(_currentUser.Id);
        return Ok(res);
    }



}