using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SafePharma.BLL.Managers.Customers
{
    public class CustomerCurrentUserContext : ICustomerCurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerCurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User =>
            _httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("No active HTTP context.");

        public Guid CustomerId
        {
            get
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(claim, out var customerId))
                    return Guid.Empty;

                return customerId;
            }
        }

        public string Phone =>
            User.FindFirstValue("Phone") ?? string.Empty;

        public string Name =>
            User.FindFirstValue("Name") ?? string.Empty;

        public bool IsAuthenticated =>
            User.Identity?.IsAuthenticated ?? false;
    }
}