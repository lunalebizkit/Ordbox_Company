using System.Security.Claims;

namespace Ordbox.SDK.Security
{
    public static class ClaimsPrincipalExtension
    {
        public static string Rol(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        public static bool HasRol(this ClaimsPrincipal user, string rolKey)
        {
            var rol = user.FindFirst(ClaimTypes.Role)?.Value ?? "";
            return rol == rolKey;
        }
    }
}
