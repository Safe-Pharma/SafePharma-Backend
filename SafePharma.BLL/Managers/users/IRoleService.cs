using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IRoleService
    {
        Task<IReadOnlyList<RoleDto>> GetRolesAsync();
    }
}
