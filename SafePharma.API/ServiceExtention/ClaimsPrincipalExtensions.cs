using System.Security.Claims;

namespace SafePharma.API
{
    public static class ClaimsPrincipalExtensions
    {
        
        public static Guid GetPharmacyId(this ClaimsPrincipal user)
        {
            var raw = user.FindFirst("PharmacyId")?.Value;

            if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var pharmacyId))
            {
                throw new UnauthorizedAccessException("Token does not contain a valid PharmacyId claim.");
            }

            return pharmacyId;
        }
    }
}