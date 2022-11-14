using KA.Entities.Models.Member;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;

namespace KA.Entities.Helpers
{
    public static class SignHelper
    {
        private static ClaimsIdentity GetIdentity(Member member)
        {
            List<string> userData = new List<string>
            {
                member.Uid.ToString(),                      // 0
                member.Email,                               // 1
                member.Pwd,                                 // 2
                member.ID,                                  // 3
                member.Name,                                // 4
                member.BirthDate,                           // 5
                member.ManagerApproval,                     // 6
                member.Type,                                // 7
                member.EssentialClause.ToString(),          // 8
                member.PrivateClause,                       // 9
                member.ClauseDate.ToString("yyyy-MM-dd"),   // 10
                member.ZipCode,                             // 11
                member.Address,                             // 12
                member.Address2,                            // 13
                member.MngUid.ToString(),                   // 14
                member.MngName,                             // 15
                member.MngEmail,                            // 16
                member.Mobile,                              // 17
                member.MngHTel,                             // 18
                member.MngExTel,                            // 19                
                member.KofficeUid.ToString(),               // 20
                member.ManagerYN,                           // 21
                member.AdminYN,                             // 22
                member.BidAllowYN,                          // 23
                member.MngTeamEmail,                        // 24
                member.MainHighlight,                       // 25 (2022.04.18 추가)
                member.ListViewMode                         // 26 (2022.04.18 추가)
            };
            return new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, member.Uid.ToString()),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.UserData, DESCryptoHelper.DESEncrypt(string.Join("^", userData)))
                }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public static ClaimsPrincipal GetPrincipal(Member member)
        {
            return new ClaimsPrincipal(GetIdentity(member));
        }
    }
}
