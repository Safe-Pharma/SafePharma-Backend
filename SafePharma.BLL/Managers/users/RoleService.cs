using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles
            .Where(r => r.Name != "Owner")
            .ToListAsync();

        return roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Description = r.Description
            })
            .ToList();
    }
}
