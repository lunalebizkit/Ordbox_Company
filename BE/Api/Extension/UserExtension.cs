using Newtonsoft.Json;
using System.Security.Claims;

namespace Ordbox.Api.Extension
{
    public static class UserExtension
    {
        public const string claimPermission = "Permisos";
        public const string CompanyId = "CompanyId";

        public static int[] GetPermission(this ClaimsPrincipal user)
        {
            var claims = user.Claims.FirstOrDefault(p => p.Type == claimPermission);

            if (claims != default)
            {
                var permission = JsonConvert.DeserializeObject<int[]>(claims.Value);

                if (permission != null)
                {
                    return permission;
                }
            }
            return Array.Empty<int>();
        }

        public static long GetCompanyId(this ClaimsPrincipal user)
        {
            var claims = user.Claims.FirstOrDefault(p => p.Type == CompanyId);

            if (claims != default)
            {
                var companyId = JsonConvert.DeserializeObject<long?>(claims.Value);

                if (companyId.HasValue)
                {
                    return companyId.Value;
                }
            }
            return 0;
        }
    }
}
