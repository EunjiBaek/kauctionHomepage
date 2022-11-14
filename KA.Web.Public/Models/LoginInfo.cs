using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace KA.Web.Public.Models
{
    public static class LoginInfo
    {
        static readonly HttpContextAccessor accessor = new HttpContextAccessor();

        private static string Identity => accessor.HttpContext.User.Identity.IsAuthenticated 
            ? KA.Entities.Helpers.DESCryptoHelper.DESDecrypt(accessor.HttpContext.User.Claims.ElementAt(2).Value) : "";

        public static int Uid => string.IsNullOrWhiteSpace(Identity) ? 0 : int.Parse(Identity.Split('^')[0]);

        public static string Email => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[1];

        public static string ID => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[3];

        public static string Name => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[4];

        public static string BirthDate => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[5];

        public static string ManagerApproval => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[6];

        public static string Type => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[7];

        public static string EssentialClause => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[8];

        public static string PrivateClause => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[9];

        public static string ClauseDate => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[10];

        public static string ZipCode => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[11];

        public static string Address => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[12];

        public static string Address2 => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[13];

        public static string FullAddress => $"{ZipCode} {Address} {Address2}";

        public static int MngUid => string.IsNullOrWhiteSpace(Identity) ? 0 : int.Parse(Identity.Split('^')[14]);

        public static string MngName => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[15];

        public static string MngEmail => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[16];

        public static string Mobile => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[17];

        public static string MngHTel => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[18];

        public static string MngExTel => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[19];

        public static int KofficeUid => string.IsNullOrWhiteSpace(Identity) ? 0 : int.Parse(Identity.Split('^')[20]);

        public static string ManagerYN => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[21];

        public static string AdminYN => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[22];

        public static string BidAllowYN => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[23];

        public static string MngTeamEmail => string.IsNullOrWhiteSpace(Identity) ? string.Empty : Identity.Split('^')[24];

        public static string MainHighlight => string.IsNullOrWhiteSpace(Identity) ? string.Empty : (Identity.Split('^').Length > 25 ? Identity.Split('^')[25] : "001");

        public static string ListViewMode => string.IsNullOrWhiteSpace(Identity) ? string.Empty : (Identity.Split('^').Length > 26 ? Identity.Split('^')[26] : "001");

        public static int Age => GetAge(BirthDate);

        /// <summary>
        /// 만 나이 가져오는 함수
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetAge(string value)
        {
            if (DateTime.TryParse(value, out DateTime result))
            {
                DateTime now = DateTime.Now;

                if (result.Month < now.Month)
                {
                    return now.Year - result.Year;
                }
                else if (result.Month.Equals(now.Month))
                {
                    return result.Day <= now.Day ? now.Year - result.Year : now.Year - result.Year - 1;
                }
                else
                {
                    return now.Year - result.Year - 1;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
