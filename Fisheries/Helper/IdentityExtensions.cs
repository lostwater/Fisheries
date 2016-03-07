
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.AspNet.Identity
{
    public static class IdentityExtensions
    {
        public static String GetHomePhone(this IIdentity identity)
        {
            if (identity == null)
                return null;

            return (identity as ClaimsIdentity).FirstOrNull(ClaimTypes.HomePhone);
        }

        internal static string FirstOrNull(this ClaimsIdentity identity, string claimType)
        {
            var val = identity.FindFirst(claimType);

            return val == null ? null : val.Value;
        }
    }

}