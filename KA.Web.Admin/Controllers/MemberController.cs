using KA.Entities.Helpers;
using KA.Entities.Models.Chart;
using KA.Entities.Models.Email;
using KA.Entities.Models.Member;
using KA.Repositories;
using KA.Web.Admin.Models;
using KA.Web.Admin.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        #region # Constructor #

        public MemberController(ICommonRepository commonRepository, 
            IMemberRepository memberRepository,
            IHttpContextAccessor httpContextAccessor,
            IManagerRepository managerRepository,
            IMainRepository mainRepository,
            IAuctionRepository auctionRepository,
            IContentRepository contentRepository,
            ILogRepository logRepository,
            EmailHelper emailHelper) 
            : base(commonRepository, memberRepository, httpContextAccessor, managerRepository, mainRepository, auctionRepository, contentRepository, logRepository, emailHelper) { }

        #endregion

        #region # 가입약관 #

        #region [WebAPI]

        [HttpPost]
        [HttpGet]
        [Route("api/Member/GetTermAndConditionVersionList")]
        public JObject GetTermAndConditionVersionList(string type = "")
        {
            var list = memberRepository.GetTermAndConditions(new JObject()
            {
                ["mode"] = "admin_version",
                ["type"] = type
            });
            foreach (var item in list)
            {
                item.Display = item.Version + (item.UseYn.Equals("Y") ? " [현재버전]" : "");
            }
            return JsonHelper.GetApiResult("00", list);
        }

        [HttpPost]
        [Route("api/Member/GetTermAndConditionVersionList2")]
        public JObject GetTermAndConditionVersionList([FromBody] JObject json)
        {
            var list = memberRepository.GetTermAndConditions(json);
            foreach (var item in list)
            {
                item.Display = item.Version + (item.UseYn.Equals("Y") ? " [현재버전]" : "");
            }
            return JsonHelper.GetApiResult("00", list);
        }

        [HttpPost]
        [Route("api/Member/GetTermAndConditionCodeList")]
        public JObject GetTermAndConditionCodeList([FromBody] JObject json)
        {
            return JsonHelper.GetApiResult("00", memberRepository.GetTermAndConditionDetails(new JObject()
            {
                ["mode"] = "admin_list",
                ["version"] = JsonHelper.GetString(json, "version", "0"),
                ["type"] = JsonHelper.GetString(json, "type")
            }));
        }

        [HttpPost]
        [Route("api/Member/GetTermAndConditionCodeDetail")]
        public JObject GetTermAndConditionCodeDetail([FromBody] JObject json)
        {
            return JsonHelper.GetApiResult("00", memberRepository.GetTermAndConditionDetails(new JObject()
            {
                ["mode"] = "detail",
                ["type"] = JsonHelper.GetString(json, "type"),
                ["version"] = JsonHelper.GetString(json, "version", "0"),
                ["code"] = JsonHelper.GetString(json, "code")
            }));
        }

        [HttpPost]
        [Route("api/Member/SetTermAndConditionInfo")]
        public JObject SetTermAndConditionInfo([FromBody] JObject json)
        {
            var result = memberRepository.SetTermAndCondition(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        [HttpPost]
        [Route("api/Member/SetTermAndConditionDetail")]
        public JObject SetTermAndConditionDetail([FromBody] JObject json)
        {
            var result = memberRepository.SetTermAndConditionDetail(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        [HttpPost]
        [Route("api/Member/GetClause")]
        public JObject GetClause([FromBody] JObject json)
        {
            json["mode"] = "detail";
            return JsonHelper.GetApiResult("00", memberRepository.GetMemberClauses("detail", JsonHelper.GetString(json, "code"), JsonHelper.GetString(json, "version")));
        }

        [HttpPost]
        [Route("api/Member/SetClause")]
        public JObject SetClause([FromBody] JObject json)
        {
            json["mode"] = "UPDATE";
            json["uid"] = LoginInfo.UID;
            return JsonHelper.GetApiResult(memberRepository.SetMemberClause(json));
        }

        #endregion

        #region [View]

        [Route("/Member/Clause")]
        public IActionResult Clause()
        {
            return View();
        }

        #endregion

        #endregion

        #region # 회원 목록 (상세포함) #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Member/GetMemberList")]
        public JObject GetMemberList([FromBody] JObject json)
        {
            if (json == null) json = new JObject();

            var filter = JsonHelper.GetString(json, "filter");
            var search = JsonHelper.GetString(json, "search");
            if (filter.Equals("INIT_KAUCTION"))
            {
                return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, null, 0, 0, int.Parse(JsonHelper.GetString(json, "page", "1"))));
            } else if (string.IsNullOrWhiteSpace(search))
            {
                return JsonHelper.GetApiResult("80");
            }

            json["mode"] = "list";
            var data = memberRepository.GetMembers(json);
            foreach (var item in data)
            {
                item.ID = StringHelper.GetPrivateInfoMask(item.ID, "I");
                item.Name = StringHelper.GetPrivateInfoMask(item.Name, "N");
                item.Email = StringHelper.GetPrivateInfoMask(item.Email, "E");
                item.Mobile = StringHelper.GetPrivateInfoMask(item.Mobile, "M");
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;

            // 검색 이력
            if ((JsonHelper.GetString(json, "page", "1")) == "1")
            {
                managerRepository.SetManagerViewHst(new JObject { ["mng_uid"] = LoginInfo.UID, ["type"] = "S", ["target"] = JsonHelper.GetString(json, "search"), ["etc1"] = totalCount.ToString(), ["etc2"] = JsonHelper.GetString(json, "filter") });
            }

            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Member/GetAddressList/{uid}")]
        public JObject GetMemberAddressList(string uid)
        {
            if (int.TryParse(uid, out int result))
            {
                return JsonHelper.GetApiResult("00", memberRepository.GetMemberAddresses("list", result));
            }
            else
            {
                return JsonHelper.GetApiResult("99");
            }
        }

        [Route("/api/Member/GetMobileAuthList/{uid}")]
        public JObject GetMemberMobileAuthList(string uid)
        {
            if (int.TryParse(uid, out int result))
            {
                return JsonHelper.GetApiResult("00", memberRepository.GetMemberMobileAuths(new JObject()
                {
                    ["mode"] = "list",
                    ["mem_uid"] = result
                }));
            }
            else
            {
                return JsonHelper.GetApiResult("99");
            }
        }

        [Route("/api/Member/GetMemberLoginList/{uid}")]
        [Route("/api/Member/GetMemberLoginList/{uid}/{page}")]
        public JObject GetMemberLoginList(string uid, string page = "1")
        {
            if (int.TryParse(uid, out int result))
            {
                return JsonHelper.GetApiResult("00", memberRepository.GetMemberLogins(new JObject()
                {
                    ["mem_uid"] = result,
                    ["page"] = page
                }));
            }
            else
            {
                return JsonHelper.GetApiResult("99");
            }
        }

        [Route("/api/Member/GetMemberPasswordList/{uid}")]
        [Route("/api/Member/GetMemberPasswordList/{uid}/{page}")]
        public JObject GetmemberPasswordList(string uid, string page = "1")
        {
            if (int.TryParse(uid, out int result))
            {
                return JsonHelper.GetApiResult("00", memberRepository.GetMemberPasswordHistories(new JObject()
                {
                    ["mem_uid"] = result,
                    ["page"] = page
                }));
            }
            else
            {
                return JsonHelper.GetApiResult("99");
            }
        }

        [Route("/api/Member/GetSuccessfulBidList/{uid}")]
        public JObject GetSuccessfulBidList(string uid, string page = "1")
        {
            if (int.TryParse(uid, out int result))
            {
                var list = auctionRepository.GetAuctionWorkByUserBid(new JObject()
                {
                    ["mem_uid"] = result,
                    ["page_size"] = "10",
                    ["mode"] = "successful_bid_list",
                    ["page"] = page
                });
                foreach (var item in list)
                {
                    item.IsKor = true;

                    // 케이오피스에서 출력완료한 경우 처리
                    if (!item.KofficePrintDate.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                    {
                        item.CertificatePrintDate = item.KofficePrintDate;
                        item.CertificatePrintYN = "Y";
                        item.CertificateYN = "N";
                    }
                }
                return JsonHelper.GetApiResult("00", list);
            }
            else
            {
                return JsonHelper.GetApiResult("99");
            }
        }

        /// <summary>
        /// 2022.04.06 :: 회원 탈퇴 시 개선 프로세스 적용
        /// - 기존 SetMemberRetire 함수만 호출 하였으나, 케이오피스 탈퇴 유효성 체크 먼저 호출 CheckProgressDateForException
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Member/SetRetire")]
        public JObject SetRetire([FromBody] JObject json)
        {
            var member = memberRepository.GetMember(int.Parse(JsonHelper.GetString(json, "mem_uid", "0")));
            var checkResult = memberRepository.CheckProgressDateForException("RETIRE", member.KofficeUid, member.Uid);
            if (checkResult != null && checkResult.RsltCD != null && checkResult.RsltCD.Equals("SUCC"))
            {

                json["uid"] = member.Uid;
                var dbResult = memberRepository.SetMemberRetire(new MemberRetire()
                {
                    Uid = int.Parse(JsonHelper.GetString(json, "mem_uid", "0")),
                    RetireReason = JsonHelper.GetString(json, "retire_reason"),
                    RetireOption = "999",
                    MngUid = LoginInfo.UID
                });
                CheckErrorLog(dbResult);

                return JsonHelper.GetApiResultLang(dbResult.Result, true);
            }
            else
            {
                return JsonHelper.GetApiResultLang("90", true, checkResult != null ? checkResult.RsltCD : string.Empty); ;
            }
        }

        /// <summary>
        /// 2022.05.30 :: [현업요청] 어드민 회원정보 비밀번호 초기화 팝업 변경 (#684)
        /// - pwd_target 값 추가하여 U인 경우 회원 등록정보가 아닌 관리자 입력 값으로 문자/메일 발송 처리
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Member/initPassword")]
        public JObject InitPassword([FromBody] JObject json)
        {
            var type = JsonHelper.GetString(json, "type");
            var memUid = JsonHelper.GetString(json, "mem_uid");
            var pwdTarget = JsonHelper.GetString(json, "pwd_target");
            var pwdValue = JsonHelper.GetString(json, "pwd_value");

            var member = memberRepository.GetMember(int.TryParse(memUid, out int uid) ? uid : 0, "detail_admin");
            if (member.Uid < 1) return JsonHelper.GetApiResult("90");
            if (pwdTarget.Equals("U") && string.IsNullOrWhiteSpace(pwdValue)) return JsonHelper.GetApiResult("90");

            var oldPassword = member.Pwd;
            var newPassword = StringHelper.GetRandomString("3").ToLower();

            if (type.Equals("E"))
            {
                var result = memberRepository.SetMember(new Member()
                {
                    Uid = member.Uid,
                    Pwd = oldPassword,
                    PwdNew = newPassword,
                    PwdType = "A",
                    PwdNotiTarget = pwdTarget,
                    PwdNotiValue = pwdValue,
                    IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                }, "CHANGE_PASSWORD");

                if (result.Result.Equals("00"))
                {
                    var email = new Email()
                    {
                        ServiceDomain = Config.ServiceDomain,
                        ToEmail = pwdTarget.Equals("U") ? pwdValue : member.Email,
                        ToName = pwdTarget.Equals("U") ? pwdValue : member.Name,
                        SubJect = "임시 비밀번호 안내입니다.",
                        Body = string.Format("{0}<br />\r\n{1}<br />\r\n{2}"
                                , "안녕하세요. K옥션입니다."
                                , "회원님의 임시 비밀번호가 발급되었습니다."
                                , "임시 비밀번호로 로그인 후 비밀번호를 변경해주세요."),
                        IsBodyHtml = true,
                        Footer = "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889",
                        Etc1 = newPassword,
                        Type = "FindPassword"
                    };
                    email.Result = emailHelper.SendMail(email) ? "T" : "F";
                    email.Site = "A";
                    email.Type = "Password";
                    email.RegUid = LoginInfo.UID;
                    email.Body += " - " + newPassword;
                    logRepository.SetEmailLog(email);

                    // 오류 시 원래 비밀번호로 변경
                    if (!email.Result.Equals("T"))
                    {
                        memberRepository.SetMember(new Member()
                        {
                            Uid = member.Uid,
                            Pwd = newPassword,
                            PwdNew = oldPassword
                        }, "CHANGE_PASSWORD_RECOVERY");
                    }

                    return JsonHelper.GetApiResult(email.Result.Equals("T") ? "00" : "90");                        
                }
                else
                {
                    return JsonHelper.GetApiResultLang(result.Result, true);
                }
            }
            else if (type.Equals("M"))
            {
                var result = memberRepository.SetMember(new Member()
                {
                    Uid = member.Uid,
                    Pwd = oldPassword,
                    PwdNew = newPassword,
                    PwdType = "A",
                    PwdNotiTarget = pwdTarget,
                    PwdNotiValue = pwdValue,
                    IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                }, "CHANGE_PASSWORD_ADMIN");
                return JsonHelper.GetApiResult(result.Result);
            }
            else
            {
                return JsonHelper.GetApiResult("90");
            }
        }

        [HttpPost]
        [Route("/api/Member/SetMemberAdmin")]
        public JObject SetMemberAdmin([FromBody] JObject json)
        {
            var mode = JsonHelper.GetString(json, "mode");
            if (mode.Equals("UPDATE_EMAIL"))
            {
                if (string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "email")))
                {
                    return JsonHelper.GetApiResult("90");
                }

                var result = memberRepository.SetMemberTest($"UPDATE dbo.tbl_member set email = '{JsonHelper.GetString(json, "email")}' where uid = {JsonHelper.GetString(json, "mem_uid")}; UPDATE db_koffice.dbo.off_member SET email = '{JsonHelper.GetString(json, "email")}' where uid = (SELECT TOP 1 koffice_uid FROM dbo.tbl_member where uid = {JsonHelper.GetString(json, "mem_uid")}); ");
                return JsonHelper.GetApiResult(result.Result);
            }
            else if (mode.Equals("UPDATE_MOBILE"))
            {
                if (string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "mobile")))
                {
                    return JsonHelper.GetApiResult("90");
                }

                var result = memberRepository.SetMemberTest($"UPDATE dbo.tbl_member set mobile = '{JsonHelper.GetString(json, "mobile")}' where uid = {JsonHelper.GetString(json, "mem_uid")}; UPDATE db_koffice.dbo.off_member SET reg_tel_no = '{JsonHelper.GetString(json, "mobile")}' where uid = (SELECT TOP 1 koffice_uid FROM dbo.tbl_member where uid = {JsonHelper.GetString(json, "mem_uid")});  ");
                return JsonHelper.GetApiResult(result.Result);
            }
            else
            {
                var result = memberRepository.SetMember(new Member()
                {
                    Uid = int.Parse(JsonHelper.GetString(json, "mem_uid", "0")),
                    AdminYN = JsonHelper.GetString(json, "admin_yn"),
                    ManagerYN = JsonHelper.GetString(json, "manager_yn"),
                    MngUid = LoginInfo.UID
                }, JsonHelper.GetString(json, "mode"));

                CheckErrorLog(result);

                return JsonHelper.GetApiResult(result.Result);
            }
        }

        #endregion

        #region [View]

        [Route("/Member/List")]
        public IActionResult List()
        {
            if (LoginInfo.UID < 1)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }

        [Route("/Member/{uid}")]
        public IActionResult Member(string uid)
        {
            if (int.TryParse(uid, out int result) && LoginInfo.UID > 0)
            {
                // 조회 이력
                managerRepository.SetManagerViewHst(new JObject { ["mng_uid"] = LoginInfo.UID, ["type"] = "V", ["target"] = result.ToString() });

                var memberKOffice = memberRepository.GetMemberKOffice(result);
                var receivable = memberRepository.SetMemberRetire(new MemberRetire()
                {
                    Uid = result
                }, "RECEIVABLE");

                return View(new MemberViewModel()
                {
                    member = memberRepository.GetMember(result, "detail_admin"),
                    memberKOffice = memberKOffice ?? new MemberKOffice(),
                    receivable = receivable.Result
                });
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #endregion

        #region # 회원 탈퇴 목록 #

        #region # [WebApi] #

        [HttpPost]
        [Route("/api/Member/GetMemberRetireList")]
        public JObject GetMemberRetireList([FromBody] JObject json)
        {
            if (json == null) json = new JObject();

            if (!string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "filter")) && string.IsNullOrWhiteSpace(JsonHelper.GetString(json, "search")))
            {
                return JsonHelper.GetApiResult("80");
            }

            json["mode"] = "list";

            var data = memberRepository.GetMemberRetires(json);
            foreach (var item in data)
            {
                item.KofficeLink = Config.KofficeDomain + $"/Pages/Member/MemberView?mem_uid={item.KofficeUid}";
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        #endregion

        #region # [View] #

        [Route("/Member/RetireList")]
        public IActionResult RetireList()
        {
            return View();
        }

        #endregion

        #endregion

        #region # 회원 대시보드 #

        #region [WebApi]

        [HttpPost]
        [Route("/api/Member/getJoinColumnChartData")]
        public JObject GetJoinColumnChartData([FromBody] JObject json)
        {
            var startDate = DateTime.Now.AddYears(-1).ToString("yyyy-MM-01");
            var categories = new List<string>();
            var data = new List<int>();
            var series = new List<Series>();
           
            // 전체 데이터 처리
            var totalData = memberRepository.GetMemberStatisticJoins(new JObject { ["mode"] = "join_month", ["start_date"] = startDate });
            var targetDate = DateTime.Parse(startDate);
            while (targetDate <= DateTime.Now)
            {
                categories.Add(targetDate.ToString("yyyy-MM"));
                var temp = totalData.Where(x => x.DisplayDate.Equals(targetDate.ToString("yyyy-MM")));
                if (temp.Any())
                {
                    data.Add(temp.First().JoinCount);
                }
                else
                {
                    data.Add(0);
                }
                targetDate = targetDate.AddMonths(1);
            }
            series.Add(new Series { Name = "전체", Data = data.ToArray() });

            targetDate = DateTime.Parse(startDate);

            // 회원유형 데이터 처리
            var memberType = commonRepository.GetCodeList("MEM_TYPE");
            var typeDate = memberRepository.GetMemberStatisticJoins(new JObject { ["mode"] = "join_type_month", ["start_date"] = startDate });
            var dictionary = new Dictionary<string, List<int>>();
            while (targetDate <= DateTime.Now)
            {
                foreach (var item in memberType)
                {
                    // TODO: Series 개체에 4가지 타입 데이터 저장.
                    if (!dictionary.ContainsKey(item.SubCode))
                    {
                        dictionary.Add(item.SubCode, new List<int>());
                    }

                    var temp = typeDate.Where(x => x.DisplayDate.Equals(targetDate.ToString("yyyy-MM")) && x.Type.Equals(item.SubCode));
                    if (temp.Any())
                    {
                        dictionary[item.SubCode].Add(temp.First().JoinCount);
                    }
                    else
                    {
                        dictionary[item.SubCode].Add(0);
                    }
                }

                targetDate = targetDate.AddMonths(1);
            }

            foreach (KeyValuePair<string, List<int>> item in dictionary)
            {
                series.Add(new Series 
                { 
                    Name = memberType.Where(x => x.SubCode.Equals(item.Key)).Any() ? memberType.Where(x => x.SubCode.Equals(item.Key)).First().CodeName : "",
                    Data = item.Value.ToArray()
                });
            }

            // 차트 개체
            var columnChart = new ColumnChart()
            {
                Categories = categories.ToArray(),
                Series = series.ToArray()
            };

            return JsonHelper.GetApiResult("00", columnChart);
        }

        #endregion

        #region [View]

        [Route("/Member/Dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        #endregion

        #endregion

        #region # 회원 보증서 출력 내역 #

        #region [WebApi]

        [Route("/api/Member/GetWorkCertificateList")]
        public JObject GetWorkCertificateList([FromBody] JObject json)
        {
            if (json == null) json = new JObject();

            json["mode"] = "admin";
            var data = memberRepository.GetMemberCertificateHistories(json);
            foreach (var item in data)
            {
                item.MemName = StringHelper.GetPrivateInfoMask(item.MemName, "N");
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;

            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        #endregion

        #region [View] 

        [Route("/Member/WorkCertificate")]
        public IActionResult WorkCertificate()
        {
            return View();
        }

        #endregion

        #region # 회원 보증서 신청 내역 #

        #region [WebApi]

        [Route("/api/Member/GetWorkCertificateRequest")]
        public JObject GetWorkCertificateRequest([FromBody] JObject json)
        {
            if (json == null) json = new JObject();

            json["mode"] = "admin";
            var data = memberRepository.GetMemberCertificateRequests(json);
            foreach (var item in data)
            {
                item.MemName = StringHelper.GetPrivateInfoMask(item.MemName, "N");
                item.ReceiverName = StringHelper.GetPrivateInfoMask(item.ReceiverName, "N");
                item.ReceiverMobile = StringHelper.GetPrivateInfoMask(item.ReceiverMobile, "M");
                item.ReceiverEmail = StringHelper.GetPrivateInfoMask(item.ReceiverEmail, "E");
                item.ReceiverAddress = StringHelper.GetPrivateInfoMask(item.ReceiverAddress, "A");
            }
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;

            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        #endregion

        #region [View]

        [Route("/Member/WorkCertificateRequest")]
        public IActionResult WorkCertificateRequest()
        {
            return View();
        }

        #endregion

        #endregion

        #endregion

        #region # 회원 일일 접속 현황 #

        #region [WebApi]

        [Route("/api/Member/GetDailyAccessStatusList")]
        public JObject GetDailyAccessStatusList([FromBody] JObject json)
        {
            json ??= new JObject();

            json["mode"] = "list";
            var data = memberRepository.GetMemberDailyAccessStatuses(json);
            var totalCount = 0;

            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        [Route("/api/Member/GetDailyAccessStatusDetailList")]
        public JObject GetDailyAccessStatusDetailList([FromBody] JObject json)
        {
            json ??= new JObject();

            json["mode"] = "day_list";
            var data = memberRepository.GetMemberDailyAccessStatusesDetail(json);
            var totalCount = 0;

            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        #endregion

        #region [View]

        [Route("/Member/DailyAccessStatus")]
        public IActionResult DailyAccessStatus()
        {
            return View();
        }

        [Route("/Member/DailyAccessStatus/{date}")]
        public IActionResult DailyAccessStatusDetail(string date)
        {
            ViewBag.Date = date;
            return View();
        }

        #endregion

        #endregion
    }
}