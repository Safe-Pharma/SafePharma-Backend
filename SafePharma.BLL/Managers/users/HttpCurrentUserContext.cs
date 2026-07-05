using Microsoft.AspNetCore.Http;
using SafePharma.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SafePharma.BLL.Managers.users
{
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

        public Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// This is the pharmacyId claim you put in the JWT at login
        /// </summary>
        public Guid PharmacyId
        {
            get
            {
                var claim = User.FindFirstValue("PharmacyId");

                if (!Guid.TryParse(claim, out var pharmacyId))
                    return Guid.Empty;

                return pharmacyId;
            }
        }

        public IReadOnlyList<string> Roles =>
            User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }
}
