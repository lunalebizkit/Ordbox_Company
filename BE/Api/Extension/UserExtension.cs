using Newtonsoft.Json;
using Ordbox.Api.Model;
using Ordbox.Domain.Model.Extensions;
using System.Security.Claims;

namespace Ordbox.Api.Extension
{
    public static class UserExtension
    {
        public const string claimPermission = "Permisos";
        public const string CompanyId = "CompanyId";
        public const string UserId = "UserId";

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
        
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var claims = user.Claims.FirstOrDefault(p => p.Type == UserId);

            if (claims != default)
            {
                var userId = JsonConvert.DeserializeObject<long?>(claims.Value);

                if (userId.HasValue)
                {
                    return userId.Value;
                }
            }
            return 0;
        }

        public static RequestedBy GetRequestedBy(this ClaimsPrincipal user)
        {
            return new RequestedBy
            {
                UserId = user.GetUserId(),
                CompanyId = user.GetCompanyId(),
                UserName = user.Identity?.Name ?? string.Empty,
                UserRolId = (Ordbox.Domain.Enum.ERol)Convert.ToInt32(user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value ?? "0")
            };
        }
    }
}
