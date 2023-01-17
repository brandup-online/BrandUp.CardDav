using System.Security.Claims;
using System.Security.Principal;

namespace BrandUp.CardDav.Server.Extentions
{
    internal static class IIdentityExtention
    {
        public static Guid GetUserId(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return Guid.Empty;

            return Guid.Parse(claim.Value);
        }
    };
}
