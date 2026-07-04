using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IUserService
    {
        Task<GeneralResult<PagedResult<UserListItemDto>>> GetUsersAsync(UserQueryParams query);

        Task<GeneralResult<UserDetailDto>> GetUserByIdAsync(Guid id);

        Task<GeneralResult<UserDetailDto>> CreateUserAsync(CreateUserRequest request);

        Task<GeneralResult<UserDetailDto>> UpdateUserAsync(Guid id, UpdateUserRequest request);

        ///<summary>Explicit Active/Inactive toggle — used by the row action menu's quick toggle.</summary>
        Task<GeneralResult> SetUserStatusAsync(Guid id, bool isActive);

        /// <summary>
        /// The "Delete user" action. Does NOT remove the row — sets IsActive = false,
        /// same effect as SetUserStatusAsync(id, false), kept as its own method so intent
        /// is unambiguous at the call site (and so you can add delete-specific side effects
        /// later — e.g. revoking sessions — without touching the toggle path).
        /// </summary>
        Task<GeneralResult> DeactivateUserAsync(Guid id);

        Task<GeneralResult<IReadOnlyList<UserActivityDto>>> GetUserActivityAsync(Guid id);

        /// <summary>Returns CSV/XLSX bytes for the "Export" button, honoring current filters.</summary>
        //Task<byte[]> ExportUsersAsync(UserQueryParams query);
    }
}
