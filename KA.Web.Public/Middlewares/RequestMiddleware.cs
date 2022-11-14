using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Repositories;
using KA.Web.Public.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KA.Web.Public.Middlewares
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogRepository _logRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ICommonRepository _commonRepository;

        public RequestMiddleware(RequestDelegate requestDelegate, ILogRepository logRepository, IMemberRepository memberRepository, ICommonRepository commonRepository)
        {
            _requestDelegate = requestDelegate;
            _logRepository = logRepository;
            _memberRepository = memberRepository;
            _commonRepository = commonRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                #region # Token 처리 #

                var token = context.Request.Cookies["K-Auction.Token"];
                if (token == null)
                {
                    token = Guid.NewGuid().ToString("N");
                    context.Response.Cookies.Append("K-Auction.Token", token, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
                }

                #endregion

                #region # Login Keepup 처리 #

                if (LoginInfo.Uid < 1)
                {
                    var keepup = context.Request.Cookies["K-Auction.Keepup"];
                    if (keepup != null && keepup.All(char.IsDigit))
                    {
                        var uid = int.TryParse(keepup, out int result) ? result : 0;
                        context.Response.Cookies.Delete("K-Auction.Keepup");
                        if (uid > 0)
                        {
                            var userAgent = context.Request.Headers["user-agent"].ToString();
                            var ip = context.Connection.RemoteIpAddress.ToString();
                            var member = _memberRepository.GetMemberLogin(uid, token, userAgent, ip, "Y", IsMobile(context) ? "M" : "W");
                            if (member != null && member.Uid > 0)
                            {
                                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, SignHelper.GetPrincipal(member));
                                context.Response.Cookies.Append("K-Auction.Keepup", keepup, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
                                context.Session.SetString("K-Auction.KeepupRefresh", "Y");
                            }
                        }
                    }
                }

                #endregion

                #region # Log 처리 #

                var path = context.Request.Path.ToString();
                if (!path.ToLower().Contains("/apple-touch") && !path.ToLower().Contains("/favicon") && !path.ToLower().Contains("/css") && !path.ToLower().Contains("/fonts") && !path.ToLower().Contains("/images") && !path.ToLower().Contains("/img") && !path.ToLower().Contains("/js") && !path.ToLower().Contains("/plugins") && !path.ToLower().Contains("/auctionhub") && !path.ToLower().Contains("/logincheck"))
                {
                    try
                    {
                        _logRepository.SetLog(new RequestLog()
                        {
                            UserID = context.User.Identity.IsAuthenticated ? context.User.Claims.ElementAt(0).Value : string.Empty,
                            Path = path,
                            Referer = context.Request.Headers["Referer"].ToString(),
                            Ip = context.Connection.RemoteIpAddress.ToString(),
                            UserAgent = context.Request.Headers["User-Agent"].ToString(),
                            Token = token
                        });
                    }
                    catch (Exception) { }
                }

                #endregion

                await _requestDelegate.Invoke(context);
            }
            catch (Exception Ex) 
            {
                _commonRepository.SetErrorLog(new DbResult()
                {
                    ErrorProcedure = "SYSTEM",
                    ErrorNumber = 999,
                    ErrorMessage = Ex.StackTrace
                });
            }
        }

        public bool IsMobile(HttpContext context)
        {
            string u = context.Request.Headers["User-Agent"].ToString();
            Regex b = new(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return b.IsMatch(u) || v.IsMatch(u.Substring(0, 4));
        }
    }
}
