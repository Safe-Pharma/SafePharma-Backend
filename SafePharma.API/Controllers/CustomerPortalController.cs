using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
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

    [HttpGet("dependents/{childId:guid}")]
    public async Task<IActionResult> GetDependentProfile(Guid childId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.GetMe(childId);  
        return result.Success ? Ok(result) : NotFound(result);
    }


    [HttpGet("medicine-history")]
    public async Task<IActionResult> GetMyMedicineHistory()
    {
        var result = await _customerManager.GetMedicineHistory(_currentUser.Id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("dependents/medicine-history/{childId:guid}")]
    public async Task<IActionResult> GetDependentmedicineHistory(Guid childId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.GetMedicineHistory(childId);
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
    [HttpPut("eiteDependents/{childId:guid}")]
    public async Task<IActionResult> EditChildInfo(CustomerUpdatePortalDto dto, Guid childId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.UpdateCustomerPortal(childId, dto);
       

        return Ok(result);
    }

    [HttpGet("GetMyAllergies")]

    public async Task<IActionResult> GetAllergies()
    {
        var result = await _customerManager.GetAllergies(_currentUser.Id);
        if (result is null) return NotFound();
        return Ok(result);
    }
    [HttpGet("dependents/GetMyAllergies/{childId:guid}")]
    public async Task<IActionResult> GetChildAllergies(Guid childId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.GetAllergies(childId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

     [HttpPost("AddMyAllergies")]
    public async Task<IActionResult> AssignMyAllergy(AssignAllergyDto dto)
    {
        var result = await _customerManager.AssignAllergy(_currentUser.Id, dto.AllergyId);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.ReferenceNotFound) return NotFound(new { message = "Allergy not found." });
        if (result.AlreadyAssigned) return Conflict(new { message = "This allergy is already assigned to the customer." });
        return NoContent();
    }

    [HttpPost("dependents/AddMyAllergies/{childId:guid}")]
    public async Task<IActionResult> AssignChildAllergy(Guid childId, [FromBody] AssignAllergyDto dto)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.AssignAllergy(childId, dto.AllergyId);
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

    [HttpDelete("dependents/allergies/{childId:guid}/{allergyId:guid}")]
    public async Task<IActionResult> RemoveChildAllergy(Guid childId, Guid allergyId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var deleted = await _customerManager.RemoveAllergy(childId, allergyId);
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

    [HttpGet("dependents/chronic-conditions/{childId:guid}")]
    public async Task<IActionResult> GetChildChronicConditions(Guid childId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.GetChronicConditions(childId);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost("chronic-conditions")]
    public async Task<IActionResult> AssignChronicCondition([FromBody] AssignChronicConditionDto dto)
    {
        var result = await _customerManager.AssignChronicCondition(_currentUser.Id, dto.ChronicConditionId);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.ReferenceNotFound) return NotFound(new { message = "Chronic condition not found." });
        if (result.AlreadyAssigned) return Conflict(new { message = "This condition is already assigned to the customer." });
        return NoContent();
    }

    [HttpPost("dependents/chronic-conditions/{childId:guid}")]
    public async Task<IActionResult> AssignChildChronicCondition(Guid childId, [FromBody] AssignChronicConditionDto dto)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.AssignChronicCondition(childId, dto.ChronicConditionId);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.ReferenceNotFound) return NotFound(new { message = "Chronic condition not found." });
        if (result.AlreadyAssigned) return Conflict(new { message = "This condition is already assigned to the customer." });
        return NoContent();
    }

    [HttpDelete("chronic-conditions/{chronicConditionId:guid}")]
    public async Task<IActionResult> RemoveChronicCondition(Guid chronicConditionId)
    {
        var deleted = await _customerManager.RemoveChronicCondition(_currentUser.Id, chronicConditionId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpDelete("dependents/chronic-conditions/{childId:guid}/{chronicConditionId:guid}")]
    public async Task<IActionResult> RemoveChildChronicCondition(Guid childId, Guid chronicConditionId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var deleted = await _customerManager.RemoveChronicCondition(childId, chronicConditionId);
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

    [HttpGet("dependents/organ-functions/{childId:guid}")]
    public async Task<IActionResult> GetChildOrganFunctions(Guid childId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.GetOrganFunctions(childId);
        if (result is null) return NotFound();
        return Ok(result);
    }

    // Also used to UPDATE an existing organ's impairment level — see AssignOrganFunction in the manager.
    [HttpPost("organ-functions")]
    public async Task<IActionResult> AssignOrganFunction([FromBody] AssignOrganFunctionDto dto)
    {
        var result = await _customerManager.AssignOrganFunction(_currentUser.Id, dto);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.OrganNotFound) return NotFound(new { message = "Organ not found." });
        if (result.ImpairmentLevelNotFound) return NotFound(new { message = "Impairment level not found." });
        return Ok(result.OrganFunction);
    }

    // Also used to UPDATE an existing organ's impairment level — see AssignOrganFunction in the manager.
    [HttpPost("dependents/organ-functions/{childId:guid}")]
    public async Task<IActionResult> AssignChildOrganFunction(Guid childId, [FromBody] AssignOrganFunctionDto dto)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _customerManager.AssignOrganFunction(childId, dto);
        if (result.CustomerNotFound) return NotFound(new { message = "Customer not found." });
        if (result.OrganNotFound) return NotFound(new { message = "Organ not found." });
        if (result.ImpairmentLevelNotFound) return NotFound(new { message = "Impairment level not found." });
        return Ok(result.OrganFunction);
    }

    [HttpDelete("organ-functions/{organFunctionId:guid}")]
    public async Task<IActionResult> RemoveOrganFunction(Guid organFunctionId)
    {
        var deleted = await _customerManager.RemoveOrganFunction(_currentUser.Id, organFunctionId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpDelete("dependents/organ-functions/{childId:guid}/{organFunctionId:guid}")]
    public async Task<IActionResult> RemoveChildOrganFunction(Guid childId, Guid organFunctionId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var deleted = await _customerManager.RemoveOrganFunction(childId, organFunctionId);
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

    [HttpGet("dependents/sales/{childId:guid}")]
    public async Task<IActionResult> GetDependentSales(
    Guid childId,
    [FromQuery] string? search,
    [FromQuery] Guid? pharmacyId,
    [FromQuery] SaleStatus? status,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _saleManager.GetCustomerSales(
            childId,
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

    [HttpGet("dependents/sales/{childId:guid}/{saleId:guid}")]
    public async Task<IActionResult> GetDependentSaleById(Guid childId, Guid saleId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var result = await _saleManager.GetCustomerSaleById(saleId, childId);

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

    [HttpGet("dependents/MyRelatives/{childId:guid}")]
    public async Task<ActionResult> GetDependentRelations(Guid childId)
    {
        if (!await _customerRelativesManager.CanAccessAsync(_currentUser.Id, childId))
            return NotFound();

        var res = await _customerRelativesManager.GetRelations(childId);
        return Ok(res);
    }


}