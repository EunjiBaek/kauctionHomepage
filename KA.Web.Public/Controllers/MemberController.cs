using ImageMagick;
using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Entities.Models.Email;
using KA.Entities.Models.Member;
using KA.Repositories;
using KA.Web.Public.Models;
using KA.Web.Public.Services;
using KA.Web.Public.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KA.Web.Public.Utils;

namespace KA.Web.Public.Controllers
{
    public class MemberController : BaseController
    {
        #region # Constructor #

        private readonly IMemberRepository _memberRepository;
        private readonly IAuctionRepository _auctionRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly EmailHelper _emailHelper;
        private readonly CommonService _commonService;
        private readonly IRsaCryptoService _rsaCryptoService;

        private readonly OkCertService _okCertService;
        private readonly string _okCertRequestKey;

        public MemberController(IHttpContextAccessor httpContextAccessor,
            IMemberRepository memberRepository,
            IAuctionRepository auctionRepository,
            ILogRepository logRepository,
            ICommonRepository commonRepository,
            EmailHelper emailHelper,
            CommonService commonService, IRsaCryptoService rsaCryptoService)
        {
            _memberRepository = memberRepository;
            _auctionRepository = auctionRepository;
            _logRepository = logRepository;
            _commonRepository = commonRepository;
            _emailHelper = emailHelper;
            _commonService = commonService;
            _rsaCryptoService = rsaCryptoService;

            _okCertService = new OkCertService(httpContextAccessor.HttpContext?.Request);
            _okCertRequestKey = Guid.NewGuid().ToString();
        }

        #endregion

        #region # COMMON API #

        [HttpPost]
        [Route("/api/Member/SetReceiveAdvertising")]
        public JObject SetReceiveAdvertising([FromBody] JObject json)
        {
            var result = _memberRepository.SetMember(new Member()
            {
                Uid = LoginInfo.Uid,
                ReceiveSmsInfo = JsonHelper.GetString(json, "receive_sms_info"),
                ReceiveEmailInfo = JsonHelper.GetString(json, "receive_email_info"),
                ReceivePhoneInfo = JsonHelper.GetString(json, "receive_phone_info")
            }, "UPDATE_ADVERTISING");
            if (result.Result.Equals("00"))
            {
                Response.Cookies.Delete("K-Auction.ReceivingAdvertisingInfo");
            }
            return JsonHelper.GetApiResult(result.Result);
        }

        /// <summary>
        /// 사업자 등록번호 진위여부 체크
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Member/CheckBusinessNum")]
        public JObject CheckBusinessNum([FromBody] JObject json)
        {
            var result = false;
            var url = Config.BusinessNumCheckAPiUrl;
            var businessNum = JsonHelper.GetString(json, "business_num");
            if (!string.IsNullOrWhiteSpace(businessNum))
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);

                    string postData = $"{{\"b_no\": [\"{businessNum.Replace("-", "")}\"]}}";
                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;

                    using Stream stream = request.GetRequestStream();
                    stream.Write(data, 0, data.Length);

                    using var response = (HttpWebResponse)request.GetResponse();
                    HttpStatusCode status = response.StatusCode;
                    Stream respStream = response.GetResponseStream();
                    using StreamReader sr = new(respStream);
                    var responseText = sr.ReadToEnd();

                    JObject jObject = JObject.Parse(responseText);
                    if (jObject.SelectToken("data").Any())
                    {
                        foreach (var item in jObject["data"])
                        {
                            if (item["b_stt_cd"] != null && item["b_stt_cd"].ToString().Equals("01"))
                            {
                                result = true;
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
            return JsonHelper.GetApiResult("00", result.ToString());
        }

        /// <summary>
        /// 회원 보증서 인쇄 이벤트 처리
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("/Member/Certificate/{filename}")]
        public JObject MemberCertifiateUpdate(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) return JsonHelper.GetApiResult("90");

            var dbResult = _memberRepository.SetMemberCertificateHistory("PRINT", new MemberCertificateHistory { FileName = filename });
            return dbResult.Result.Equals("00") ? JsonHelper.GetApiResult("00") : JsonHelper.GetApiResult("90");
        }

        #endregion

        #region # [LOGIN] 휴면계정 활성화 #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Member/SetActivation")]
        public JObject SetActivation([FromBody] JObject json)
        {
            var memID = Request.Cookies["K-Auction.Activation"] != null ? Request.Cookies["K-Auction.Activation"].ToString() : "";
            if (!string.IsNullOrWhiteSpace(memID))
            {
                // 활성화 데이터 치리
                var result = _memberRepository.SetMember(new Member
                {
                    ID = memID,
                    ReceiveSmsInfo = JsonHelper.GetString(json, "receive_sms_info"),
                    ReceiveEmailInfo = JsonHelper.GetString(json, "receive_email_info"),
                    ReceivePhoneInfo = JsonHelper.GetString(json, "receive_phone_info"),
                    InfoValidatePeriod = JsonHelper.GetString(json, "info_validate_period")
                }, "ACTIVATION");

                // 로그인 처리
                if (result.Result.Equals("00"))
                {
                    var member = _memberRepository.GetMember(new Member { ID = memID }, "detail");
                    if (member != null && member.Uid > 0)
                    {
                        SignIn(member);
                        Response.Cookies.Delete("K-Auction.Activation");
                    }
                    else
                    {
                        result.Result = "90";
                    }
                }
                return JsonHelper.GetApiResult(result.Result);
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        #endregion

        public IActionResult Activation()
        {
            return View();
        }

        #endregion

        #region # [UTIL] 로그인 - /Member/Login #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Member/Login")]
        public JObject ModalLogin([FromBody] JObject json)
        {
            var member = _memberRepository.GetMember(new Member
            {
                ID = JsonHelper.GetString(json, "id"),
                Pwd = JsonHelper.GetString(json, "pwd"),
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                RegType = IsMobile() ? "M" : "W",
                Token = Request.Cookies["K-Auction.Token"] != null ? Request.Cookies["K-Auction.Token"].ToString() : "",
                IsSaved = JsonHelper.GetString(json, "is_saved", "F"),
                HighlightRead = JsonHelper.GetString(json, "highlight_read")
            }, "login");

            string result;
            if (member != null && member.Uid > 0)
            {
                if (member.ExcpTypCD != null && member.ExcpTypCD.Equals("PAU"))
                {
                    Response.Cookies.Append("K-Auction.Activation", JsonHelper.GetString(json, "id"));
                    result = "02";

                }
                // 최종 약관 동의 여부 체크
                else if (member.EssentialClause < _commonService.GetMemClauseVersion())
                {
                    Response.Cookies.Append("K-Auction.Agreement", member.ID);
                    Response.Cookies.Append("K-Auction.Agreement2", DESCryptoHelper.DESEncrypt(JsonHelper.GetString(json, "pwd", "")));
                    result = "01|" + member.ID;
                }
                else
                {
                    SignIn(member, JsonHelper.GetString(json, "is_saved", "F"));

                    if (member.PwdNoti != null && member.PwdNoti.Equals("Y"))
                    {
                        result = "00P";
                    }
                    else if (member.RedirectMyPageFlag != null && member.RedirectMyPageFlag.Equals("Y"))
                    {
                        result = "00Y";
                    }
                    else
                    {
                        result = "00";
                    }

                    // 광고성 정보 수신 동의 팝업 오픈 여부
                    if (string.IsNullOrWhiteSpace(member.ReceiveEmailInfo) || string.IsNullOrWhiteSpace(member.ReceivePhoneInfo) || string.IsNullOrWhiteSpace(member.ReceiveSmsInfo))
                    {
                        Response.Cookies.Append("K-Auction.ReceivingAdvertisingInfo", "Y");
                    }
                }
            }
            else
            {
                result = "ka.msg.login.fail";
            }

            return JsonHelper.GetApiResultLang(result, IsKor(),
                new
                {
                    id = JsonHelper.GetString(json, "id", ""),
                    fail_cnt = member != null ? member.PwdFailCnt : 0,
                    fail_lock_time = member != null ? member.PwdFailLockTime : 10
                });
        }

        [Route("/api/Member/LoginCheck")]
        public JObject LoginCheck()
        {
            return JsonHelper.GetApiResult(User.Identity.IsAuthenticated ? "00" : "90");
        }

        #endregion

        #region [View]

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (IsLogin()) return RedirectMain();

            ViewBag.ReturnUrl = returnUrl;
            ViewData["Title"] = GetPageTitle("common", "login");
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(model.ID))
            {
                ModelState.AddModelError("Message", MessageHelper.Get("202", IsKor()));
            }
            if (string.IsNullOrWhiteSpace(model.Pwd))
            {
                ModelState.AddModelError("Message", MessageHelper.Get("203", IsKor()));
            }

            if (ModelState.IsValid)
            {
                model.ID = model.ID;
                model.IP = HttpContext?.Connection.RemoteIpAddress?.ToString();
                model.UserAgent = HttpContext?.Request.Headers["User-Agent"].ToString();
                model.RegType = IsMobile() ? "M" : "W";
                model.Token = Request.Cookies["K-Auction.Token"] != null ? Request.Cookies["K-Auction.Token"] : "";
                model.IsSaved = model.IsSaved == "true" ? "T" : "F";

                var member = _memberRepository.GetMember(model, "login");
                if (member is { Uid: > 0 })
                {
                    // 최종 약관 동의 여부 체크
                    if (member.EssentialClause < _commonService.GetMemClauseVersion())
                    {
                        Response.Cookies.Append("K-Auction.Agreement", member.ID);
                        Response.Cookies.Append("K-Auction.Agreement2", member.Pwd);
                        return RedirectToAction("Agreement", new { id = member.ID });
                    }

                    SignIn(member, model.IsSaved);

                    var url = returnUrl != null && !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl.Split("/", StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
                    var actionName = url.Length > 1 ? url[1] : "Index";
                    var controllerName = url.Length > 1 ? url[0] : "Home";

                    // 광고성 정보 수신 동의 팝업 오픈 여부
                    if (string.IsNullOrWhiteSpace(member.ReceiveEmailInfo) || string.IsNullOrWhiteSpace(member.ReceivePhoneInfo) || string.IsNullOrWhiteSpace(member.ReceiveSmsInfo))
                    {
                        Response.Cookies.Append("K-Auction.ReceivingAdvertisingInfo", "Y");
                    }

                    if (member.RedirectMyPageFlag != null && member.RedirectMyPageFlag.Equals("Y"))
                    {
                        return RedirectToAction("MyPage", "Member");
                    }

                    Response.Cookies.Append("K-Auction.PwdNoti", member.PwdNoti != null && member.PwdNoti.Equals("Y") ? "Y" : "");

                    if (url.Length > 2)
                    {
                        return RedirectPermanent(returnUrl);
                    }

                    return RedirectToAction(actionName, controllerName);
                }

                if (member != null)
                {
                    ViewBag.FailCnt = member.PwdFailCnt;
                    ViewBag.FailLockTime = member.PwdFailLockTime;
                }
                
                ModelState.AddModelError("Message", MessageHelper.Get("201", IsKor()));
                return View("Login");
            }

            return View("Login");
        }

        #endregion

        #endregion

        #region # [UTIL] 로그인 - 약관동의 - /Member/Agreement #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Member/SetAgreement")]
        public JObject SetAgreement([FromBody] JObject json)
        {
            var result = "90";
            var cookieID = Request.Cookies["K-Auction.Agreement"] == null ? string.Empty : Request.Cookies["K-Auction.Agreement"].ToString();
            var cookiePwd = Request.Cookies["K-Auction.Agreement2"] == null ? string.Empty : Request.Cookies["K-Auction.Agreement2"].ToString();
            var paramID = JsonHelper.GetString(json, "id");

            // 응찰화면에서 인증한 경우 (로그인O)
            if (string.IsNullOrWhiteSpace(cookieID) && LoginInfo.Uid > 0)
            {
                var tempMember = _memberRepository.GetMember(LoginInfo.Uid);
                cookieID = tempMember.ID;
                cookiePwd = tempMember.Pwd;
            }

            if (!string.IsNullOrWhiteSpace(cookieID) && LoginInfo.Uid < 1)
            {
                cookiePwd = DESCryptoHelper.DESDecrypt(cookiePwd);
            }

            if (paramID.Equals(cookieID) && !string.IsNullOrWhiteSpace(cookiePwd))
            {
                json["mode"] = "AGREEMENT";
                result = _memberRepository.SetMemberClause(json);
                if (result.Equals("00") && LoginInfo.Uid < 1)
                {
                    Response.Cookies.Delete("K-Auction.Agreement");
                    Response.Cookies.Delete("K-Auction.Agreement2");

                    // 약관 동의 후 인증 처리
                    SignIn(_memberRepository.GetMember(new Member
                    {
                        ID = cookieID,
                        Pwd = cookiePwd,
                        IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                        RegType = IsMobile() ? "M" : "W",
                        Token = Request.Cookies["K-Auction.Token"] != null ? Request.Cookies["K-Auction.Token"].ToString() : "",
                        IsSaved = "F"
                    }, "login"), "F");
                }
            }
            return JsonHelper.GetApiResult(result);
        }

        #endregion

        #region [View]

        [HttpPost, HttpGet]
        public IActionResult Agreement(string id = "")
        {
            var cookieID = Request.Cookies["K-Auction.Agreement"] == null ? string.Empty : Request.Cookies["K-Auction.Agreement"].ToString();
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(cookieID) || !id.Equals(cookieID)) return RedirectToAction("Error", "Home");

            // 핸드폰 인증 내역이 있는지 조회 (Y인 경우 DB 처리 전 모바일 인증 처리)
            var member = _memberRepository.GetMember(new Member() { ID = cookieID }, "mobile_agree_check");

            ViewBag.ID = cookieID;
            ViewBag.Type = member.Type;
            ViewBag.RequireMobileAuth = member.Result;

            var info = _memberRepository.GetTermAndConditions(new JObject()
            {
                ["mode"] = "detail",
                ["type"] = "join",
                ["version"] = _commonService.GetMemClauseVersion()
            });
            if (info != null && info.ToList().Count > 0)
            {
                foreach (var item in info)
                {
                    item.IsKor = IsKor();
                }
                ViewBag.Info = info.ToList()[0].DisplayDescription;
            }

            // mode 가 agree 인 경우 tbl_term_and_condition_detail 테이블 agree_yn 칼럼의 Y값만 가져와 처리
            var clauses = _memberRepository.GetMemberClauses("agree");
            foreach (var item in clauses)
            {
                item.IsKor = IsKor();
            }
            return View(new RegisterViewModel
            {
                memberClauses = clauses
            });
        }

        /// <summary>
        /// 모바일에서 신규 약관 인증 처리 후 정보 Update 및 로그인 처리
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        public IActionResult AgreementComplete([FromQuery(Name = "seq")] string seq = "", [FromQuery(Name = "target")] string target = "")
        {
            if (string.IsNullOrWhiteSpace(seq) || string.IsNullOrWhiteSpace(target)) return RedirectError();

            var cookieID = Request.Cookies["K-Auction.Agreement"] == null ? string.Empty : Request.Cookies["K-Auction.Agreement"].ToString();
            var cookiePwd = Request.Cookies["K-Auction.Agreement2"] == null ? string.Empty : Request.Cookies["K-Auction.Agreement2"].ToString();

            if (target.Equals(cookieID) && !string.IsNullOrWhiteSpace(cookiePwd))
            {
                var result = _memberRepository.SetMemberClause(new JObject { ["mode"] = "AGREEMENT", ["id"] = target, ["seq"] = seq });
                if (result.Equals("00"))
                {
                    Response.Cookies.Delete("K-Auction.Agreement");
                    Response.Cookies.Delete("K-Auction.Agreement2");

                    // 약관 동의 후 인증 처리
                    SignIn(_memberRepository.GetMember(new Member
                    {
                        ID = cookieID,
                        Pwd = cookiePwd,
                        IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                        RegType = IsMobile() ? "M" : "W",
                        Token = Request.Cookies["K-Auction.Token"] != null ? Request.Cookies["K-Auction.Token"].ToString() : "",
                        IsSaved = "F"
                    }, "login"), "F");

                    return RedirectMain();
                }
            }
            return RedirectError();
        }

        #endregion

        #endregion

        #region # [UTIL] 회원가입 - /Member/Join #

        /// <summary>
        /// [#895/오류] 회원가입 링크 오류 수정
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Join(string key = "", string result = "")
        {
            if (IsLogin()) { return RedirectMain(); }

            return RedirectToAction("JoinStep1");
        }

        [Route("/api/Member/CheckID")]
        public bool CheckID([FromBody] JObject json)
        {
            var member = _memberRepository.GetMember(new Member()
            {
                ID = JsonHelper.GetString(json, "id")
            }, "id_check");
            return member.Result.Equals("Y");
        }

        [Route("/api/Member/CheckMobile")]
        public bool CheckMobile([FromBody] JObject json)
        {
            var member = _memberRepository.GetMember(new Member()
            {
                Mobile = JsonHelper.GetString(json, "mobile")
            }, "mobile_check");
            return member.Result.Equals("Y");

        }

        [Route("/api/Member/SetMobileAuth")]
        public JObject SetMobileAuth([FromBody] JObject json)
        {
            var mode = JsonHelper.GetString(json, "mode");
            // 2022.07.18 :: INSERT 인 경우라도 seq 파라미터 값으로 처리
            // var sequence = mode.Equals("CONFIRM") ? JsonHelper.GetString(json, "seq") : Guid.NewGuid().ToString();
            var sequence = JsonHelper.GetString(json, "seq");
            var authNo = mode.Equals("INSERT") ? StringHelper.GetRandomString("num", 6) : JsonHelper.GetString(json, "auth_no");
            var auth = JsonHelper.GetString(json, "auth");
            var result = _memberRepository.SetMemberMobileAuth(new MemberMobileAuth
            {
                Seq = sequence,
                Auth = auth,
                Type = JsonHelper.GetString(json, "type"),
                Type2 = JsonHelper.GetString(json, "req_type"),
                TypeDetail = JsonHelper.GetString(json, "type_detail"),
                Device = IsMobile() ? "M" : "W",
                UserAgent = UserAgent,
                Ip = Ip,
                MobileNo = JsonHelper.GetString(json, "mobile_no"),
                AuthNo = authNo,
                State = mode.Equals("CONFIRM") ? "R" : "S",
                MemUid = LoginInfo.Uid
            }, mode);
            if (result.Equals("00") && auth.Equals("E") && mode.Equals("INSERT"))
            {
                // 고객에게 메일 발송
                var mailResult = _emailHelper.SendMail(new Email()
                {
                    ServiceDomain = Config.ServiceDomain,
                    ToEmail = JsonHelper.GetString(json, "mobile_no"),
                    SubJect = IsKor() ? "해외가입 이메일 인증 번호 안내" : "Email Verification for Foreign Individuals",
                    Body = string.Format("{0}<br />\r\n{1}<br />\r\n{2}"
                        , IsKor() ? "안녕하세요. K옥션입니다." : "Hello, K Auction"
                        , IsKor() ? "케이옥션 회원가입 시 휴대폰번호로 문자인증이 불가능한 경우에 한하여 이메일로도 인증문자를 발송해드리고 있습니다."
                        : "For users who are unable to proceed with mobile SMS verification during member registration, verification codes can now be received via email."
                        , IsKor() ? "아래의 6자리 숫자를 인증번호 입력란에 기입해주세요. " : "Please enter the verification code (seen below) in the 6-digit verification code field."),
                    IsBodyHtml = true,
                    Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889" : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                    Etc1 = authNo,
                    IsKor = IsKor(),
                    Type = "JoinEmailAuthNum"
                });

                return JsonHelper.GetApiResult(mailResult ? "00" : "ka.msg.common.error", new { Sequence = sequence, AuthNo = authNo });
            }
            return JsonHelper.GetApiResult(result, new { Sequence = sequence, AuthNo = authNo });
        }

        [Route("/api/Member/SetMobileForeignAuth")]
        public JObject SetMobileForeignAuth([FromBody] JObject json)
        {
            var mode = JsonHelper.GetString(json, "mode");
            var sequence = mode.Equals("CONFIRM") ? JsonHelper.GetString(json, "seq") : Guid.NewGuid().ToString();
            var authNo = mode.Equals("INSERT") ? StringHelper.GetRandomString("num", 6) : JsonHelper.GetString(json, "auth_no");
            var auth = JsonHelper.GetString(json, "auth");
            var result = _memberRepository.SetMemberMobileForeignAuth(new MemberMobileForeignAuth
            {
                Seq = sequence,
                Type = JsonHelper.GetString(json, "type"),
                TypeDetail = JsonHelper.GetString(json, "type_detail"),
                Device = IsMobile() ? "M" : "W",
                UserAgent = UserAgent,
                Ip = Ip,
                Auth = auth,
                MobileNo = JsonHelper.GetString(json, "mobile_no"),
                AuthNo = authNo
            }, mode);
            _commonService.CheckErrorLog(result);
            if (result.Result.Equals("00") && auth.Equals("E") && mode.Equals("INSERT"))
            {
                // 고객에게 메일 발송
                var mailResult = _emailHelper.SendMail(new Email()
                {
                    ServiceDomain = Config.ServiceDomain,
                    ToEmail = JsonHelper.GetString(json, "mobile_no"),
                    SubJect = IsKor() ? "해외가입 이메일 인증 번호 안내" : "Email Verification for Foreign Individuals",
                    Body = string.Format("{0}<br />\r\n{1}<br />\r\n{2}"
                        , IsKor() ? "안녕하세요. K옥션입니다." : "Hello, K Auction"
                        , IsKor() ? "케이옥션 회원가입 시 휴대폰번호로 문자인증이 불가능한 경우에 한하여 이메일로도 인증문자를 발송해드리고 있습니다."
                        : "For users who are unable to proceed with mobile SMS verification during member registration, verification codes can now be received via email."
                        , IsKor() ? "아래의 6자리 숫자를 인증번호 입력란에 기입해주세요. " : "Please enter the verification code (seen below) in the 6-digit verification code field."),
                    IsBodyHtml = true,
                    Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889" : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                    Etc1 = authNo,
                    IsKor = IsKor(),
                    Type = "JoinEmailAuthNum"
                });

                return JsonHelper.GetApiResult(mailResult ? "00" : "ka.msg.common.error", new { Sequence = sequence, AuthNo = authNo });
            }
            return JsonHelper.GetApiResult(result.Result, new { Sequence = sequence, AuthNo = authNo });
        }

        [HttpGet]
        [Route("/Member/AuthMobileSend/{typeDetail}")]
        [Route("/Member/AuthMobileSend/{typeDetail}/{reqType}")]
        public IActionResult AuthMobileSend(string typeDetail = "", string reqType = "", [FromQuery(Name = "redirect")] string redirectUrl = "", [FromQuery(Name = "target")] string target = "")
        {
            var sendRequest = _okCertService.SendRequest("M", "/Member/AuthMobileReceive/" + _okCertRequestKey);
            if (sendRequest.RSLT)
            {
                _memberRepository.SetMemberMobileAuth(new MemberMobileAuth()
                {
                    Seq = _okCertRequestKey,
                    Auth = "M",
                    Type = string.IsNullOrWhiteSpace(reqType) || reqType.Equals("JoinStep") || reqType.Equals("JoinComplete") ? "J" : reqType,
                    // Type2 = reqType.Equals("JoinStep") ? reqType : string.Empty,
                    Type2 = string.IsNullOrWhiteSpace(reqType) ? string.Empty : reqType,
                    TypeDetail = typeDetail,
                    State = "S",
                    Device = IsMobile() ? "M" : "W",
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                    Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                    MemId = reqType.Equals("JoinComplete") ? LoginInfo.ID : target,
                    MemUid = LoginInfo.Uid,
                    RedirectUrl = redirectUrl
                }, "INSERT");

                Response.Cookies.Append("K-Auction.JoinToken", _okCertRequestKey, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddHours(1) });
                Response.Cookies.Append("K-Auction.JoinType", typeDetail, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddHours(1) });
            }

            return View(sendRequest);
        }

        [HttpPost, HttpGet]
        [Route("/Member/AuthMobileReceive/{okCertRequestKey}")]
        public IActionResult AuthMobileReceive(string okCertRequestKey, string MDL_TKN = "")
        {
            var requestKey = okCertRequestKey;
            var receiveResult = _okCertService.ReceiveResult("M", MDL_TKN);
            receiveResult.IS_MOBILE = IsMobile() ? "Y" : "N";

            if (receiveResult.RSLT && !string.IsNullOrWhiteSpace(requestKey))
            {
                string result = _memberRepository.SetMemberMobileAuth(new MemberMobileAuth()
                {
                    Seq = requestKey,
                    State = "R",
                    Result = receiveResult.RSLT ? "T" : "F",
                    Message = receiveResult.RETURN_MSG,
                    MemName = receiveResult.RSLT_NAME,
                    MobileCo = receiveResult.TEL_COM_CD,
                    MobileNo = receiveResult.TEL_NO,
                    BirthDate = receiveResult.RSLT_BIRTHDAY,
                    Gender = receiveResult.RSLT_SEX_CD,
                    DI = receiveResult.DI,
                    CI = receiveResult.CI,
                    CipherTime = "",
                    MemUid = LoginInfo.Uid
                }, "UPDATE_STATE");

                // 현재 요청 정보에 타입을 가져와 처리
                var memberMobileAuth = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "detail", ["seq"] = requestKey });
                if (memberMobileAuth.Any())
                {
                    receiveResult.REQ_TYPE = memberMobileAuth.First().Type;
                    receiveResult.MEM_UID = memberMobileAuth.First().MemUid;
                    receiveResult.MEM_ID = memberMobileAuth.First().MemId;
                    receiveResult.REDIRECT_URL = memberMobileAuth.First().RedirectUrl;
                    receiveResult.INFO_CHANGE_NAME = LoginInfo.Uid > 0 && !LoginInfo.Name.Equals(memberMobileAuth.First().MemName) ? LoginInfo.Name + " -> " + memberMobileAuth.First().MemName : "";

                    // 2021.12.27 :: 회원 가입 개편
                    if (!string.IsNullOrWhiteSpace(memberMobileAuth.First().Type2))
                    {
                        receiveResult.REQ_TYPE = memberMobileAuth.First().Type2;
                    }
                }

                if (!result.Equals("00"))
                {
                    receiveResult.RSLT = false;
                    receiveResult.RETURN_MSG = MessageHelper.Get(result, IsKor());
                    receiveResult.RSLT_MSG = result.Equals("26") || result.Equals("205") || result.Equals("206") ? receiveResult.RETURN_MSG : receiveResult.RSLT_MSG;
                }
                else
                {
                    receiveResult.REQUEST_KEY = requestKey;
                }
            }
            return View(receiveResult);
        }

        [HttpGet]
        [Route("/Member/AuthCardSend/{typeDetail}")]
        [Route("/Member/AuthCardSend/{typeDetail}/{reqType}")]
        public IActionResult AuthCardSend(string typeDetail = "", string reqType = "", [FromQuery(Name = "redirect")] string redirectUrl = "", [FromQuery(Name = "target")] string target = "")
        {
            var sendRequest = _okCertService.SendRequest("C", "/Member/AuthCardReceive/" + _okCertRequestKey);
            if (sendRequest.RSLT)
            {
                HttpContext.Session.SetString("KA.Join.Auth.RequestKey", _okCertService.REQUEST_KEY);
                _memberRepository.SetMemberMobileAuth(new MemberMobileAuth()
                {
                    Seq = _okCertRequestKey,
                    Auth = "C",
                    Type = string.IsNullOrWhiteSpace(reqType) || reqType.Equals("JoinStep") || reqType.Equals("JoinComplete") ? "J" : reqType,
                    // Type2 = reqType.Equals("JoinStep") ? reqType : string.Empty,
                    Type2 = string.IsNullOrWhiteSpace(reqType) ? string.Empty : reqType,
                    TypeDetail = typeDetail,
                    State = "S",
                    Device = IsMobile() ? "M" : "W",
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                    Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                    MemId = reqType.Equals("JoinComplete") ? LoginInfo.ID : target,
                    MemUid = LoginInfo.Uid,
                    RedirectUrl = redirectUrl
                }, "INSERT");

                Response.Cookies.Append("K-Auction.JoinToken", _okCertRequestKey, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddHours(1) });
                Response.Cookies.Append("K-Auction.JoinType", typeDetail, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddHours(1) });
            }
            return View(sendRequest);
        }

        [HttpPost, HttpGet]
        [Route("/Member/AuthCardReceive/{okCertRequestKey}")]
        public IActionResult AuthCardReceive(string okCertRequestKey, string MDL_TKN = "")
        {
            var requestKey = okCertRequestKey;
            var receiveResult = _okCertService.ReceiveResult("C", MDL_TKN);
            receiveResult.IS_MOBILE = IsMobile() ? "Y" : "N";

            if (receiveResult.RSLT && !string.IsNullOrWhiteSpace(requestKey))
            {
                string result = _memberRepository.SetMemberMobileAuth(new MemberMobileAuth()
                {
                    Seq = requestKey,
                    State = "R",
                    Result = receiveResult.RSLT ? "T" : "F",
                    Message = receiveResult.RETURN_MSG,
                    MemName = receiveResult.RSLT_NAME,
                    CrdCD = receiveResult.CRD_CD,
                    BirthDate = receiveResult.RSLT_BIRTHDAY,
                    Gender = receiveResult.RSLT_SEX_CD,
                    DI = receiveResult.DI,
                    CI = receiveResult.CI,
                    CipherTime = "",
                    MemUid = LoginInfo.Uid
                }, "UPDATE_STATE");

                // 현재 요청 정보에 타입을 가져와 처리
                var memberMobileAuth = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "detail", ["seq"] = requestKey });
                if (memberMobileAuth.Any())
                {
                    receiveResult.REQ_TYPE = memberMobileAuth.First().Type;
                    receiveResult.MEM_UID = memberMobileAuth.First().MemUid;
                    receiveResult.MEM_ID = memberMobileAuth.First().MemId;
                    receiveResult.REDIRECT_URL = memberMobileAuth.First().RedirectUrl;
                    receiveResult.INFO_CHANGE_NAME = LoginInfo.Uid > 0 && !LoginInfo.Name.Equals(memberMobileAuth.First().MemName) ? LoginInfo.Name + " -> " + memberMobileAuth.First().MemName : "";

                    // 2021.12.27 :: 회원 가입 개편
                    if (!string.IsNullOrWhiteSpace(memberMobileAuth.First().Type2))
                    {
                        receiveResult.REQ_TYPE = memberMobileAuth.First().Type2;
                    }
                }

                if (!result.Equals("00"))
                {
                    receiveResult.RSLT = false;
                    receiveResult.RETURN_MSG = MessageHelper.Get(result, IsKor());
                    receiveResult.RSLT_MSG = result.Equals("26") || result.Equals("205") || result.Equals("206") ? receiveResult.RETURN_MSG : receiveResult.RSLT_MSG;
                }
                else
                {
                    receiveResult.REQUEST_KEY = requestKey;
                }
            }
            return View(receiveResult);
        }

        [Route("/api/Member/GetMobileAuth")]
        public JObject GetMobileAuth([FromBody] JObject json)
        {
            json["mode"] = "detail";
            return JsonHelper.GetApiResult("00", _memberRepository.GetMemberMobileAuths(json));
        }

        [Route("/Member/JoinStep1")]
        public IActionResult JoinStep1()
        {
            var clauses = _memberRepository.GetMemberClauses();
            foreach (var item in clauses)
            {
                item.IsKor = IsKor();
            }
            var memberType = _commonRepository.GetCodeList("MEM_TYPE", "list", !IsKor() ? "E" : "K");
            foreach (var item in memberType)
            {
                item.DisplayCodeName = IsKor() ? item.CodeName : item.CodeName2;
            }

            // JoinToken 생성 (Step1/Step2 저장 키)
            if (Request.Cookies["K-Auction.JoinToken"] != null)
            {
                Response.Cookies.Delete("K-Auction.JoinToken");
            }
            string joinToken = Guid.NewGuid().ToString();
            Response.Cookies.Append("K-Auction.JoinToken", joinToken);

            return View(new RegisterViewModel
            {
                memberClauses = clauses,
                memberType = memberType,
                JoinToken = joinToken
            });
        }

        [HttpGet]
        [Route("Member/JoinStep2/{key}")]
        public IActionResult JoinStep2(string key = "")
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var joinToken = Request.Cookies["K-Auction.JoinToken"] != null ? Request.Cookies["K-Auction.JoinToken"].ToString() : string.Empty;
            var joinType = Request.Cookies["K-Auction.JoinType"] != null ? Request.Cookies["K-Auction.JoinType"].ToString() : string.Empty;

            // 파라미터와 세션토큰 비교

            // 인증 결과 정보 처리
            var authResults = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "detail", ["seq"] = key });

            // 해외가입의 경우 모바일 인증으로 처리
            // 2022.05.17 :: 해외 이메일 인증 추가로 인하여 이메일(E)/모바일(M) 값 적용 (없는 경우 M 처리)
            if (authResults.Any() && (authResults.First().TypeDetail.Equals("002") || authResults.First().TypeDetail.Equals("004")))
            {
                authResults.First().Auth = string.IsNullOrWhiteSpace(authResults.First().Auth) ? "M" : authResults.First().Auth;
            }

            // 회원 가입 유형
            string joinTypeName;
            if (authResults.Any())
            {
                joinTypeName = !IsKor() ? "Enter " + authResults.First().CodeNameEn + " Information" : authResults.First().CodeName.Replace("고객", " 정보 입력");
            }
            else
            {
                joinTypeName = !IsKor() ? "Enter Information" : "개인정보 입력";
            }

            return View(new JoinStep2ViewModel()
            {
                Key = key,
                JoinType = authResults.Any() ? authResults.First().TypeDetail : "",
                JoinTypeName = joinTypeName,
                MemCountryCodeList = _commonService.GetCodeList("MEM_COUNTRY", IsKor()),
                MemInterestCodeList = _commonService.GetCodeList("MEM_INTEREST", IsKor()),
                MemJobCodeList = _commonService.GetCodeList("MEM_JOB", IsKor()),
                MemberMobileAuth = authResults.Any() ? authResults.First() : new MemberMobileAuth()
            });
        }

        [HttpPost]
        public async Task<IActionResult> JoinStep2(JoinStep2ViewModel model)
        {
            var fileResult = true;
            Member member = model as Member;
            member.Result = "90";
            List<string> fileNames = new();

            try
            {
                // 국내 개인/법인 회원은 국가코드를 대한민국으로 처리
                if (member.Type.Equals("001") || member.Type.Equals("003")) member.CountryCode = "KOR";

                // 아이디 공백 여부 체크
                if (string.IsNullOrWhiteSpace(model.ID))
                {
                    ModelState.AddModelError("Message", IsKor() ? "아이디를 입력해 주십시오." : "Please enter your ID");
                    member.Result = "23";
                }

                // 가입시 미성년자 유무 체크
                if (LoginInfo.GetAge(model.BirthDate) < 19)
                {
                    ModelState.AddModelError("Message", IsKor() ? "당사 정책상 미성년자는 회원가입 및 응찰이 불가합니다." : "Per company policy, member registration and bidding are not available to minors.");
                    member.Result = "24";
                }

                // 국내개인을 제외한 가입 유형의 첨부파일 처리
                if (!model.Type.Equals("001"))
                {
                    foreach (var item in HttpContext.Request.Form.Files)
                    {
                        if (fileResult)
                        {
                            var filePath = string.Empty;
                            //if (item.Name.StartsWith("identification"))
                            //{
                            //    filePath = "\\Temp\\Member\\Join\\" + model.ID + "\\" + GetImageSavedFolder(item.Name) + "\\" + item.FileName;
                            //    fileResult = FileHelper.CopyTo(item.OpenReadStream(), filePath, Config.ContentPath);
                            //}
                            //else if (item.Name.StartsWith("company-reg-doc"))
                            //{
                            //    filePath = "\\Temp\\Member\\Join\\" + model.ID + "\\" + GetImageSavedFolder(item.Name) + "\\" + item.FileName;
                            //    fileResult = FileHelper.CopyTo(item.OpenReadStream(), filePath, Config.ContentPath);
                            //}
                            //else if (item.Name.StartsWith("company-business-card"))
                            //{
                            //    filePath = "\\Temp\\Member\\Join\\" + model.ID + "\\" + GetImageSavedFolder(item.Name) + "\\" + item.FileName;
                            //    fileResult = FileHelper.CopyTo(item.OpenReadStream(), filePath, Config.ContentPath);
                            //}

                            if (item.FileName[item.FileName.LastIndexOf('.')..].ToLower().Contains(".heic"))
                            {
                                var newFileName = item.FileName.ToUpper().Replace(".HEIC", ".jpg");
                                filePath = "\\Temp\\Member\\Join\\" + model.ID + "\\" + GetImageSavedFolder(item.Name) + "\\" + newFileName;
                                FileHelper.MakeFolder(filePath, Config.ContentPath);

                                using var image = new MagickImage(item.OpenReadStream());
                                image.Format = MagickFormat.Jpg;
                                image.Write(Config.ContentPath + filePath);
                                fileResult = true;

                                SetMemberAttachFileName(member, item.Name, newFileName);
                            }
                            else
                            {
                                filePath = "\\Temp\\Member\\Join\\" + model.ID + "\\" + GetImageSavedFolder(item.Name) + "\\" + item.FileName;
                                fileResult = FileHelper.CopyTo(item.OpenReadStream(), filePath, Config.ContentPath);
                            }

                            if (!fileResult)
                            {
                                ModelState.AddModelError("Message", IsKor() ? "파일 업로드 중 오류가 발생하였습니다." : "An error occurred while uploading the file.");
                            }
                            else
                            {
                                fileNames.Add(filePath);
                            }
                        }
                    }

                    // 임시 경로의 파일을 실제 저장소 위치로 업로드 처리 (AWS/LOCAL)
                    FileUploadResult fileUploadResult = FileHelper.CopyTo(new FileUploadInfo()
                    {
                        ServerType = Config.ContentMode,
                        Target = "Join",
                        FilePath = Config.ContentPath,
                        FileNames = fileNames,
                        AccessKey = Config.AWS.AccessKey,
                        AccessKeyPrivate = Config.AWS.AccessKeyPrivate,
                        SecretKey = Config.AWS.Secretkey,
                        SecretKeyPrivate = Config.AWS.SecretkeyPrivate,
                        BucketNameHomepage = Config.AWS.BucketNameKoffice,
                        BucketNameHomepatePrivate = Config.AWS.BucketNameKofficePrivate,
                        Etc = member.ID,
                        IsKofficeCopy = false
                    });
                    fileResult = fileUploadResult.Result;
                }
                
                // 국내 가입자 모바일 번호 하이픈 처리
                if (member.Type.Equals("001") && !member.Mobile.Contains("-"))
                {
                    member.Mobile = PhoneNumberUtils.SetHyphenToLocalNumber(member.Mobile);
                }

                if (ModelState.IsValid && fileResult)
                {
                    member.RegType = IsMobile() ? "M" : "W";
                    member.IP = HttpContext.Connection.RemoteIpAddress.ToString();

                    // 현재 케이오피스에 등록된 번호 인지 체크
                    var mobileCheckMember = _memberRepository.GetMember(new Member() { Mobile = member.Mobile }, "mobile_check_koffice");

                    DbResult result = _memberRepository.SetMember(member, "INSERT");

                    _commonService.CheckErrorLog(result);

                    if (result.Result.Equals("00") || result.Result.Equals("OK") || result.Result.Contains("OK|"))
                    {
                        // 가입정보 성공 저장 후 로그인 처리
                        member = _memberRepository.GetMember(new Member()
                        {
                            ID = member.ID,
                            Pwd = model.Pwd,
                            IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                        }, "login");

                        // 가입 완료 후 AWS 버킷의 폴더명을 아이디에서 케이오피스 고유번호로 변경
                        if (!model.Type.Equals("001"))
                        {
                            await FileHelper.MoveTo(new FileUploadInfo()
                            {
                                ServerType = Config.ContentMode,
                                Target = "Join",
                                FilePath = Config.ContentPath,
                                FileNames = fileNames,
                                AccessKey = Config.AWS.AccessKey,
                                AccessKeyPrivate = Config.AWS.AccessKeyPrivate,
                                SecretKey = Config.AWS.Secretkey,
                                SecretKeyPrivate = Config.AWS.SecretkeyPrivate,
                                BucketNameHomepage = Config.AWS.BucketNameKoffice,
                                BucketNameHomepatePrivate = Config.AWS.BucketNameKofficePrivate,
                                Etc = member.ID,
                                Etc2 = member.KofficeUid.ToString(),
                                IsKofficeCopy = false
                            });

                            // 임시 업로드 파일 삭제 처리
                            DirectoryInfo directory = new(Config.ContentPath + "\\Temp\\Member\\Join\\" + member.ID);
                            if (directory.Exists)
                            {
                                directory.Delete(true);
                            }
                        }

                        // 로그인 처리
                        SignIn(member);

                        // 회원 가입 완료 메일 처리
                        if (!model.Type.Equals("001"))
                        {
                            var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "MemJoin", ["uid"] = LoginInfo.Uid });
                            if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                            {
                                StringBuilder emailBody = new();
                                emailBody.Append($"아래의 외국인/법인 회원이 가입하였으므로, 온라인 외국인/법인 승인 요청에서 승인/반려 처리하시기 바랍니다.<br /><br />");
                                emailBody.Append("[홈페이지 가입정보]<br />");
                                emailBody.Append($"아이디 : {member.ID}<br />");
                                emailBody.Append($"이름 : {StringHelper.GetPrivateInfoMask(member.Name, "N")}<br />");
                                emailBody.Append($"휴대전화 : {StringHelper.GetPrivateInfoMask(member.Mobile, "M")}<br />");
                                emailBody.Append($"회원번호(홈페이지) : {member.Uid}<br />");
                                emailBody.Append($"회원번호(케이오피스) : {member.KofficeUid}<br /><br />");

                                var emailInfo = new Email()
                                {
                                    AddToEmail = GetEmail(code.Extra1),
                                    AddToName = GetEmail(code.Extra1, "N"),
                                    AddCcEmail = GetEmail(code.Extra2),
                                    AddCcName = GetEmail(code.Extra2, "N"),
                                    SubJect = "[홈페이지알림] 케이옥션 홈페이지 회원가입 (온라인 외국인/법인)",
                                    Body = emailBody.ToString(),
                                    Type = "default",
                                    IsBodyHtml = true,
                                    Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                                };
                                emailInfo.Site = "P";
                                emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                                emailInfo.Type = "Join";
                                emailInfo.RegUid = LoginInfo.Uid;
                                _logRepository.SetEmailLog(emailInfo);
                            }
                        }

                        // 중복 번호 메일 알림 처리
                        if (mobileCheckMember.Result != null && mobileCheckMember.Result.Equals("Y"))
                        {
                            var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "MemJoinExist", ["uid"] = LoginInfo.Uid });
                            if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                            {
                                StringBuilder emailBody = new();
                                emailBody.Append($"케이오피스 회원 {mobileCheckMember.Name}(케이오피스 회원번호 : {mobileCheckMember.KofficeUid})님이 동일한 휴대폰 번호로 홈페이지에 신규가입 하셨습니다.<br /><br />");
                                emailBody.Append("[홈페이지 가입정보]<br />");
                                emailBody.Append($"아이디 : {member.ID}<br />");
                                emailBody.Append($"이름 : {StringHelper.GetPrivateInfoMask(member.Name, "N")}<br />");
                                emailBody.Append($"휴대전화 : {StringHelper.GetPrivateInfoMask(member.Mobile, "M")}<br />");
                                emailBody.Append($"회원번호(홈페이지) : {member.Uid}<br />");
                                emailBody.Append($"회원번호(케이오피스) : {member.KofficeUid}<br /><br />");

                                var emailInfo = new Email()
                                {
                                    AddToEmail = GetEmail(code.Extra1),
                                    AddToName = GetEmail(code.Extra1, "N"),
                                    AddCcEmail = GetEmail(code.Extra2),
                                    AddCcName = GetEmail(code.Extra2, "N"),
                                    SubJect = "[홈페이지알림] 케이옥션 홈페이지 회원가입 (휴대폰중복)",
                                    Body = emailBody.ToString(),
                                    Type = "default",
                                    IsBodyHtml = true,
                                    Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                                };
                                emailInfo.Site = "P";
                                emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                                emailInfo.Type = "Join2";
                                emailInfo.RegUid = LoginInfo.Uid;
                                _logRepository.SetEmailLog(emailInfo);
                            }
                        }

                        return RedirectToAction("JoinStepComplete");
                    }
                    else
                    {
                        // 초기정보 다시 셋팅
                        var authResults = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "detail", ["seq"] = model.MobileAuthSeq });
                        return View(new JoinStep2ViewModel()
                        {
                            Key = model.MobileAuthSeq,
                            JoinType = authResults.Any() ? authResults.First().TypeDetail : "",
                            Message = MessageHelper.Get(member.Result, IsKor()),
                            MemCountryCodeList = _commonService.GetCodeList("MEM_COUNTRY", IsKor()),
                            MemInterestCodeList = _commonService.GetCodeList("MEM_INTEREST", IsKor()),
                            MemJobCodeList = _commonService.GetCodeList("MEM_JOB", IsKor()),
                            MemberMobileAuth = authResults.Any() ? authResults.First() : new MemberMobileAuth()
                        });
                    }
                }
                else
                {
                    // 초기정보 다시 셋팅
                    var authResults = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "detail", ["seq"] = model.MobileAuthSeq });
                    return View(new JoinStep2ViewModel()
                    {
                        Key = model.MobileAuthSeq,
                        JoinType = authResults.Any() ? authResults.First().TypeDetail : "",
                        Message = MessageHelper.Get(member.Result, IsKor()),
                        MemCountryCodeList = _commonService.GetCodeList("MEM_COUNTRY", IsKor()),
                        MemInterestCodeList = _commonService.GetCodeList("MEM_INTEREST", IsKor()),
                        MemJobCodeList = _commonService.GetCodeList("MEM_JOB", IsKor()),
                        MemberMobileAuth = authResults.Any() ? authResults.First() : new MemberMobileAuth()
                    });
                }
            }
            catch (Exception Ex)
            {
                _logRepository.SetErrorLog("Member", "Join", LoginInfo.Uid, Ex); return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region # [UTIL] 회원가입완료 - /Member/JoinComplete #

        [Authorize]
        [Route("/Member/JoinStepComplete")]
        [Route("/Member/JoinStepComplete/{code}")]
        public ActionResult JoinStepComplete(string code = "")
        {
            // adCode 값이 있는 경우 광고성 정보 수신 동의 처리
            if (!string.IsNullOrWhiteSpace(code))
            {
                _memberRepository.SetMember(new Member()
                {
                    Uid = LoginInfo.Uid,
                    ReceiveInfoType = "JoinComplete",
                    ReceiveInfoCode = code.ToUpper(),
                    ReceiveInfoValue = "Y"
                }, "UPDATE_RECEIVE_INFO");
                return Redirect("/Member/JoinStepComplete");
            }

            // 국내개인/법인 휴대폰/신용카드 인증 유무 체크
            var memberAuth = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "check_mem_auth", ["mem_uid"] = LoginInfo.Uid });
            ViewBag.AuthFlag = memberAuth.Any() && memberAuth.First().Result.Equals("Y");
            ViewBag.JoinToken = Request.Cookies["K-Auction.JoinToken"] != null ? Request.Cookies["K-Auction.JoinToken"].ToString() : "";
            return View(_memberRepository.GetMember(LoginInfo.Uid));
        }

        [Authorize]
        [HttpPost]
        [Route("/api/Member/SetExtraInfo")]
        public JObject SetExtraInfo([FromBody] JObject json)
        {
            var dbResult = _memberRepository.SetMember(new Member()
            {
                Uid = LoginInfo.Uid,
                JobCode = JsonHelper.GetString(json, "job_code"),
                CompanyName = JsonHelper.GetString(json, "company_name"),
                CompanyTel = JsonHelper.GetString(json, "company_tel"),
                CompanyFax = JsonHelper.GetString(json, "company_fax"),
                CompanyZipCode = JsonHelper.GetString(json, "company_zipcode"),
                CompanyAddress = JsonHelper.GetString(json, "company_address"),
                CompanyAddress2 = JsonHelper.GetString(json, "company_address2"),
                DeliveryZipCode = JsonHelper.GetString(json, "delivery_zipcode"),
                DeliveryAddress = JsonHelper.GetString(json, "delivery_address"),
                DeliveryAddress2 = JsonHelper.GetString(json, "delivery_address2")
            }, "UPDATE_EXTRA");

            _commonService.CheckErrorLog(dbResult);

            return JsonHelper.GetApiResult(dbResult.Result);
        }

        #endregion

        #region # [UTIL] 아이디/비밀번호 찾기 - /Member/FindIdPass #

        #region [WebApi]

        /// <summary>
        /// 아이디/비밀번호 찾기
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route("/api/Member/SetFindIdPass")]
        public JObject SetFindIdPass([FromBody] JObject json)
        {
            var type = JsonHelper.GetString(json, "type");
            var nameByID = JsonHelper.GetString(json, "name_id");
            var mobile = JsonHelper.GetString(json, "mobile");
            var nameByPwd = JsonHelper.GetString(json, "name_password");
            var id = JsonHelper.GetString(json, "id");

            if (string.IsNullOrWhiteSpace(type) || (!type.Equals("id") && !type.Equals("password")))
            {
                return JsonHelper.GetApiResult("90", "ka.msg.common.error");
            }

            if (type.Equals("password") && string.IsNullOrWhiteSpace(nameByPwd))
            {
                return JsonHelper.GetApiResult("90", "ka.msg.join.nameEmpty");
            }

            if (type.Equals("password") && string.IsNullOrWhiteSpace(id))
            {
                return JsonHelper.GetApiResult("90", "ka.msg.join.idEmpty");
            }

            if (type.Equals("id"))
            {
                if (string.IsNullOrWhiteSpace(nameByID)) return JsonHelper.GetApiResult("90", "ka.msg.join.nameEmpty");
                if (string.IsNullOrWhiteSpace(mobile)) return JsonHelper.GetApiResult("90", "ka.msg.join.memMobileRuleEmpty");

                var member = _memberRepository.GetMember(new Member()
                {
                    Name = nameByID,
                    Mobile = mobile
                }, "find_id") ?? new Member();

                return JsonHelper.GetApiResult(member.Uid > 0 ? "00" : "90", member.Uid > 0 ? "ka.msg.findIdPwd.findId" : "ka.msg.findIdPwd.findIdFail", new Member() { Name = member.Name, ID = member.ID });
            }
            else if (type.Equals("password"))
            {
                if (string.IsNullOrWhiteSpace(nameByPwd)) return JsonHelper.GetApiResult("ka.msg.join.nameEmpty");
                if (string.IsNullOrWhiteSpace(id)) return JsonHelper.GetApiResult("ka.msg.join.idEmpty");

                var member = _memberRepository.GetMember(new Member()
                {
                    Name = nameByPwd,
                    ID = id
                }, "find_password") ?? new Member();

                if (member.Uid > 0)
                {
                    // var oldPassword = DESCryptoHelper.DESDecrypt(member.Pwd);
                    var oldPassword = member.Pwd;
                    var newPassword = StringHelper.GetRandomString("3").ToLower();
                    var result = _memberRepository.SetMember(new Member()
                    {
                        Uid = member.Uid,
                        Pwd = oldPassword,
                        PwdNew = newPassword,
                        PwdType = "F",
                        IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                    }, "CHANGE_PASSWORD");

                    if (result.Result.Equals("00"))
                    {
                        var mailResult = _emailHelper.SendMail(new Email()
                        {
                            ServiceDomain = Config.ServiceDomain,
                            ToEmail = member.Email,
                            ToName = member.Name,
                            SubJect = IsKor() ? "임시 비밀번호 안내입니다." : "A temporary password has been sent to your registered email address. please check your mailbox.",
                            Body = string.Format("{0}<br />\r\n{1}<br />\r\n{2}"
                                    , IsKor() ? "안녕하세요. K옥션입니다." : "Hello, K Auction"
                                    , IsKor() ? "회원님의 임시 비밀번호가 발급되었습니다." : "Your temporary password has been issued."
                                    , IsKor() ? "임시 비밀번호로 로그인 후 비밀번호를 변경해주세요." : "Please change your password after logging in with the temporary password."),
                            IsBodyHtml = true,
                            Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                            Etc1 = newPassword,
                            Type = "FindPassword"
                        });

                        // 오류 시 원래 비밀번호로 변경
                        if (!mailResult)
                        {
                            _memberRepository.SetMember(new Member()
                            {
                                Uid = member.Uid,
                                Pwd = newPassword,
                                PwdNew = oldPassword
                            }, "CHANGE_PASSWORD_RECOVERY");
                        }

                        return JsonHelper.GetApiResult(mailResult ? "00" : "90"
                            , mailResult ? "ka.msg.findIdPwd.findPwd" : "ka.msg.findIdPwd.findPwdEmailFail"
                            , mailResult ? new Member() { Name = member.Name, Email = (member.Email.Length > 3 ? "***" + member.Email[3..] : "") } : new Member() { Name = "", Email = "" });
                    }
                    else
                    {
                        return JsonHelper.GetApiResultLang(result.Result, IsKor());
                    }
                }
                else
                {
                    return JsonHelper.GetApiResult("90", "ka.msg.findIdPwd.findPwdFail");
                }
            }
            else
            {
                return JsonHelper.GetApiResult("ka.msg.common.error");
            }
        }

        [Route("/api/Member/SetFindId")]
        public JObject SetFindId([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90", "ka.msg.common.error");

            var type = JsonHelper.GetString(json, "type");
            var name = JsonHelper.GetString(json, "name");
            var email = JsonHelper.GetString(json, "email");
            var mobile = JsonHelper.GetString(json, "mobile");

            if (string.IsNullOrWhiteSpace(name)) return JsonHelper.GetApiResult("90", "ka.msg.join.nameEmpty");
            if (type.Equals("E") && string.IsNullOrWhiteSpace(email)) return JsonHelper.GetApiResult("90", "ka.msg.findIdPwd.emptyEmail");
            if (type.Equals("M") && string.IsNullOrWhiteSpace(mobile)) return JsonHelper.GetApiResult("90", "ka.msg.join.memMobileRuleEmpty");

            var member = _memberRepository.GetMember(new Member()
            {
                Name = name,
                Mobile = mobile,
                Email = email
            }, "find_id") ?? new Member();

            return JsonHelper.GetApiResult(member.Uid > 0 ? "00" : "90", member.Uid > 0 ? "ka.msg.findIdPwd.findId" : "ka.msg.findIdPwd.findIdFail", new Member() { Name = member.Name, ID = member.ID });
        }

        [Route("/api/Member/SetFindPass")]
        public JObject SetFindPass([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90", "ka.msg.common.error");

            var type = JsonHelper.GetString(json, "type");
            // var name = JsonHelper.GetString(json, "name");
            var email = JsonHelper.GetString(json, "email");
            var mobile = JsonHelper.GetString(json, "mobile");
            var id = JsonHelper.GetString(json, "id");
            var mode = JsonHelper.GetString(json, "mode");

            // if (string.IsNullOrWhiteSpace(name)) return JsonHelper.GetApiResult("90", "ka.msg.join.nameEmpty");
            if (string.IsNullOrWhiteSpace(id)) return JsonHelper.GetApiResult("90", "ka.msg.join.idEmpty");
            if (type.Equals("E") && string.IsNullOrWhiteSpace(email)) return JsonHelper.GetApiResult("90", "ka.msg.findIdPwd.emptyEmail");
            if (type.Equals("M") && string.IsNullOrWhiteSpace(mobile)) return JsonHelper.GetApiResult("90", "ka.msg.findIdPwd.emptyMobile");

            var member = _memberRepository.GetMember(new Member()
            {
                ID = id,
                Email = email,
                Mobile = mobile
            }, "find_password") ?? new Member();

            if (member.Uid > 0)
            {
                if (mode.Equals("C")) // 입력받은 정보로 사용자 정보 있는지 체크
                {
                    return JsonHelper.GetApiResult("00", "ka.msg.findIdPwd.findPwd", type.Equals("E") ? new Member { Email = StringHelper.GetMaskingData(member.Email, "E") } : new Member { Mobile = StringHelper.GetMaskingData(member.Mobile, "M") });
                }
                else if (mode.Equals("S")) // 이메일, 문자 발송 처리
                {
                    var oldPassword = member.Pwd;
                    var newPassword = StringHelper.GetRandomString("3").ToLower();

                    if (type.Equals("E"))
                    {
                        var result = _memberRepository.SetMember(new Member()
                        {
                            Uid = member.Uid,
                            Pwd = oldPassword,
                            PwdNew = newPassword,
                            PwdType = "F",
                            IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                        }, "CHANGE_PASSWORD");
                        if (result.Result.Equals("00"))
                        {
                            var sendEmail = new Email()
                            {
                                ServiceDomain = Config.ServiceDomain,
                                ToEmail = member.Email,
                                ToName = member.Name,
                                SubJect = IsKor() ? "임시 비밀번호 안내입니다." : "A temporary password has been sent to your registered email address. please check your mailbox.",
                                Body = string.Format("{0}<br />\r\n{1}<br />\r\n{2}"
                                        , IsKor() ? "안녕하세요. K옥션입니다." : "Hello, K Auction"
                                        , IsKor() ? "회원님의 임시 비밀번호가 발급되었습니다." : "Your temporary password has been issued."
                                        , IsKor() ? "임시 비밀번호로 로그인 후 비밀번호를 변경해주세요." : "Please change your password after logging in with the temporary password."),
                                IsBodyHtml = true,
                                Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                                Etc1 = newPassword,
                                Type = "FindPassword"
                            };
                            sendEmail.Result = _emailHelper.SendMail(sendEmail) ? "T" : "F";
                            sendEmail.Site = "P";
                            sendEmail.Type = "FindPassword";
                            sendEmail.RegUid = LoginInfo.Uid;
                            sendEmail.Body = sendEmail.Body += " - " + newPassword;
                            _logRepository.SetEmailLog(sendEmail);

                            // 오류 시 원래 비밀번호로 변경
                            if (!sendEmail.Result.Equals("T"))
                            {
                                _memberRepository.SetMember(new Member()
                                {
                                    Uid = member.Uid,
                                    Pwd = newPassword,
                                    PwdNew = oldPassword
                                }, "CHANGE_PASSWORD_RECOVERY");
                            }

                            return JsonHelper.GetApiResult(sendEmail.Result.Equals("T") ? "00" : "90", sendEmail.Result.Equals("T") ? "ka.msg.findIdPwd.findPwd" : "ka.msg.findIdPwd.findPwdEmailFail");
                        }
                        else
                        {
                            return JsonHelper.GetApiResult(result.Result, "ka.msg.common.error");
                        }
                    }
                    else if (type.Equals("M"))
                    {
                        var result = _memberRepository.SetMember(new Member()
                        {
                            Uid = member.Uid,
                            Pwd = oldPassword,
                            PwdNew = newPassword,
                            PwdType = "F",
                            IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                        }, "CHANGE_PASSWORD_ADMIN");
                        return JsonHelper.GetApiResult(result.Result, result.Result.Equals("00") ? "ka.msg.findIdPwd.findPwdComplete" : "ka.msg.common.error");
                    }
                }
            }
            else
            {
                return JsonHelper.GetApiResult("90", "ka.msg.findIdPwd.findPwdFail");
            }

            return null;
        }

        #endregion

        #region [View]

        public ActionResult FindIdPass()
        {
            return View();
        }

        public ActionResult FindId()
        {
            return View();
        }

        public ActionResult FindPass()
        {
            return View();
        }

        #endregion

        [HttpPost]
        public ActionResult FindIdPass(FindIdPassViewModel model)
        {
            if (model.FindMode.Equals("id"))
            {
                if (string.IsNullOrWhiteSpace(model.NameByID))
                {
                    ModelState.AddModelError("MsgByID", IsKor() ? "이름을 입력해 주십시오." : "Please enter your name.");
                }
                if (string.IsNullOrWhiteSpace(model.Mobile))
                {
                    ModelState.AddModelError("MsgByID", IsKor() ? "휴대전화를 입력해 주십시오." : "Please enter your phone number.");
                }

                if (ModelState.IsValid)
                {
                    model.Name = model.NameByID;
                    var member = _memberRepository.GetMember(model, "find_id");
                    member ??= new Member();

                    string message = IsKor() ? "해당되는 아이디가 없습니다." : "Invalid email. Please try again with the correct email or join us.";
                    if (member.Uid > 0)
                    {
                        message = IsKor() ? $"{member.Name} 회원님의 ID는 {member.ID}입니다." : $"{member.Name} Your ID is {member.ID}.";
                    }

                    return View(new FindIdPassViewModel
                    {
                        Result = member.Uid > 0 ? "Y" : "N",
                        FindMode = "id",
                        NameByID = model.NameByID,
                        NameByPass = string.Empty,
                        Mobile = model.Mobile,
                        FindMessage = message
                    });
                }
                else
                {
                    return View(new FindIdPassViewModel
                    {
                        NameByID = model.NameByID,
                        Mobile = model.Mobile,
                        Result = ""
                    });
                }
            }
            else if (model.FindMode.Equals("password"))
            {
                if (string.IsNullOrWhiteSpace(model.NameByPass))
                {
                    ModelState.AddModelError("MsgByPass", IsKor() ? "이름을 입력해 주십시오." : "Please enter your name.");
                }
                if (string.IsNullOrWhiteSpace(model.ID))
                {
                    ModelState.AddModelError("MsgByPass", IsKor() ? "아이디를 입력해 주십시오." : "Please enter your ID.");
                }

                if (ModelState.IsValid)
                {
                    model.Name = model.NameByPass;

                    var member = _memberRepository.GetMember(model, "find_password");
                    member ??= new Member();

                    string message = IsKor() ? "일치하는 회원정보가 없습니다." : "Invalid email. Please try again with the correct email or join us";
                    if (member.Uid > 0)
                    {
                        // var oldPassword = DESCryptoHelper.DESDecrypt(member.Pwd);
                        var oldPassword = member.Pwd;
                        var newPassword = StringHelper.GetRandomString("3").ToLower();
                        var result = _memberRepository.SetMember(new Member()
                        {
                            Uid = member.Uid,
                            Pwd = oldPassword,
                            PwdNew = newPassword
                        }, "CHANGE_PASSWORD");

                        if (result.Result.Equals("00"))
                        {
                            var mailResult = _emailHelper.SendMail(new Email()
                            {
                                ToEmail = member.Email,
                                ToName = member.Name,
                                SubJect = IsKor() ? "임시 비밀번호 안내입니다." : "A temporary password has been sent to your registered email address. please check your mailbox.",
                                Body = string.Format("{0}<br />\r\n{1}<br /><br />\r\n{2}<br /><br />{3}"
                                    , IsKor() ? "안녕하세요." : "Hello"
                                    , IsKor() ? "K옥션입니다." : "K Auction"
                                    , IsKor() ? $"회원님의 임시 비밀번호는 {newPassword} 입니다." : $"Your temporary password is {newPassword}."
                                    , IsKor() ? "임시 비밀번호로 로그인후 비밀번호를 변경해주세요." : "Please change your password after logging in with the temporary password."),
                                IsBodyHtml = true
                            });
                            message = mailResult
                                ? (IsKor() ? $"{member.Name} 회원님의 메일주소{(member.Email.Length > 3 ? "(***" + member.Email[3..] + ")" : "")}로 임시 패스워드가 발송되었습니다."
                                    : $"A temporary password has been sent to the email address {(member.Email.Length > 3 ? "(***" + member.Email[3..] + ")" : "")} of {member.Name}.")
                                : (IsKor() ? "변경된 비밀번호 정보를 메일 발송하는 중 오류가 발생하였습니다. 케이옥션으로 문의 하시기 바랍니다. (02-3479-8888)"
                                    : "An error occurred while mailing changed password information. Please make an inquiry to K-Auction. (02-3479-8888)");

                            // 오류 시 원래 비밀번호로 변경
                            if (!mailResult)
                            {
                                _memberRepository.SetMember(new Member()
                                {
                                    Uid = member.Uid,
                                    Pwd = newPassword,
                                    PwdNew = oldPassword
                                }, "CHANGE_PASSWORD");
                            }
                        }
                        else
                        {
                            message = MessageHelper.Get(result.Result, IsKor());
                        }
                    }

                    return View(new FindIdPassViewModel
                    {
                        Result = member.Uid > 0 ? "Y" : "N",
                        FindMode = "password",
                        NameByID = string.Empty,
                        ID = member.ID,
                        NameByPass = model.NameByPass,
                        FindMessage = message
                    });
                }
                else
                {
                    return View(new FindIdPassViewModel
                    {
                        NameByPass = model.NameByPass,
                        ID = model.ID,
                        Result = ""
                    });
                }
            }

            return View(new FindIdPassViewModel()
            {
                Result = ""
            });
        }

        #endregion

        #region # [UTIL] 로그아웃 - /Member/Logout #

        [HttpGet]
        public IActionResult Logout(string redirect = "")
        {
            SignOut();
            return string.IsNullOrWhiteSpace(redirect) ? RedirectMain() : Redirect(redirect);
        }

        #endregion

        #region # [UTIL] 마이페이지 - /Member/MyPage #

        #region [WebApi]

        [Authorize]
        [Route("/api/Member/SetMember")]
        public JObject SetMember([FromBody] JObject json)
        {
            var fileResult = true;

            var beforeMember = _memberRepository.GetMember(LoginInfo.Uid);

            if (json == null)
            {
                return JsonHelper.GetApiResult("ka.msg.common.error");
            }
            else
            {
                var type = JsonHelper.GetString(json, "type");
                if (type.Equals("002"))
                {
                    #region # 해외개인 - 증빙서류 처리 #
                    var identification = JsonHelper.GetString(json, "identification");
                    var identificationOriginal = JsonHelper.GetString(json, "identification_original");
                    List<string> fileNames = new();

                    if (string.IsNullOrWhiteSpace(identification))
                    {
                        json["identification"] = identificationOriginal;
                    }
                    else
                    {
                        json["identification"] = identification;
                        fileNames.Add("\\Temp\\Member\\Join\\" + LoginInfo.ID + "\\identification\\" + identification);

                        // 첨부파일 임시경로에서 사용자 Uid 폴더 경로로 이동 처리 및 DB 파일명 처리
                        //fileResult = FileHelper.CopyTo(new FileUploadInfo()
                        //{
                        //    ServerType = Config.ContentMode,
                        //    Target = "Join",
                        //    FilePath = Config.ContentPath,
                        //    FileNames = fileNames,
                        //    AccessKey = Config.AWS.AccessKey,
                        //    SecretKey = Config.AWS.Secretkey,
                        //    BucketNameHomepage = Config.AWS.BucketNameHomepage,
                        //    BucketNameKoffice = Config.AWS.BucketNameKoffice,
                        //    Etc = LoginInfo.Uid.ToString(),
                        //    Etc2 = LoginInfo.KofficeUid.ToString(),
                        //    IsKofficeCopy = true
                        //}).Result;
                        fileResult = FileHelper.CopyTo(new FileUploadInfo()
                        {
                            ServerType = Config.ContentMode,
                            Target = "Join",
                            FilePath = Config.ContentPath,
                            FileNames = fileNames,
                            AccessKey = Config.AWS.AccessKey,
                            AccessKeyPrivate = Config.AWS.AccessKeyPrivate,
                            SecretKey = Config.AWS.Secretkey,
                            SecretKeyPrivate = Config.AWS.SecretkeyPrivate,
                            BucketNameHomepage = Config.AWS.BucketNameKoffice,
                            BucketNameHomepatePrivate = Config.AWS.BucketNameKofficePrivate,
                            Etc = LoginInfo.KofficeUid.ToString(),
                            IsKofficeCopy = false
                        }).Result;
                    }
                    #endregion
                }
                else if (type.Equals("003") || type.Equals("004"))
                {
                    #region # 국내법인/해외법인 - 사업자등록증/명함 처리 #
                    var companyRegDoc = JsonHelper.GetString(json, "company_reg_doc");
                    var companyRegDocOriginal = JsonHelper.GetString(json, "company_reg_doc_original");
                    List<string> fileNames = new();

                    if (string.IsNullOrWhiteSpace(companyRegDoc))
                    {
                        json["company_reg_doc"] = companyRegDocOriginal;
                    }
                    else
                    {
                        json["company_reg_doc"] = companyRegDoc;
                        fileNames.Add("\\Temp\\Member\\Join\\" + LoginInfo.ID + "\\memBusinessLicenseDoc\\" + companyRegDoc);

                        fileResult = FileHelper.CopyTo(new FileUploadInfo()
                        {
                            ServerType = Config.ContentMode,
                            Target = "Join",
                            FilePath = Config.ContentPath,
                            FileNames = fileNames,
                            AccessKey = Config.AWS.AccessKey,
                            SecretKey = Config.AWS.Secretkey,
                            BucketNameHomepage = Config.AWS.BucketNameHomepage,
                            BucketNameKoffice = Config.AWS.BucketNameKoffice,
                            Etc = LoginInfo.Uid.ToString(),
                            Etc2 = LoginInfo.KofficeUid.ToString(),
                            IsKofficeCopy = true
                        }).Result;
                    }

                    if (fileResult)
                    {
                        var businessCard = JsonHelper.GetString(json, "company_business_card");
                        var businessCardOriginal = JsonHelper.GetString(json, "company_business_card_original");
                        fileNames.Clear();

                        if (string.IsNullOrWhiteSpace(businessCard))
                        {
                            json["company_business_card"] = businessCardOriginal;
                        }
                        else
                        {
                            json["company_business_card"] = businessCard;
                            fileNames.Add("\\Temp\\Member\\Join\\" + LoginInfo.ID + "\\memBusinessCard\\" + businessCard);

                            fileResult = FileHelper.CopyTo(new FileUploadInfo()
                            {
                                ServerType = Config.ContentMode,
                                Target = "Join",
                                FilePath = Config.ContentPath,
                                FileNames = fileNames,
                                AccessKey = Config.AWS.AccessKey,
                                SecretKey = Config.AWS.Secretkey,
                                BucketNameHomepage = Config.AWS.BucketNameHomepage,
                                BucketNameKoffice = Config.AWS.BucketNameKoffice,
                                Etc = LoginInfo.Uid.ToString(),
                                Etc2 = LoginInfo.KofficeUid.ToString(),
                                IsKofficeCopy = true
                            }).Result;
                        }
                    }
                    #endregion
                }

                if (fileResult)
                {
                    Member member = json.ToObject<Member>();
                    member.Uid = LoginInfo.Uid;
                    member.Mobile = JsonHelper.GetString(json, "mobile");
                    member.Email = JsonHelper.GetString(json, "email");
                    var dbResult = _memberRepository.SetMember(member, "UPDATE");
                    _commonService.CheckErrorLog(dbResult);

                    dbResult.Result = dbResult.Result.Contains("OK|") ? "00" : dbResult.Result;

                    if (dbResult.Result.Equals("00"))
                    {
                        var newMember = _memberRepository.GetMember(LoginInfo.Uid);

                        var changeField = "";
                        var changeValue = "";
                        if (!beforeMember.Email.Equals(newMember.Email))
                        {
                            changeField += IsKor() ? "이메일" : "Email";
                            changeValue += StringHelper.GetPrivateInfoMask(newMember.Email, "E");
                        }
                        if (!beforeMember.Mobile.Equals(newMember.Mobile))
                        {
                            changeField += (!string.IsNullOrWhiteSpace(changeField) ? ", " : "") + (IsKor() ? "휴대폰" : "Mobile");
                            changeValue += (!string.IsNullOrWhiteSpace(changeValue) ? ", " : "") + StringHelper.GetPrivateInfoMask(newMember.Mobile, "M");
                        }

                        if (!string.IsNullOrWhiteSpace(changeField))
                        {
                            // [케이옥션] 고객님의 [핸드폰 번호, 주소, 이메일이]가 ****년 **월**일 [변경내용] 으로 [변경,추가]되었습니다. 감사합니다. 
                            var email = new Email()
                            {
                                Site = "P",
                                SubJect = "[K-Auction] 정보 변경 알림",
                                Body = IsKor() ? $"{StringHelper.GetPrivateInfoMask(LoginInfo.Name, "M")} 고객님의 [{changeField}] 정보가 {DateTime.Now.Year}년 {DateTime.Now.Month}월 {DateTime.Now.Day}일 [{changeValue}] 으로 변경되었습니다. 감사합니다."
                                    : $"Your [{changeField}] information was changed to [{changeValue}] on {DateTime.Now:MMMM dd, yyyy}. Thank you.",
                                IsBodyHtml = true,
                                ToEmail = newMember.Email,
                                ToName = newMember.Name,
                                AddCcEmail = !string.IsNullOrWhiteSpace(newMember.MngTeamEmail) ? GetMemberTeamEmail(newMember.MngTeamEmail) : null,
                                AddCcName = !string.IsNullOrWhiteSpace(newMember.MngTeamEmail) ? GetMemberTeamEmail(newMember.MngTeamEmail) : null,
                                Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                            };
                            email.Result = _emailHelper.SendMail(email) ? "T" : "F";
                            email.Type = "Modify";
                            email.RegUid = LoginInfo.Uid;
                            _logRepository.SetEmailLog(email);
                        }

                        // 첨부파일 수정시 알림 메일 발송
                        if (JsonHelper.GetString(json, "info_req_file").Equals("Y"))
                        {
                            var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "MemFileModify", ["uid"] = LoginInfo.Uid });
                            if (code != null && code.UseYN.Equals("Y") && !string.IsNullOrWhiteSpace(code.Extra1))
                            {
                                StringBuilder emailBody = new();
                                emailBody.Append(type.Equals("002") ? $"케이오피스 회원 {StringHelper.GetPrivateInfoMask(newMember.Name, "N")}(케이오피스 회원번호 : {newMember.KofficeUid})님이 신분증(첨부파일)을 수정하였습니다.<br /><br />"
                                    : $"케이오피스 회원 {StringHelper.GetPrivateInfoMask(newMember.Name, "N")}(케이오피스 회원번호 : {newMember.KofficeUid})님이 법인정보(첨부파일)를 수정하였습니다.<br /><br />");
                                emailBody.Append("[홈페이지 가입정보]<br />");
                                emailBody.Append($"아이디 : {newMember.ID}<br />");
                                emailBody.Append($"이름 : {StringHelper.GetPrivateInfoMask(newMember.Name, "N")}<br />");
                                emailBody.Append($"휴대전화 : {StringHelper.GetPrivateInfoMask(newMember.Mobile, "M")}<br />");
                                emailBody.Append($"회원번호(홈페이지) : {newMember.Uid}<br />");
                                emailBody.Append($"회원번호(케이오피스) : {newMember.KofficeUid}<br /><br />");

                                var emailInfo = new Email()
                                {
                                    AddToEmail = GetEmail(code.Extra1),
                                    AddToName = GetEmail(code.Extra1, "N"),
                                    AddCcEmail = GetEmail(code.Extra2),
                                    AddCcName = GetEmail(code.Extra2, "N"),
                                    SubJect = $"[홈페이지알림] 케이옥션 홈페이지 {(type.Equals("002") ? "해외개인" : "법인회원")} 정보수정",
                                    Body = emailBody.ToString(),
                                    Type = "default",
                                    IsBodyHtml = true,
                                    Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                                };
                                emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
                                emailInfo.Site = "P";
                                emailInfo.Type = "Modify2";
                                emailInfo.RegUid = LoginInfo.Uid;
                                _logRepository.SetEmailLog(emailInfo);
                            }
                        }
                    }

                    return JsonHelper.GetApiResult(dbResult.Result);
                }
                else
                {
                    return JsonHelper.GetApiResult("ka.msg.common.error");
                }
            }
        }

        #endregion

        #region [View]

        [Authorize]
        [HttpGet]
        [Route("/Member/MyPage")]
        [Route("/Member/MyPage/{menu}")]
        public IActionResult MyPage(string menu = "", string key = "", string result = "")
        {
            ViewBag.Member = _memberRepository.GetMember(LoginInfo.Uid);
            var memberActivity = _memberRepository.GetMemberActivities(new JObject { ["mode"] = "mypage", ["mem_uid"] = LoginInfo.Uid });
            foreach (var item in memberActivity)
            {
                item.IsKor = IsKor();
            }
            ViewBag.MemberActivity = memberActivity;

            // 광고성 정보 수신 동의
            var advertisingInfo = _memberRepository.GetMemberClauses("advertising_info");
            if (advertisingInfo.Any())
            {
                foreach (var item in advertisingInfo)
                {
                    item.IsKor = IsKor();
                }
                ViewBag.AdvertisingInfo = advertisingInfo.First().DisplayContent;
            }
            else
            {
                ViewBag.AdvertisingInfo = "";
            }

            // 국내개인/법인 휴대폰/신용카드 인증 유무 체크
            var memberAuth = _memberRepository.GetMemberMobileAuths(new JObject() { ["mode"] = "check_mem_auth", ["mem_uid"] = LoginInfo.Uid });
            ViewBag.AuthFlag = memberAuth.Any() && memberAuth.First().Result.Equals("Y");

            ViewBag.Menu = menu;
            ViewBag.Result = result;
            ViewBag.Key = key;

            return View();
        }

        #endregion

        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult MyPage(MyPageViewModel model)
        {
            string result;

            if (model.ProcessMode.Equals("MI")) // 회원정보 변경
            {
                var dbResult = _memberRepository.SetMember(new Member()
                {
                    Uid = Uid,
                    Type = model.Type,
                    MobileAuthSeq = model.MobileAuthSeq,
                    Name = model.Name,
                    BirthDate = model.BirthDate,
                    BirthType = model.BirthType,
                    Sex = model.Sex,
                    Mobile = model.Mobile,
                    Email = model.Email,
                    CountryCode = model.CountryCode,
                    JobCode = model.JobCode,
                    CompanyName = model.CompanyName,
                    CompanyTel = model.CompanyTel,
                    CompanyFax = model.CompanyFax
                }, "UPDATE");

                _commonService.CheckErrorLog(dbResult);

                result = dbResult.Result;

                if (result.Equals("00"))
                {
                    ModelState.AddModelError("Message" + model.TabIndex.ToString(), IsKor() ? "정보가 변경되었습니다." : "Your Infomation has been changed.");
                }
                else
                {
                    ModelState.AddModelError("Message" + model.TabIndex.ToString(), MessageHelper.Get(result, IsKor()));
                }

                ViewBag.TabIndex = model.TabIndex;
                ViewBag.Member = _memberRepository.GetMember(new Member() { Uid = Uid }, "detail");
                return View();
            }
            else if (model.ProcessMode.Equals("MR")) // 회원탈퇴 (Api 처리로 변경 [SetRetire])
            {
                if (!model.ID.Equals(ID))
                {
                    ModelState.AddModelError("ID", IsKor() ? "아이디가 일치하지 않습니다." : "The ID does not match.");
                }
                // 2021.11.03 :: 패스워드 암호화
                // if (!DESCryptoHelper.DESEncrypt(model.Pwd).Equals(Pwd))
                if (!Pwd.Equals(_commonRepository.GetEncrypt(model.Pwd)))
                {
                    ModelState.AddModelError("Pwd", IsKor() ? "비밀번호가 일치하지 않습니다." : "Passwords do not match.");
                }

                if (ModelState.IsValid)
                {
                    model.Uid = Uid;

                    var dbResult = _memberRepository.SetMemberRetire(model);
                    result = dbResult.Result;

                    if (!result.Equals("00"))
                    {
                        ModelState.AddModelError("Message" + model.TabIndex.ToString(), "탈퇴 처리 중 오류가 발생하였습니다.");
                    }
                    else
                    {
                        return Logout();
                    }
                }

                ViewBag.TabIndex = model.TabIndex;
                ViewBag.Member = _memberRepository.GetMember(new Member() { Uid = Uid }, "detail");
                return View();

            }
            else
            {
                ViewBag.Member = _memberRepository.GetMember(new Member() { Uid = Uid }, "detail");
                return View();
            }
        }

        [Route("/api/Member/AddressList")]
        public JObject GetMemberAddressList()
        {
            var list = _memberRepository.GetMemberAddresses("list", Uid);
            foreach (var item in list)
            {
                item.IsKor = IsKor();
            }
            return JsonHelper.GetApiResultLang("00", IsKor(), list);
        }

        [Route("/api/Member/AddressInfo")]
        public JObject InsertMemberAddress([FromBody] JObject json)
        {
            if (LoginInfo.Uid > 0)
            {
                json["mem_uid"] = LoginInfo.Uid;
                json["contact"] = StringHelper.GetMobileHyphen(JsonHelper.GetString(json, "contact"));

                var address = _memberRepository.GetMemberAddresses("list", LoginInfo.Uid, int.TryParse(JsonHelper.GetString(json, "uid", "0"), out int addrUid) ? addrUid : 0);
                var result = _memberRepository.SetMemberAddress(json);

                if (result.Equals("00"))
                {
                    var newMember = _memberRepository.GetMember(LoginInfo.Uid);
                    var mode = JsonHelper.GetString(json, "mode");
                    string title = mode switch
                    {
                        "INSERT" => IsKor() ? "추가" : "added",
                        "DELETE" => IsKor() ? "삭제" : "deleted",
                        "PRIMARY" => IsKor() ? "기본 주소 변경" : "changed primary address",
                        _ => IsKor() ? "변경" : "changed",
                    };

                    if (mode.Equals("DELETE") || mode.Equals("PRIMARY"))
                    {
                        if (address != null && address.ToList().Count > 0)
                        {
                            var data = address.ToList()[0];
                            json["zipcode"] = data.ZipCode;
                            json["address1"] = data.Address;
                            json["address2"] = data.Address2;
                            json["receiver"] = data.Receiver;
                            json["contact"] = data.Contact;
                        }
                    }

                    var email = new Email()
                    {
                        Site = "P",
                        SubJect = "[K-Auction] 정보 변경 알림",
                        Body = (IsKor() ? $"{StringHelper.GetPrivateInfoMask(LoginInfo.Name, "N")} 고객님의 [주소] 정보가 {DateTime.Now.Year}년 {DateTime.Now.Month}월 {DateTime.Now.Day}일 [{title}] 되었습니다. 감사합니다."
                            : $"Your address information was {title} on {DateTime.Now:MMMM dd, yyyy}. Thank you.")
                            + "<br /><br /> - "
                            + "[" + JsonHelper.GetString(json, "zipcode") + "] "
                            + JsonHelper.GetString(json, "address1") + " "
                            + JsonHelper.GetString(json, "address2") + " / "
                            + JsonHelper.GetString(json, "receiver") + " ("
                            + JsonHelper.GetString(json, "contact") + ")<br />",
                        IsBodyHtml = true,
                        ToEmail = newMember.Email,
                        ToName = newMember.Name,
                        AddCcEmail = GetMemberTeamEmail(newMember.MngTeamEmail),
                        AddCcName = GetMemberTeamEmail(newMember.MngTeamEmail),
                        Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                        : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                    };
                    email.Result = _emailHelper.SendMail(email) ? "T" : "F";
                    email.Type = "Address";
                    email.RegUid = LoginInfo.Uid;
                    _logRepository.SetEmailLog(email);

                    SignIn(newMember, JsonHelper.GetString(json, "is_saved", "F"));
                }

                return JsonHelper.GetApiResultLang(result, IsKor());
            }
            else
            {
                return JsonHelper.GetApiResultLang("90", IsKor());
            }
        }

        #endregion

        #region # [UTIL] 마이페이지 - 정보수정 (1.비밀번호 변경) - /Member/MyPage #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Member/ChangePassword")]
        public JObject SetPassword([FromBody] JObject json)
        {
            if (string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "password_old")))
            {
                return JsonHelper.GetApiResult("ka.msg.mypage.passwordEmpty");
            }

            if (string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "password_new")))
            {
                return JsonHelper.GetApiResult("ka.msg.mypage.passwordEmptyNew");
            }
            
            var passwordOld = _rsaCryptoService.Decrypt(JsonHelper.GetString(json, "password_old"));
            if (string.IsNullOrEmpty(passwordOld))
            {
                return JsonHelper.GetApiResult("ka.msg.mypage.invalidCrypto");
            }
            
            var passwordNew = _rsaCryptoService.Decrypt(JsonHelper.GetString(json, "password_new"));
            var passwordNewConfirm = _rsaCryptoService.Decrypt(JsonHelper.GetString(json, "password_new_confirm"));

            var validateRegex = new Regex("^(?=.*?[A-Za-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{10,}$");

            if (!validateRegex.IsMatch(passwordNew))
            {
                return JsonHelper.GetApiResult("ka.msg.mypage.passwordNewInvalid");
            }

            if (string.IsNullOrWhiteSpace(passwordNewConfirm))
            {
                return JsonHelper.GetApiResult("ka.msg.mypage.passwordEmptyNewConfirm");
            }

            if (!passwordNew.Equals(passwordNewConfirm))
            {
                return JsonHelper.GetApiResult("ka.msg.mypage.passwordNotMatch");
            }

            var member = _memberRepository.GetMember(LoginInfo.Uid);

            // 2021.11.03 :: 패스워드 암호화
            // if (!member.Pwd.Equals(DESCryptoHelper.DESEncrypt(passwordOld)))
            if (!member.Pwd.Equals(_commonRepository.GetEncrypt(passwordOld)))
            {
                return JsonHelper.GetApiResult("ka.msg.mypage.passwordOldNotMatch");
            }

            var dbResult = _memberRepository.SetMember(new Member()
            {
                Uid = Uid,
                Pwd = passwordOld,
                PwdNew = passwordNew,
                PwdType = "M",
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            }, "CHANGE_PASSWORD_MYPAGE");

            _commonService.CheckErrorLog(dbResult);

            switch (dbResult.Result)
            {
                case "00": break;
                case "22": dbResult.Result = "ka.msg.mypage.passwordOldNotMatch"; break;
                case "90":
                default: dbResult.Result = "ka.msg.common.error"; break;
            }
            return JsonHelper.GetApiResult(dbResult.Result);
        }

        #endregion

        #endregion

        #region # [UTIL] 마이페이지 - 정보수정 (4.회원탈퇴) - /Member/MyPage #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Member/SetRetire")]
        public JObject SetRetire([FromBody] JObject json)
        {
            if (!LoginInfo.ID.Equals(JsonHelper.GetString(json, "input_id"), StringComparison.OrdinalIgnoreCase))
            {
                return JsonHelper.GetApiResultLang("201", IsKor());
            }

            var member = _memberRepository.GetMember(LoginInfo.Uid);

            // 2021.11.03 :: 패스워드 암호화
            if (member.Uid < 1 || !member.Pwd.Equals(_commonRepository.GetEncrypt(JsonHelper.GetString(json, "input_pwd"))))
            {
                return JsonHelper.GetApiResultLang("201", IsKor());
            }

            // 케이오피스 탈퇴 조건 체크 & 케이옥션 홈페이지 및 케이오피스 데이터 제거 처리
            var checkResult = _memberRepository.CheckProgressDateForException("RETIRE", LoginInfo.KofficeUid, LoginInfo.Uid);
            if (checkResult != null && checkResult.RsltCD != null && checkResult.RsltCD.Equals("SUCC"))
            {
                json["uid"] = LoginInfo.Uid;
                var dbResult = _memberRepository.SetMemberRetire(json); // 케이옥션 탈퇴 테이블에 내역 추가
                _commonService.CheckErrorLog(dbResult);

                return JsonHelper.GetApiResultLang(dbResult.Result, IsKor());
            }

            // 실패사유 메일 발송 처리
            if (!Config.RetireKoffice.Equals("Y"))
                return JsonHelper.GetApiResultLang("90", IsKor(),
                    checkResult != null ? checkResult.RsltCD : string.Empty);
            
            var code = _commonRepository.GetCode(new JObject() { ["main_code"] = "MAIL_RECEIVER", ["sub_code"] = "MemRetire", ["uid"] = LoginInfo.Uid });
            
            if (code is not { UseYN: "Y" } || string.IsNullOrWhiteSpace(code.Extra1))
                return JsonHelper.GetApiResultLang("90", IsKor(),
                    checkResult != null ? checkResult.RsltCD : string.Empty);
            
            StringBuilder emailBody = new();
            emailBody.Append($"{StringHelper.GetPrivateInfoMask(member.Name, "N")} 회원 (고유번호 : {member.KofficeUid}, 담당자 : {member.MngName}) 이 탈퇴를 요청하였으나, 아래 조건에 해당하여 탈퇴가 불가하오니 고객과 소통 바랍니다.<br /><br />");
            emailBody.Append("1. 진행 중인 경매에 응찰한 경우 -> 라이브 경매 서면/전화 응찰 신청 포함<br />");
            emailBody.Append("2. (주) 케이옥션과 진행 중인 거래내역이 있을 경우<br />");
            emailBody.Append("&nbsp;&nbsp;1) 미수금이 남아 있는 경우(낙찰대금, 배송비, 기타 비용 등)<br />");
            emailBody.Append("&nbsp;&nbsp;2) 미지급액이 남아 있는 경우<br />");
            emailBody.Append("&nbsp;&nbsp;3) 케이 아트 스페이스 계약기간이 종료되지 않은 경우<br />");
            emailBody.Append("&nbsp;&nbsp;4) 담보 계약이 종료되지 않은 경우<br />");
            emailBody.Append("&nbsp;&nbsp;5) 도록 회원 만료일이 지나지 않은 경우<br />");
            emailBody.Append("&nbsp;&nbsp;6) 케이옥션에 보관된 작품이 있는 경우 (위 사항과 중복이 있을 수 있음)<br />");

            var emailInfo = new Email
            {
                AddToEmail = GetEmail(code.Extra1),
                AddToName = GetEmail(code.Extra1, "N"),
                AddCcEmail = GetEmail(code.Extra2),
                AddCcName = GetEmail(code.Extra2, "N"),
                SubJect = $"[홈페이지알림] 케이옥션 홈페이지 탈퇴 처리 불가 알림 ({StringHelper.GetPrivateInfoMask(member.Name, "N")}/{member.KofficeUid})",
                Body = emailBody.ToString(),
                Type = "default",
                IsBodyHtml = true,
                Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                    : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                Site = "P"
            };
            emailInfo.Result = _emailHelper.SendMail(emailInfo) ? "T" : "F";
            emailInfo.Type = "Retire";
            emailInfo.RegUid = LoginInfo.Uid;
            _logRepository.SetEmailLog(emailInfo);

            return JsonHelper.GetApiResultLang("90", IsKor(), checkResult != null ? checkResult.RsltCD : string.Empty); ;
        }

        #endregion

        #endregion

        #region # [UTIL] 위시리스트 #

        [Authorize]
        [HttpGet]
        public IActionResult WishWork()
        {
            return View();
        }

        [HttpPost]
        [Route("/api/Member/SetWishWork/{workUid}")]
        public JObject SetMemberWishWork(string workUid, [FromBody] JObject json)
        {
            if (User.Identity.IsAuthenticated)
            {
                var mode = JsonHelper.GetString(json, "wish_yn", "") == "Y" ? "INSERT" : "DELETE";
                return JsonHelper.GetApiResult(_memberRepository.SetMemberWishWork(mode, LoginInfo.Uid, int.TryParse(workUid, out int uid) ? uid : 0));
            }
            else
            {
                return JsonHelper.GetApiResult("ACCESSDENY");
            }
        }

        [HttpPost]
        [Route("/api/Member/WishWorkList")]
        public JObject GetMemberWishWork([FromBody] JObject json)
        {
            var list = _memberRepository.GetMemberWishWorks("list", User.Identity.IsAuthenticated ? LoginInfo.Uid : 0, JsonHelper.GetString(json, "auc_kind", ""));
            var isKor = IsKor();
            var isLogin = IsLogin();
            foreach (var item in list)
            {
                item.IsKor = isKor;
                item.IsLogin = isLogin;
            }
            return JsonHelper.GetApiResult("00", list);
        }

        #endregion

        #region # [공통] 문의하기 #

        [Authorize]
        [Route("/api/Member/Inquiry")]
        public JObject SetInquiry([FromBody] JObject json)
        {
            json["mode"] = string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "uid")) ? "INSERT" : "UPDATE";
            json["reg_uid"] = LoginInfo.Uid;
            json["reg_type"] = "M";
            json["work_uid"] = JsonHelper.GetString(json, "work_uid", "0");

            var result = _memberRepository.SetMemberInquiry(json);
            _commonService.CheckErrorLog(result);

            var mailContent = JsonHelper.GetString(json, "contents");

            if (!string.IsNullOrWhiteSpace(mailContent) && result.Result.Equals("00"))
            {
                // 경매 정보
                var auctionWork = _auctionRepository.GetAuctionWork(int.TryParse(json["work_uid"].ToString(), out int wUid) ? wUid : 0);
                var member = _memberRepository.GetMember(LoginInfo.Uid);

                EmailHelper emailHelper = new(Startup.Config);
                emailHelper.SendMail(new Email()
                {
                    ToEmail = string.IsNullOrWhiteSpace(member.MngEmail) ? "inbound-team@k-auction.com" : member.MngEmail,
                    ToName = string.IsNullOrWhiteSpace(member.MngEmail) ? "# 고객관리팀" : member.MngName,
                    SubJect = "[홈페이지알림] " + StringHelper.GetPrivateInfoMask(member.Name, "N") + " [홈페이지회원번호 : " + member.Uid.ToString() + "] 님의 문의가 등록되었습니다.",
                    Body = (auctionWork != null ? auctionWork.AucTitle + " Lot." + auctionWork.LotNum.ToString() + "<br />" : "")
                        + "케이오피스 회원번호 - " + member.KofficeUid.ToString() + " / 홈페이지 회원번호 - " + member.Uid.ToString() + "<br />"
                        + JsonHelper.GetString(json, "contents").Replace("\r\n", "<br />"),
                    IsBodyHtml = true,
                    Footer = IsKor() ? "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                                : "Art Tower 23, Eonju-ro 172-gil, Gangnam-gu, Seoul, Korea ㅣ Tax Registration Number 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                });
            }

            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        /// <summary>
        /// 수신/참조 이메일 규칙에 맞게 주소 리턴
        /// </summary>
        /// <param name="emailInfo">LOGIN_USER (로그인한 사용자), MANAGER_APPROVER (담당자의 승인권자)</param>
        /// <param name="loginUserMail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string[] GetEmail(string emailInfo, string type = "")
        {
            if (string.IsNullOrWhiteSpace(emailInfo)) return null;

            if (emailInfo.Contains("|"))
            {
                try
                {
                    var list = new List<string>();
                    foreach (var item in emailInfo.Split(';'))
                    {
                        var name = item.Split('|')[0];
                        var email = item.Split('|')[1];
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            list.Add(type.ToUpper().Equals("N") ? name : email);
                        }
                    }
                    return list.ToArray();
                }
                catch (Exception) { return Array.Empty<string>(); }
            }
            else if (emailInfo.Equals("LOGIN_USER"))
            {
                return type.ToUpper().Equals("N") ? new string[] { LoginInfo.Name } : new string[] { LoginInfo.Email };
            }
            else if (emailInfo.Equals("MANAGER_APPROVER"))
            {
                var member = _memberRepository.GetMemberKOffice(LoginInfo.Uid);
                if (member != null && !string.IsNullOrWhiteSpace(member.MngApprover))
                {
                    var list = new List<string>();
                    foreach (var item in member.MngApprover.Split(';'))
                    {
                        var name = item.Split('|')[0];
                        var email = item.Split('|')[1];
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            list.Add(type.ToUpper().Equals("N") ? name : email);
                        }
                    }
                    return list.ToArray();
                }
                else
                {
                    return Array.Empty<string>();
                }
            }
            else if (emailInfo.Equals("MANAGER"))
            {
                if (LoginInfo.MngEmail != null && !string.IsNullOrWhiteSpace(LoginInfo.MngEmail))
                {
                    return type.ToUpper().Equals("N") ? new string[] { LoginInfo.MngName } : new string[] { LoginInfo.MngEmail };
                }
                else
                {
                    return GetEmail("MANAGER_APPROVER", type);
                }
            }
            else
            {
                return Array.Empty<string>();
            }
        }
    }
}