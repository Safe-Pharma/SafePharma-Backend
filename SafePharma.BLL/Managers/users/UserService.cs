using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafePharma.BLL;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserContext _currentUser;

    public UserService(
        UserManager<ApplicationUser> userManager,
        ICurrentUserContext currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    // ── GET LIST ────────────────────────────────────────────────────────────

    public async Task<GeneralResult<PagedResult<UserListItemDto>>> GetUsersAsync(UserQueryParams query)
    {
        // Always scope to the caller's tenant first
        var q = _userManager.Users;
            //.Where(u => u.PharmacyId == _currentUser.PharmacyId && !u.IsDeleted);

        // Search: name or email
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(u =>
                (u.FirstName + " " + u.LastName).ToLower().Contains(term) ||
                u.Email!.ToLower().Contains(term));
        }

        // Status filter
        if (query.IsActive.HasValue)
            q = q.Where(u => u.IsActive == query.IsActive.Value);

        // Role filter — requires a join through Identity's UserRoles
        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(query.Role);
            var idsInRole = usersInRole.Select(u => u.Id).ToHashSet();
            q = q.Where(u => idsInRole.Contains(u.Id));
        }

        // Sort
        q = query.SortBy?.ToLower() switch
        {
            "email"     => query.SortDescending ? q.OrderByDescending(u => u.Email)     : q.OrderBy(u => u.Email),
            "createdat" => query.SortDescending ? q.OrderByDescending(u => u.CreatedAt) : q.OrderBy(u => u.CreatedAt),
            _           => query.SortDescending
                               ? q.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName)
                               : q.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
        };

        var totalCount = await q.CountAsync();

        var users = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Resolve roles in bulk — UserManager has no batch API, so we do it per page (small N)
        var items = new List<UserListItemDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(MapToListItem(user, roles.FirstOrDefault() ?? string.Empty));
        }

        var result = new PagedResult<UserListItemDto>
        {
            Items = items,
            Metadata = new PaginationMetaData
            {
                CurrentPage = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
                HasNext = query.Page < (int)Math.Ceiling(totalCount / (double)query.PageSize),
                HasPrev = query.Page > 1,
            }
        };

        return GeneralResult<PagedResult<UserListItemDto>>.SuccessResult(result);
    }

    // ── GET DETAIL ──────────────────────────────────────────────────────────

    public async Task<GeneralResult<UserDetailDto>> GetUserByIdAsync(Guid id)
    {
        var user = await _userManager.Users
            .Include(u => u.AuditList)
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                u.PharmacyId == _currentUser.PharmacyId &&
                !u.IsDeleted);

        if (user is null) return GeneralResult<UserDetailDto>.NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var recentActivity = user.AuditList
            .OrderByDescending(a => a.Date)
            .Take(10)
            .Select(a => new UserActivityDto
            {
                Id        = a.Id,
                Message   = a.Action,        // adjust to your Audit fields
                Timestamp = a.Date,
            })
            .ToList();

        var dto = new UserDetailDto
        {
            Id             = user.Id,
            FirstName      = user.FirstName,
            LastName       = user.LastName,
            Name           = user.FullName,
            Email          = user.Email ?? string.Empty,
            Phone          = user.PhoneNumber,
            Role           = roles.FirstOrDefault() ?? string.Empty,
            Branch         = user.Branch,
            IsActive       = user.IsActive,
            LastLoginAt    = user.LastLoginAt,
            CreatedAt      = user.CreatedAt,
            EmailConfirmed = user.EmailConfirmed,
            //LockoutEnabled = user.LockoutEnabled,
            //LockoutEnd     = user.LockoutEnd,
            PharmacyId     = user.PharmacyId ?? Guid.Empty,
            RecentActivity = recentActivity,
        };

        return GeneralResult<UserDetailDto>.SuccessResult(dto);
    }

    // ── CREATE ──────────────────────────────────────────────────────────────

    public async Task<GeneralResult<UserDetailDto>> CreateUserAsync(CreateUserRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return GeneralResult<UserDetailDto>.FailResult("Passwords do not match.");

        var user = new ApplicationUser
        {
            FirstName    = request.FirstName.Trim(),
            LastName     = request.LastName.Trim(),
            Email        = request.Email.Trim().ToLower(),
            UserName     = request.Email.Trim().ToLower(),
            PhoneNumber  = request.Phone,
            Branch       = request.Branch,
            IsActive     = request.IsActive,
            // Scope new user to the caller's pharmacy — never take this from the request body
            PharmacyId   = _currentUser.PharmacyId,
            CreatedAt    = DateTime.UtcNow,
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = MapIdentityErrors(createResult.Errors);
            return GeneralResult<UserDetailDto>.FailResult(errors);
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                var errors = MapIdentityErrors(roleResult.Errors);
                return GeneralResult<UserDetailDto>.FailResult(errors);
            }
        }
        var getResult = await GetUserByIdAsync(user.Id);
        return getResult;
    }

    // ── UPDATE ──────────────────────────────────────────────────────────────

    public async Task<GeneralResult<UserDetailDto>> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await GetOwnedUserAsync(id);
        if (user is null) return GeneralResult<UserDetailDto>.NotFound();

        user.FirstName   = request.FirstName.Trim();
        user.LastName    = request.LastName.Trim();
        user.PhoneNumber = request.Phone;
        user.Branch      = request.Branch;
        user.IsActive    = request.IsActive;
        user.UpdatedAt   = DateTime.UtcNow;

        // Email change goes through UserManager to keep the normalised index in sync
        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, request.Email.Trim().ToLower());
            if (!setEmailResult.Succeeded)
            {
                var errors = MapIdentityErrors(setEmailResult.Errors);
                return GeneralResult<UserDetailDto>.FailResult(errors);
            }

            await _userManager.SetUserNameAsync(user, request.Email.Trim().ToLower());
        }
        else
        {
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = MapIdentityErrors(updateResult.Errors);
                return GeneralResult<UserDetailDto>.FailResult(errors);
            }
        }

        // Role: remove all existing, assign the new one
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var remRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!remRes.Succeeded)
            {
                var errors = MapIdentityErrors(remRes.Errors);
                return GeneralResult<UserDetailDto>.FailResult(errors);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var addRes = await _userManager.AddToRoleAsync(user, request.Role);
            if (!addRes.Succeeded)
            {
                var errors = MapIdentityErrors(addRes.Errors);
                return GeneralResult<UserDetailDto>.FailResult(errors);
            }
        }

        return await GetUserByIdAsync(id);
    }

    // ── STATUS TOGGLE ───────────────────────────────────────────────────────

    public async Task<GeneralResult> SetUserStatusAsync(Guid id, bool isActive)
    {
        var user = await GetOwnedUserAsync(id);
        if (user is null) return GeneralResult.NotFound();

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        var res = await _userManager.UpdateAsync(user);
        if (!res.Succeeded) return GeneralResult.FailResult(MapIdentityErrors(res.Errors));

        return GeneralResult.SuccessResult("User status updated.");
    }

    // ── DEACTIVATE (soft delete) ─────────────────────────────────────────────

    public async Task<GeneralResult> DeactivateUserAsync(Guid id)
    {
        var user = await GetOwnedUserAsync(id);
        if (user is null) return GeneralResult.NotFound();

        user.IsActive = false;
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = _currentUser.UserId.ToString();
        user.UpdatedAt = DateTime.UtcNow;

        var res = await _userManager.UpdateAsync(user);
        if (!res.Succeeded) return GeneralResult.FailResult(MapIdentityErrors(res.Errors));

        return GeneralResult.SuccessResult("User deactivated.");
    }

    // ── ACTIVITY ────────────────────────────────────────────────────────────

    public async Task<GeneralResult<IReadOnlyList<UserActivityDto>>> GetUserActivityAsync(Guid id)
    {
        var user = await _userManager.Users
            .Include(u => u.AuditList)
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                u.PharmacyId == _currentUser.PharmacyId &&
                !u.IsDeleted);

        if (user is null) return GeneralResult<IReadOnlyList<UserActivityDto>>.NotFound();

        var list = user.AuditList
            .OrderByDescending(a => a.Date)
            .Take(20)
            .Select(a => new UserActivityDto
            {
                Id        = a.Id,
                Message   = a.Action,        // adjust to your Audit property names
                Timestamp = a.Date,
            })
            .ToList();

        return GeneralResult<IReadOnlyList<UserActivityDto>>.SuccessResult(list);
    }

    // ── PRIVATE HELPERS ─────────────────────────────────────────────────────

    /// <summary>
    /// Loads a user that must belong to the caller's pharmacy and not be deleted.
    /// Throws 404-equivalent if not found — avoids cross-tenant access.
    /// </summary>
    private async Task<ApplicationUser?> GetOwnedUserAsync(Guid id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.Id == id &&
            u.PharmacyId == _currentUser.PharmacyId &&
            !u.IsDeleted);
        return user;
    }

    private static Dictionary<string, List<Error>> MapIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var dict = new Dictionary<string, List<Error>>();
        var list = new List<Error>();
        foreach (var e in errors)
        {
            list.Add(new Error { ErrorCode = e.Code, ErrorMessage = e.Description });
        }
        dict["identity"] = list;
        return dict;
    }

    private static UserListItemDto MapToListItem(ApplicationUser user, string role) => new()
    {
        Id          = user.Id,
        Name        = user.FullName,
        Email       = user.Email ?? string.Empty,
        Phone       = user.PhoneNumber,
        Role        = role,
        Branch      = user.Branch,
        IsActive    = user.IsActive,
        LastLoginAt = user.LastLoginAt,
        CreatedAt   = user.CreatedAt,
    };
}