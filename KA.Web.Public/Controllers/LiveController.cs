using KA.Entities.Models.Live;
using KA.Repositories;
using KA.Web.Public.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Text;

namespace KA.Web.Public.Controllers
{
    public class LiveController : BaseController
    {   
        private readonly ILiveRepository _liveRepository;                
        private IMemoryCache _cache;        
        private LiveResult _jsonResult = new LiveResult();
        
        public LiveController(ILiveRepository liveRepository, IMemoryCache memoryCache)
        {
            _liveRepository = liveRepository;
            _cache = memoryCache;            
        }

        [Route("/Live/Major/{auc_num}")]
        public IActionResult Index(int auc_num, int auc_kind = 1)
        {   
            if(!base.IsLogin()) {
                return RedirectToAction("Login", "Member", new { returnUrl= $"/live/major/{auc_num}"});
            }



            ViewBag.AucKind = auc_kind;
            ViewBag.AucNum = auc_num;

            string ua = HttpContext.Request.Headers["User-Agent"].ToString().ToLower();
            //ViewBag.isMobile = ua.Contains("iphone") || ua.Contains("ipad") || ua.Contains("ipod") || ua.Contains("android");
            // ViewBag.isMobile = ua.Contains("iphone") || ua.Contains("ipad") || ua.Contains("ipod");
            // ViewBag.isAndroid = ua.Contains("android");

            ViewBag.isMobile = true;


            var jObject = usp_Live_auction_schedule_Select(new LiveRequest() { auc_kind = auc_kind, auc_num = auc_num });

            string auc_title = base.IsKor() ? "라이브 {{auc_title}} :: 케이옥션" : "Live {{auc_title}} :: K-Auction";
            if ((jObject["data"]["Table"] as JArray).Count > 0)
            {
                ViewData["Title"] = auc_title.Replace("{{auc_title}}", "-" + jObject["data"]["Table"][0]["auc_title"].ToString());
            } else {
                ViewData["Title"] = auc_title.Replace("{{auc_title}}", "");
            }
            
            return View(jObject["data"]);
        }

        [Route("/Live/Major/Test/{auc_num}")]
        public IActionResult Test(int auc_num, int auc_kind = 1)
        {
            if (!base.IsLogin())
            {
                return RedirectToAction("Login", "Member", new { returnUrl = $"/live/major/test/{auc_num}" });
            }

            return Redirect($"/liveAuction?auc_num={auc_num}&lang_cd={(!IsKor() ? "en" : "kr")}");
        }


        [Route("/Live/Conn")]
        public IActionResult Conn()
        {   
            return View();
        }


        [Route("/Live/Stream")]
        public IActionResult Stream()
        {
            string ua = HttpContext.Request.Headers["User-Agent"].ToString().ToLower();
            //ViewBag.isMobile = ua.Contains("iphone") || ua.Contains("ipad") || ua.Contains("ipod") || ua.Contains("android");
            ViewBag.isMobile = ua.Contains("iphone") || ua.Contains("ipad") || ua.Contains("ipod");
            ViewBag.isAndroid = ua.Contains("android");

            return View();
        }



        #region Util

        /// <summary>
        /// 파일경로
        /// </summary>
        /// <param name="img_file_name"></param>
        /// <param name="auc_num"></param>
        /// <param name="lot_num"></param>
        /// <returns></returns>
        public string GetImgPath(string img_file_name, string auc_num, string lot_num)
        {   
            string img_thumb_path = $"{Config.ImageDomain}/www/Work/{auc_num.PadLeft(4, '0')}/T/{img_file_name}";
            return img_thumb_path;
        }


        /// <summary>
        /// 경매일정
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetDate(DateTime dateTime)
        {
            string dt = base.IsKor() ? GetDateType(dateTime.ToString(), "A") : GetDateTypeEn(dateTime.ToString(), "A");
            string tm = base.IsKor() ? GetTimeType(dateTime.ToString("HH:mm:ss"), "") : GetTimeType(dateTime.ToString("HH:mm:ss"), "A");
            return dt + " " + tm;
        }




        private string GetDateType(string dt, string date_type)
        {
            string strDate = string.Empty;

            string week = "";

            try
            {
                week = DateTime.Parse(dt).DayOfWeek.ToString();

                switch (week)
                {
                    case "Monday":
                        week = "월";
                        break;
                    case "Tuesday":
                        week = "화";
                        break;
                    case "Wednesday":
                        week = "수";
                        break;
                    case "Thursday":
                        week = "목";
                        break;
                    case "Friday":
                        week = "금";
                        break;
                    case "Saturday":
                        week = "토";
                        break;
                    case "Sunday":
                        week = "일";
                        break;
                }
                if (date_type == "A")
                {
                    strDate = Convert.ToDateTime(dt).ToString("yyyy년 M월 d일") + "(" + week + ")";
                }
                else
                {
                    strDate = Convert.ToDateTime(dt).ToString("M월 d일") + "(" + week + ")";
                }
            }
            catch (Exception)
            {
                strDate = "잘못된 날짜 형식";
            }
            return strDate;
        }

        /// <summary> 시간 문자열 만들기. </summary>
        /// <param name="tm">시간</param>
        /// <returns>시간 문자열</returns>
        private string GetTimeType(string tm, string time_type)
        {
            string strTime = string.Empty;
            string strTimeType1 = string.Empty;
            string strTimeType2 = string.Empty;

            if (time_type == "A")
            {
                strTimeType1 = "AM";
                strTimeType2 = "PM";
            }
            else
            {
                strTimeType1 = "오전";
                strTimeType2 = "오후";
            }

            try
            {
                int intHour = Convert.ToInt32(tm.Split(':')[0].ToString());

                if (intHour == 0)
                {
                    strTime = strTimeType1 + " 12:" + tm.Split(':')[1].ToString();
                }
                else if (intHour < 12)
                {
                    strTime = strTimeType1 + " " + intHour.ToString() + ":" + tm.Split(':')[1].ToString();
                }
                else
                {
                    intHour = intHour - 12;
                    strTime = strTimeType2 + " " + intHour.ToString() + ":" + tm.Split(':')[1].ToString();
                }
            }
            catch (Exception)
            {
                strTime = "잘못된 시간 형식";
            }
            return strTime;
        }


        /// <summary> 날짜 문자열 만들기(영문). </summary>
        /// <param name="dt">날짜</param>
        /// <param name="date_type">날짜타입</param>
        /// <returns>날짜 문자열</returns>
        private string GetDateTypeEn(string dt, string date_type)
        {
            string strDate = string.Empty;

            string year = "";
            string month = "";
            string day = "";
            string week = "";

            try
            {
                week = DateTime.Parse(dt).DayOfWeek.ToString();

                switch (week)
                {
                    case "Monday":
                        week = "Mon";
                        break;
                    case "Tuesday":
                        week = "Tues";
                        break;
                    case "Wednesday":
                        week = "Wed";
                        break;
                    case "Thursday":
                        week = "Thurs";
                        break;
                    case "Friday":
                        week = "Fri";
                        break;
                    case "Saturday":
                        week = "Sat";
                        break;
                    case "Sunday":
                        week = "Sun";
                        break;
                }

                month = DateTime.Parse(dt).Month.ToString();
                switch (month)
                {
                    case "1":
                        month = "Jan";
                        break;
                    case "2":
                        month = "Feb";
                        break;
                    case "3":
                        month = "March";
                        break;
                    case "4":
                        month = "Apr";
                        break;
                    case "5":
                        month = "May";
                        break;
                    case "6":
                        month = "June";
                        break;
                    case "7":
                        month = "July";
                        break;
                    case "8":
                        month = "Aug";
                        break;
                    case "9":
                        month = "Sep";
                        break;
                    case "10":
                        month = "Oct";
                        break;
                    case "11":
                        month = "Nov";
                        break;
                    case "12":
                        month = "Dec";
                        break;
                }

                year = DateTime.Parse(dt).Year.ToString();
                day = DateTime.Parse(dt).Day.ToString();

                if (date_type == "A")
                {
                    strDate = string.Format("{0}, {1} {2} {3}", week, day, month, year);
                }
                else if (date_type == "B")
                {
                    strDate = string.Format("{0}, {1} {2}", week, day, month);
                }
                else if (date_type == "C")
                {
                    strDate = string.Format("{0}, {1} {2} {3} KST", week, day, month, Convert.ToDateTime(dt).ToString("HH:mm"));
                }
                else
                {
                    strDate = string.Format("{0}, {1} {2} {3} {4} KST", week, day, month, year, Convert.ToDateTime(dt).ToString("HH:mm:ss"));
                }
            }
            catch (Exception)
            {
                strDate = "잘못된 날짜 형식";
            }
            return strDate;
        }
        #endregion


        #region API

        /// <summary>
        /// 초기데이터 - 경매일정, 카테고리, 경매작품, 현재LOT정보
        /// </summary>
        /// <param name="_jsonRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_auction_schedule_Select")]
        public JObject usp_Live_auction_schedule_Select([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    mem_seq = _jsonRequest.mem_seq
            ,
                    lang_cd = _jsonRequest.lang_cd
            ,
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
                };

                var rtn = _liveRepository.GetDataSet("usp_Live_auction_schedule_Select", _param);

                foreach (DataRow dr in rtn.Tables[0].Rows)
                {
                    dr["auc_date"] = GetDate(Convert.ToDateTime(dr["auc_date"].ToString()));
                }

                foreach (DataRow dr in rtn.Tables[2].Rows)
                {
                    dr["img_file_name"] = GetImgPath(dr["img_file_name"].ToString(), _jsonRequest.auc_num.ToString(), dr["lot_num"].ToString());
                }

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = rtn;
            }
            catch(Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }
            
            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 경매작품리스트
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_auction_work_Select")]
        public JObject usp_Live_auction_work_Select([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    mem_seq = _jsonRequest.mem_seq
                ,
                    t_seq = _jsonRequest.t_seq
                ,
                    lang_cd = _jsonRequest.lang_cd
                ,
                    auc_kind = _jsonRequest.auc_kind
                ,
                    auc_num = _jsonRequest.auc_num
                ,
                    lot_num = _jsonRequest.lot_num
                };

                var rtn = _liveRepository.GetDataSet("usp_Live_auction_work_Select", _param);

                //foreach (DataRow dr in rtn.Tables[0].Rows)
                //{
                //    dr["img_file_name"] = GetImgPath(dr["img_file_name"].ToString(), _jsonRequest.auc_num.ToString(), dr["lot_num"].ToString());
                //}


                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = rtn;
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 현재진행 LOT 정보
        /// </summary>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_LotStat_Info_SelectForWAITORING")]
        public JObject usp_Live_Auc_LotStat_Info_SelectForWAITORING([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    auc_kind = _jsonRequest.auc_kind
                ,
                    auc_num = _jsonRequest.auc_num
                ,
                    lang_cd = _jsonRequest.lang_cd
                };

                var cacheEntry = _cache.GetOrCreate(_jsonRequest.auc_num.ToString(), entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                    return _liveRepository.GetDataSet("usp_Live_Auc_LotStat_Info_SelectForWAITORING", _param);
                });
               
                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = cacheEntry;
            }
            catch (Exception ex) 
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }
            finally
            {
                
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 최상위 응찰가 조회
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_Bidding_Hst_SelectForTop")]
        public JObject usp_Live_Auc_Bidding_Hst_SelectForTop([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    lang_cd = _jsonRequest.lang_cd
            ,
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
            ,
                    bid_type_cd = _jsonRequest.bid_type_cd
            ,
                    lot_num = _jsonRequest.lot_num
                };

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Auc_Bidding_Hst_SelectForTop", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 응찰내역 조회
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_Bidding_Hst_SelectForPage")]
        public JObject usp_Live_Auc_Bidding_Hst_SelectForPage([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    lang_cd = _jsonRequest.lang_cd
            ,
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
            ,
                    lot_num = _jsonRequest.lot_num
            ,
                    page_no = _jsonRequest.page_no
            ,
                    page_size = _jsonRequest.page_size
                };                                
                
                //DataSet cacheEntry = new DataSet();
                //if (!_cache.TryGetValue($"BidHst_{_jsonRequest.auc_num}", out cacheEntry))
                //{
                //    cacheEntry = _cache.Set($"BidHst_{_jsonRequest.auc_num}", _liveRepository.GetDataSet("usp_Live_Auc_Bidding_Hst_SelectForPage", _param));
                //}

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Auc_Bidding_Hst_SelectForPage", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 내 응찰내역 조회
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_Bidding_Hst_SelectForMyBid")]
        public JObject usp_Live_Auc_Bidding_Hst_SelectForMyBid([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
            ,
                    page_no = _jsonRequest.page_no
            ,
                    page_size = _jsonRequest.page_size
            ,
                    mem_seq = _jsonRequest.mem_seq
            ,
                    lang_cd = _jsonRequest.lang_cd
                };

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Auc_Bidding_Hst_SelectForMyBid", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        


        /// <summary>
        /// 응찰하기
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_Bidding_Hst_InsertProc")]
        public JObject usp_Live_Auc_Bidding_Hst_InsertProc([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
            ,
                    lot_num = _jsonRequest.lot_num
            ,
                    paddle_num = _jsonRequest.paddle_num
            ,
                    mem_seq = _jsonRequest.mem_seq
            ,
                    bid_type_cd = _jsonRequest.bid_type_cd ?? "ONL"
            ,
                    bid_stat_cd = _jsonRequest.bid_stat_cd ?? "BID"
            ,
                    bid_price = _jsonRequest.bid_price
                };

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Auc_Bidding_Hst_InsertProc", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 응찰가변경
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_Bidding_Hst_UpdateForBidPrice")]
        public JObject usp_Live_Auc_Bidding_Hst_UpdateForBidPrice([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
            ,
                    lot_num = _jsonRequest.lot_num
            ,
                    bid_type_cd = _jsonRequest.bid_type_cd ?? "FLD"
            ,
                    bid_stat_cd = _jsonRequest.bid_stat_cd ?? "BID"
            ,
                    bid_price = _jsonRequest.bid_price
                };

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Auc_Bidding_Hst_UpdateForBidPrice", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 나의 관심작품 조회
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Mem_Wish_Info_Select")]
        public JObject usp_Live_Mem_Wish_Info_Select([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    mem_seq = _jsonRequest.mem_seq
            ,
                    lang_cd = _jsonRequest.lang_cd
            ,
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
                };

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Mem_Wish_Info_Select", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 나의 관심작품 등록
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Mem_Wish_Info_Insert")]
        public JObject usp_Live_Mem_Wish_Info_Insert([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    mem_seq = _jsonRequest.mem_seq
           ,
                    auc_kind = _jsonRequest.auc_kind
           ,
                    work_seq = _jsonRequest.work_seq
                };


                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Mem_Wish_Info_Insert", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 호가단위변경
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_LotStat_Info_UpdateForIncreasePrice")]
        public JObject usp_Live_Auc_LotStat_Info_UpdateForIncreasePrice([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    auc_kind = _jsonRequest.auc_kind
            ,
                    auc_num = _jsonRequest.auc_num
            ,
                    lot_num = _jsonRequest.lot_num
            ,
                    bid_increase_price = _jsonRequest.bid_increase_price
                };

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Auc_LotStat_Info_UpdateForIncreasePrice", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        /// <summary>
        /// 공지조회
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/Live/usp_Live_Auc_Notice_Info_Select")]
        public JObject usp_Live_Auc_Notice_Info_Select([FromBody] LiveRequest _jsonRequest)
        {
            try
            {
                _jsonRequest.mem_seq = base.IsLogin() ? base.Uid : 0;
                _jsonRequest.lang_cd = base.IsKor() ? "ko" : "en";

                var _param = new
                {
                    lang_cd = _jsonRequest.lang_cd
                };

                _jsonResult.resultCd = "00";
                _jsonResult.resultMsg = "성공";
                _jsonResult.data = _liveRepository.GetDataSet("usp_Live_Auc_Notice_Info_Select", _param);
            }
            catch (Exception ex)
            {
                _jsonResult.resultCd = "99";
                _jsonResult.resultMsg = ex.Message;
            }

            return JObject.FromObject(_jsonResult);
        }


        #endregion

    }



}
