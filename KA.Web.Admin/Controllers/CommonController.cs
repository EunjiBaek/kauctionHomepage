using ClosedXML.Excel;
using KA.Entities.Helpers;
using KA.Entities.Models.Email;
using KA.Repositories;
using KA.Web.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace KA.Web.Admin.Controllers
{
    public class CommonController : BaseController
    {
        #region # Constructor #

        public CommonController(ICommonRepository commonRepository,
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

        #region # MailSend #

        #region [WebApi]

        [Route("/api/Common/SendMail")]
        public JObject SendMail([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            var mode = JsonHelper.GetString(json, "mode");
            var type = JsonHelper.GetString(json, "type");

            var result = "00";

            if (mode.ToLower().Equals("consign"))
            {
                json["mode"] = "update_state";
                json["state"] = type.ToLower().Equals("a") ? "004" : "005";
                json["admin_uid"] = LoginInfo.UID;
                result = memberRepository.SetMemberConsign(json).Result;
            }

            if (result.Equals("00")) // 메일 발송 처리
            {
                var email = new Email()
                {
                    ToEmail = JsonHelper.GetString(json, "receiver_email"),
                    ToName = JsonHelper.GetString(json, "receiver_name"),
                    SubJect = JsonHelper.GetString(json, "subject"),
                    Body = JsonHelper.GetString(json, "body"),
                    IsBodyHtml = true,
                    Footer = "서울시 강남구 언주로 172길 23 아트타워 ㅣ 사업자등록번호 101-86-17910<br />TEL. 02-3479-8888 ㅣ FAX. 02-3479-8889"
                };
                email.Site = "A";
                email.Type = "Consign";
                email.Result = emailHelper.SendMail(email) ? "T" : "F";
                email.RegUid = LoginInfo.UID;
                logRepository.SetEmailLog(email);
            }

            return JsonHelper.GetApiResult(result);
        }

        #endregion

        #region [View]

        [HttpGet]
        public IActionResult MailSend(string mode = "", string type = "", string uid = "")
        {
            ViewBag.Mode = mode;
            ViewBag.Type = type;
            ViewBag.Uid = uid;
            
            if (mode.Equals("consign"))
            {
                var consignInfo = memberRepository.GetMemberConsign(int.TryParse(uid, out int outUid) ? outUid : 0);
                ViewBag.Receiver = $"{consignInfo.MemName} <{consignInfo.MemEmail}>";
                ViewBag.ReceiverName = consignInfo.MemName;
                ViewBag.ReceiverEmail = consignInfo.MemEmail;
                ViewBag.Subject = $"{consignInfo.MemName}님 케이옥션에 위탁신청해 주셔서 감사합니다.";
                ViewBag.Body = type.Equals("R")
                    ? "안녕하세요. 케이옥션입니다.<br />"
                        + "보내주신 작품사진은 잘 받아보았습니다.<br /><br />"
                        + "그러나 귀하께서 보내주신 사진의 작품은 케이옥션의 경매 출품이 불가함을 알려드립니다.<br /><br />"
                        + "보내주신 사진의 작품은 현재 거래가 어려운 작품으로 생각되어 이와같이 결정되었습니다. 추후 경매 가능할 때 연락드리겠습니다.<br /><br />"
                        + "저희 케이옥션에 관심을 가져주셔서 다시 한 번 감사드리오며 궁금하신 사항이 있으시면 연락바랍니다.<br /><br />"
                        + "감사합니다.<br /><br />"
                        + "케이옥션 드림<br />"
                    : "안녕하세요. 케이옥션입니다.<br />"
                        + "보내주신 사진은 잘 받아보았습니다.<br /><br />"
                        + "귀하께서 보내주신 작품사진은 1차 사진 감정 후 작품 접수 여부가 결정이 되며, 이후 2차 실물 감정을 실시하여 최종 출품이 결정됩니다.<br />"
                        + "따라서 2차 접수 가능 여부에 대해서는 1차 심사 후 연락을 드리겠습니다.<br />"
                        + "시간이 다소 지체되더라도 양해바랍니다.<br /><br />"
                        + "저희 케이옥션에 관심 가져주셔서 감사드리오며, 궁금한 사항이 있으시면 연락바랍니다.<br><br />"
                        + "케이옥션 드림<br />";
            }
            return View();
        }

        #endregion

        #endregion

        #region # Excel Download #

        [HttpPost]
        public IActionResult Excel(string mode, string data)
        {
            using var workbook = new XLWorkbook();

            var requestParam = JObject.Parse(data);
            requestParam["page"] = "1";
            requestParam["page_size"] = "100000";

            // 엑셀 리스트 처리
            var listCodes = GetListCodes(mode);

            var worksheet = workbook.Worksheets.Add("Sheet1");
            var currentRow = 1;

            // 헤더 생성
            var currentColumn = 1;
            foreach (var item in listCodes)
            {
                if (item.Width > 0)
                {
                    worksheet.Column(currentColumn).Width = item.Width;
                }
                worksheet.Cell(currentRow, currentColumn++).Value = item.DisplayName;
            }

            // 데이터 쿼리
            if (mode.Equals("consign"))
            {
                requestParam["mode"] = "admin";
                var list = memberRepository.GetMemberConsigns(requestParam);
                ProcessData(listCodes, list, worksheet, currentRow);
            }
            else if (mode.Equals("inquiry"))
            {
                requestParam["mode"] = "admin";
                requestParam["reg_type"] = "M";
                var list = memberRepository.GetMemberInquiries(requestParam);
                ProcessData(listCodes, list, worksheet, currentRow);
            }
            else if (mode.Equals("daily_access_list"))
            {
                requestParam["mode"] = "list";
                var list = memberRepository.GetMemberDailyAccessStatuses(requestParam);
                ProcessData(listCodes, list, worksheet, currentRow);
            }
            else if (mode.Equals("daily_access_detail"))
            {
                requestParam["mode"] = "day_list";
                var list = memberRepository.GetMemberDailyAccessStatusesDetail(requestParam);
                ProcessData(listCodes, list, worksheet, currentRow);
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            workbook.Dispose();
            stream.Dispose();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "excel.xlsx");
        }

        private static IEnumerable<ListCode> GetListCodes(string mode)
        {
            List<ListCode> list = new();

            if (mode.Contains("consign"))
            {
                list.Add(new ListCode { Type = "consign", Code = "001", ColumnName = "No", DisplayName = "번호", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "002", ColumnName = "Artist", DisplayName = "작가명", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "003", ColumnName = "Title", DisplayName = "작품명", Width = 30 });
                list.Add(new ListCode { Type = "consign", Code = "004", ColumnName = "KofficeMemUid", DisplayName = "위탁자 고유번호(케이오피스)", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "005", ColumnName = "MemUid", DisplayName = "위탁자 고유번호(홈페이지)", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "006", ColumnName = "MemName", DisplayName = "위탁자 이름", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "007", ColumnName = "MngName", DisplayName = "위탁자 담당자", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "008", ColumnName = "MaterialName", DisplayName = "작품 재료", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "009", ColumnName = "WorkX", DisplayName = "작품 크기(가로)", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "010", ColumnName = "WorkY", DisplayName = "작품 크기(세로)", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "011", ColumnName = "WorkZ", DisplayName = "작품 크기(폭)", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "012", ColumnName = "Edition", DisplayName = "Edition", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "013", ColumnName = "MakeDate", DisplayName = "제작연도", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "014", ColumnName = "Desc", DisplayName = "작품설명", Width = 30 });
                list.Add(new ListCode { Type = "consign", Code = "015", ColumnName = "Etc", DisplayName = "기타", Width = 30 });
                list.Add(new ListCode { Type = "consign", Code = "016", ColumnName = "PricePurchase", DisplayName = "구입가(KRW)", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "017", ColumnName = "PriceDesired", DisplayName = "희망가(KRW)", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "018", ColumnName = "RegDate", DisplayName = "신청일자", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "019", ColumnName = "StateName", DisplayName = "상태", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "020", ColumnName = "ReceiptYn", DisplayName = "입고여부", Width = 10 });
                list.Add(new ListCode { Type = "consign", Code = "021", ColumnName = "RecommendedPrice", DisplayName = "권고가", Width = 20 });
                list.Add(new ListCode { Type = "consign", Code = "022", ColumnName = "Memo", DisplayName = "메모", Width = 30 });
            } 
            else if (mode.Contains("inquiry"))
            {
                list.Add(new ListCode { Type = mode, Code = "001", ColumnName = "Uid", DisplayName = "번호", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "002", ColumnName = "AucTitle", DisplayName = "경매명", Width = 40 });
                list.Add(new ListCode { Type = mode, Code = "003", ColumnName = "LotNum", DisplayName = "Lot", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "004", ColumnName = "ArtistName", DisplayName = "작가명", Width = 30 });
                list.Add(new ListCode { Type = mode, Code = "005", ColumnName = "Title", DisplayName = "작품명", Width = 40 });
                list.Add(new ListCode { Type = mode, Code = "006", ColumnName = "KofficeUid", DisplayName = "KO고객번호", Width = 20 });
                list.Add(new ListCode { Type = mode, Code = "007", ColumnName = "MemName", DisplayName = "고객명", Width = 20 });
                list.Add(new ListCode { Type = mode, Code = "008", ColumnName = "Contents", DisplayName = "질문내용", Width = 50 });
                list.Add(new ListCode { Type = mode, Code = "009", ColumnName = "RegDate", DisplayName = "등록일시", Width = 20 });
                list.Add(new ListCode { Type = mode, Code = "010", ColumnName = "ReplyCount", DisplayName = "답변", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "011", ColumnName = "MngName", DisplayName = "담당자", Width = 20 });
            }
            else if (mode.Contains("daily_access_list"))
            {
                list.Add(new ListCode { Type = mode, Code = "001", ColumnName = "Date", DisplayName = "날짜", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "002", ColumnName = "Name", DisplayName = "요일", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "003", ColumnName = "LoginCount", DisplayName = "접속건수", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "004", ColumnName = "LoginMemberCount", DisplayName = "접속자수", Width = 10 });
            }
            else if (mode.Contains("daily_access_detail"))
            {
                list.Add(new ListCode { Type = mode, Code = "001", ColumnName = "MemUid", DisplayName = "회원 고유번호", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "002", ColumnName = "MemName", DisplayName = "회원 이름", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "003", ColumnName = "Type", DisplayName = "접속유형", Width = 10 });
                list.Add(new ListCode { Type = mode, Code = "004", ColumnName = "RegDate", DisplayName = "최종 접속시간", Width = 10 });
            }

            return list;
        }

        private static void ProcessData(IEnumerable<ListCode> header, IEnumerable<object> dataList, IXLWorksheet workSheet, int rowIndex)
        {
            if (dataList == null) return;

            foreach (var item in dataList)
            {
                // 새행 추가
                rowIndex++;

                // 새열 인덱스
                var currentColumn = 1;

                // 셀 값 처리
                foreach (var code in header)
                {
                    workSheet.Cell(rowIndex, currentColumn++).Value = GetPropValue(item, code.ColumnName);
                }
            }
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName) == null ? "" : src.GetType().GetProperty(propName).GetValue(src, null);
        }

        #endregion
    }

    public class ListCode
    {
        public string Type { get; set; }

        public string Code { get; set; }

        public string ColumnName { get; set; }

        public string DisplayName { get; set; }

        public int Order { get; set; }

        public int Width { get; set; }
    }
}
