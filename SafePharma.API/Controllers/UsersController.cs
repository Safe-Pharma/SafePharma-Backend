using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// GET /api/users?search=&role=&isActive=&page=&pageSize=&sortBy=&sortDescending=
        /// Backs the Users list page: search bar, role filter, status filter, pagination, sort.
        /// </summary>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<UserListItemDto>>> GetUsers([FromQuery] UserQueryParams query)
        {
            var result = await _userService.GetUsersAsync(query);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result);
        }

        /// <summary>GET /api/users/{id} — backs the user detail page.</summary>
        [HttpGet("{id:guid}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDetailDto>> GetUser(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (result.Success)
                return Ok(result.Data);
            if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(result.Message);
            return BadRequest(result);
        }

        ///// <summary>GET /api/users/{id}/activity — backs the "Recent activity" card separately, if needed.</summary>
        //[HttpGet("{id:guid}/activity")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<IReadOnlyList<UserActivityDto>>> GetUserActivity(Guid id)
        //{
        //    return Ok(await _userService.GetUserActivityAsync(id));
        //}

        /// <summary>POST /api/users — backs the "Create new user" dialog.</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDetailDto>> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            if (result.Success)
                return CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result.Data);
            return BadRequest(result);
        }

        /// <summary>PUT /api/users/{id} — backs the "Edit user" dialog.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDetailDto>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            return Ok(await _userService.UpdateUserAsync(id, request));
        }

        /// <summary>PATCH /api/users/{id}/status — quick Active/Inactive toggle from the row action menu.</summary>
        //[HttpPatch("{id:guid}/status")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> SetUserStatus(Guid id, [FromBody] SetUserStatusRequest request)
        //{
        //    await _userService.SetUserStatusAsync(id);
        //    return NoContent();
        //}

        /// <summary>
        /// DELETE /api/users/{id} — "Delete user" row action.
        /// Does NOT remove the row: sets IsActive = false so audit history
        /// (activity log, past sales, etc.) is preserved.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeactivateUserAsync(id);
            if (result.Success)
                return NoContent();
            if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(result.Message);
            return BadRequest(result);
        }

        ///// <summary>GET /api/users/export?... — the "Export" button, honors current filters.</summary>
        //[HttpGet("export")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> ExportUsers([FromQuery] UserQueryParams query)
        //{
        //    var bytes = await _userService.ExportUsersAsync(query);
        //    return File(bytes, "text/csv", "users.csv");
        //}
    }
}
