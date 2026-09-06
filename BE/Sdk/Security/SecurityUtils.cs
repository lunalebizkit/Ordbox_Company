using System;
using System.Linq;

namespace Ordbox.SDK.Security
{

    public static class SecurityUtils
    {
        public static string GetToken()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public static string GetGuidString()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
