using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IValidator<CreateUserRequest> _createValidator;
        private readonly IValidator<UpdateUserRequest> _updateValidator;

        public UsersController(
            IUserService userService,
            IRoleService roleService,
            IValidator<CreateUserRequest> createValidator,
            IValidator<UpdateUserRequest> updateValidator
            )
        {
            _userService = userService;
            _roleService = roleService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetRoles()
        {
            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<UserListItemDto>>> GetUsers(
            [FromQuery] UserQueryParams query)
        {
            var result = await _userService.GetUsersAsync(query);

            if (result.Success)
                return Ok(result.Data);

            return BadRequest(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDetailDto>> GetUser(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (result.Success)
                return Ok(result.Data);

            if (result.Message?.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        [HttpPost]
        public async Task<ActionResult<UserDetailDto>> CreateUser(
            [FromBody] CreateUserRequest request)
        {

            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );
                return BadRequest(GeneralResult<UserDetailDto>.FailResult(errors));
            }
            var result = await _userService.CreateUserAsync(request);

            if (result.Success)
            {
                return CreatedAtAction(
                    nameof(GetUser),
                    new { id = result.Data!.Id },
                    result.Data);
            }

            return BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserDetailDto>> UpdateUser(
            Guid id,
            [FromBody] UpdateUserRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );
                return BadRequest(GeneralResult<UserDetailDto>.FailResult(errors));
            }
            var result = await _userService.UpdateUserAsync(id, request);

            if (result.Success)
                return Ok(result.Data);

            if (result.Message?.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> SetUserStatus(
            Guid id,
            [FromBody] SetuserStatusRequest request)
        {
            var result = await _userService.SetUserStatusAsync(
                id,
                request.IsActive);

            if (result.Success)
                return NoContent();

            if (result.Message?.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeactivateUserAsync(id);

            if (result.Success)
                return NoContent();

            if (result.Message?.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }
    }
}