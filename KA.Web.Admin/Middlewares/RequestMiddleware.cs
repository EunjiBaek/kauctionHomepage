using KA.Entities.Models.Common;
using KA.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KA.Web.Admin.Middlewares
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogRepository _logRepository;
        private readonly IMemberRepository _memberRepository;

        public RequestMiddleware(RequestDelegate requestDelegate, ILogRepository logRepository, IMemberRepository memberRepository)
        {
            _requestDelegate = requestDelegate;
            _logRepository = logRepository;
            _memberRepository = memberRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            #region # Token 처리 #

            var token = context.Request.Cookies["K-Manager.Token"];
            if (token == null)
            {
                token = Guid.NewGuid().ToString("N");
                context.Response.Cookies.Append("K-Manager.Token", token, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
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
                        UserAgent = context.Request.Headers["User-Agent"].ToString()
                    });
                }
                catch (Exception) { }
            }

            #endregion

            await _requestDelegate.Invoke(context);
        }
    }
}
