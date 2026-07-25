using Microsoft.AspNetCore.Http;
using SafePharma.Common;
using System.Security.Claims;

namespace SafePharma.BLL.Authentication;

public class HttpCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No active HTTP context.");

    public Guid Id
    {
        get
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(claim, out var id)
                ? id
                : Guid.Empty;
        }
    }

    public string Name =>
        User.FindFirstValue("Name") ?? string.Empty;

    public string Phone =>
        User.FindFirstValue("Phone") ?? string.Empty;

    public Guid PharmacyId
    {
        get
        {
            var claim = User.FindFirstValue("PharmacyId");

            if (Guid.TryParse(claim, out var pharmacyId))
                return pharmacyId;

            return Guid.Empty;
        }
    }

    public bool IsCustomer =>
        User.FindFirstValue("EntityType") == "Customer";

    public bool IsStaff =>
        User.FindFirstValue("EntityType") == "Staff";

    public IReadOnlyList<string> Roles =>
        User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
}