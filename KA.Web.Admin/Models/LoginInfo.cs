using KA.Entities.Helpers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace KA.Web.Admin.Models
{
    public static class LoginInfo
    {
        static readonly HttpContextAccessor accessor = new();

        public static string GetLoginInfo(int index = 0)
        {
            var tempLoginInfo = DESCryptoHelper.DESDecrypt(accessor.HttpContext.User.Claims.ElementAt(2).Value);
            if (!string.IsNullOrWhiteSpace(tempLoginInfo))
            {
                var loginData = tempLoginInfo.Split('^');
                return index < loginData.Length && index >= 0 ? loginData[index] : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public static List<string> GetAuthInfo()
        {
            var auth = GetLoginInfo(4);
            return auth.Split('@').ToList<string>();
        }

        public static bool IsAuthenticated => accessor.HttpContext.User.Identity.IsAuthenticated;

        public static int UID => IsAuthenticated ? int.TryParse(GetLoginInfo(0), out int result) ? result : -1 : -1;

        public static string ID => IsAuthenticated ? GetLoginInfo(1) : string.Empty;

        public static string Name => IsAuthenticated ? GetLoginInfo(2) : string.Empty;

        public static string Email => IsAuthenticated ? GetLoginInfo(3) : string.Empty;

        public static List<string> Auth => IsAuthenticated ? GetAuthInfo() : new List<string>();

        public static bool ContainAuth(string value) => Auth.Contains("MngAdmin") || Auth.Contains(value);
    }
}
