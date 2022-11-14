using Dapper;
using KA.Entities.Helpers;
using KA.Entities.Models.Auction;
using KA.Entities.Models.Common;
using KA.Entities.Models.Main;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace KA.Repositories
{
    public interface IMainRepository
    {
        /// <summary>
        /// 상단 메뉴 현재 경매 정보 목록 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuctionSchedule> GetCurrentAuctions();

        /// <summary>
        /// 배너 정보 조회 처리 함수
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        public IEnumerable<Banner> GetMainBannerList(string mode, int page, string lang = "K");

        /// <summary>
        /// 배너 정보 조회 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public IEnumerable<Banner> GetMainBannerList(JObject json);

        /// <summary>
        /// 배너 상세 정보 처리 함수
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Banner GetBanner(string uid);

        /// <summary>
        /// 배너 저장/수정 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DbResult SetBanner(JObject json);

        /// <summary>
        /// 배너 버튼 저장/수정 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DbResult SetBannerButton(JObject json);

        /// <summary>
        /// 배너 버튼 정보 처리 함수
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IEnumerable<BannerButton> GetBannerButtons(string mode, int uid);

        /// <summary>
        /// 작품 하이라이트 목록 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuctionWork> GetWorkHighlight(string mode, int memUid = 0);

        /// <summary>
        /// 작품 하이라트 변경 처리 함수
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="workUid"></param>
        /// <returns></returns>
        public DbResult SetWorkHighlight(string mode, int workUid);

        /// <summary>
        /// 공지사항 목록 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notice> GetNotices(string mode, string lang = "", string page = "1", string activeFlag = "", string filter = "");

        /// <summary>
        /// 공지사항 목록 처리 함수
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notice> GetNotices(JObject json);

        /// <summary>
        /// 공지사항 상제 정보 처리 함수
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Notice GetNotice(string uid);

        /// <summary>
        /// 공지사항 저장/수정 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DbResult SetNotice(JObject json);

        /// <summary>
        /// 경매 일정 목록 처리 함수
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="aucKind"></param>
        /// <returns></returns>
        public IEnumerable<Schedule> GetAuctionSchedules(string mode, string aucKind = "", JObject json = null);

        /// <summary>
        /// 경매 일정 정보 저장 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DbResult SetAuctionSchedule(JObject json);

        /// <summary>
        /// 외부 컨텐츠 데이터 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public IEnumerable<CrawlingData> GetCrawlingDatas(JObject json);

        /// <summary>
        /// 외부 컨텐츠 데이터 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DbResult SetCrawlingData(JObject json);

        /// <summary>
        /// 메인 컨텐츠 조회 이력 처리 함수
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public DbResult SetContentReadHst(JObject json);

        /// <summary>
        /// 추천 검색어 목록 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public IEnumerable<SearchTerm> GetSearchTerms(JObject json);

        /// <summary>
        /// 추천 검색어 데이터 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DbResult SetSearchTerm(JObject json);
    }

    public class MainRepository : BaseRepository, IMainRepository
    {
        public MainRepository(IConfiguration configuration)
        {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");
        }

        public IEnumerable<AuctionSchedule> GetCurrentAuctions()
        {
            return GetResults<AuctionSchedule>("usp_main_current_auction_select", null);
        }

        public IEnumerable<Banner> GetMainBannerList(string mode, int page, string lang = "K")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@page", page);
            parameters.Add("@lang", lang);
            return GetResults<Banner>("usp_main_banner_select", parameters);
        }

        public IEnumerable<Banner> GetMainBannerList(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            parameters.Add("@lang", JsonHelper.GetString(json, "lang"));
            parameters.Add("@active_flag", JsonHelper.GetString(json, "active_flag"));
            return GetResults<Banner>("usp_main_banner_select", parameters);
        }

        public Banner GetBanner(string uid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "detail");
            parameters.Add("@uid", uid);
            return GetSingleResult<Banner>("usp_main_banner_select", parameters);
        }

        public DbResult SetBanner(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@title", JsonHelper.GetString(json, "title"));
            parameters.Add("@image_file_name", JsonHelper.GetString(json, "image_file_name"));
            parameters.Add("@image_file_name_en", JsonHelper.GetString(json, "image_file_name_en"));
            parameters.Add("@image_bg_color", JsonHelper.GetString(json, "image_bg_color"));
            parameters.Add("@mobile_image_file_name", JsonHelper.GetString(json, "mobile_image_file_name"));
            parameters.Add("@mobile_image_file_name_en", JsonHelper.GetString(json, "mobile_image_file_name_en"));
            parameters.Add("@layer_title", JsonHelper.GetString(json, "layer_title"));
            parameters.Add("@layer_title_en", JsonHelper.GetString(json, "layer_title_en"));
            parameters.Add("@layer_sub_title", JsonHelper.GetString(json, "layer_sub_title"));
            parameters.Add("@layer_sub_title_en", JsonHelper.GetString(json, "layer_sub_title_en"));
            parameters.Add("@layer_content", JsonHelper.GetString(json, "layer_content"));
            parameters.Add("@layer_content_en", JsonHelper.GetString(json, "layer_content_en"));
            parameters.Add("@start_date", JsonHelper.GetString(json, "start_date"));
            parameters.Add("@end_flag", JsonHelper.GetString(json, "end_flag"));
            parameters.Add("@end_date", JsonHelper.GetString(json, "end_date"));
            parameters.Add("@end_auc_uid", JsonHelper.GetString(json, "end_auc_uid", "0"));
            parameters.Add("@end_auc_opt", JsonHelper.GetString(json, "end_auc_opt"));
            parameters.Add("@end_auc_val", JsonHelper.GetString(json, "end_auc_val", "0"));
            parameters.Add("@use_layer", JsonHelper.GetString(json, "use_layer"));
            parameters.Add("@use_flag", JsonHelper.GetString(json, "use_flag"));
            parameters.Add("@use_link_work", JsonHelper.GetString(json, "use_link_work"));
            parameters.Add("@link_work_uid", JsonHelper.GetString(json, "link_work_uid", "0"));
            parameters.Add("@order", JsonHelper.GetString(json, "order", "1"));
            parameters.Add("@reg_uid", JsonHelper.GetString(json, "reg_uid", "0"));
            return GetSingleResult<DbResult>("usp_main_banner_update", parameters);
        }

        public IEnumerable<BannerButton> GetBannerButtons(string mode, int uid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@uid", uid);
            return GetResults<BannerButton>("usp_main_banner_button_select", parameters);
        }

        public DbResult SetBannerButton(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            foreach (var item in json)
            {
                parameters.Add("@" + item.Key, JsonHelper.GetString(json, item.Key));
            }
            return GetSingleResult<DbResult>("usp_main_banner_button_update", parameters);
        }

        public IEnumerable<AuctionWork> GetWorkHighlight(string mode, int memUid = 0)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@mem_uid", memUid);
            return GetResults<AuctionWork>("usp_main_work_highlight_select", parameters);
        }

        public DbResult SetWorkHighlight(string mode, int workUid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@work_uid", workUid);
            return GetSingleResult<DbResult>("usp_main_work_highlight_update", parameters);
        }

        public IEnumerable<Notice> GetNotices(string mode, string lang = "", string page = "1", string activeFlag = "", string filter = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@lang", lang);
            parameters.Add("@page", page);
            parameters.Add("@active_flag", activeFlag);
            parameters.Add("@filter", filter);
            return GetResults<Notice>("usp_main_notice_select", parameters);
        }

        public IEnumerable<Notice> GetNotices(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@lang", JsonHelper.GetString(json, "lang"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@active_flag", JsonHelper.GetString(json, "active_flag"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            return GetResults<Notice>("usp_main_notice_select", parameters);
        }

        public Notice GetNotice(string uid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "detail");
            parameters.Add("@uid", uid);
            return GetSingleResult<Notice>("usp_main_notice_select", parameters);
        }

        public DbResult SetNotice(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            foreach (var item in json)
            {
                parameters.Add("@" + item.Key, JsonHelper.GetString(json, item.Key));
            }
            return GetSingleResult<DbResult>("usp_main_notice_update", parameters);
        }

        public IEnumerable<Schedule> GetAuctionSchedules(string mode, string aucKind = "", JObject json = null)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@auc_kind", aucKind);
            if (json != null)
            {
                parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
                parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            }
            return GetResults<Schedule>("usp_main_auction_schedule_select", parameters);
        }

        public DbResult SetAuctionSchedule(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind"));
            parameters.Add("@auc_num", JsonHelper.GetString(json, "auc_num", "0"));
            parameters.Add("@use_yn", JsonHelper.GetString(json, "use_yn"));
            parameters.Add("@reg_uid", JsonHelper.GetString(json, "reg_uid"));
            return GetSingleResult<DbResult>("usp_main_auction_schedule_update", parameters);
        }

        public IEnumerable<CrawlingData> GetCrawlingDatas(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            return GetResults<CrawlingData>("usp_crawling_data_select", parameters);
        }

        public DbResult SetCrawlingData(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@id", JsonHelper.GetString(json, "id"));
            parameters.Add("@link", JsonHelper.GetString(json, "link"));
            parameters.Add("@image_path", JsonHelper.GetString(json, "image_path"));
            parameters.Add("@title", JsonHelper.GetString(json, "title"));
            parameters.Add("@date", JsonHelper.GetString(json, "date"));
            parameters.Add("@use_yn", JsonHelper.GetString(json, "use_yn"));
            parameters.Add("@etc1", JsonHelper.GetString(json, "etc1"));
            parameters.Add("@etc2", JsonHelper.GetString(json, "etc2"));
            parameters.Add("@etc3", JsonHelper.GetString(json, "etc3"));
            return GetSingleResult<DbResult>("usp_crawling_data_update", parameters);
        }

        public DbResult SetContentReadHst(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@target", JsonHelper.GetString(json, "target", "0"));
            parameters.Add("@url", JsonHelper.GetString(json, "url"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@token", JsonHelper.GetString(json, "token"));
            parameters.Add("@ip", JsonHelper.GetString(json, "ip"));
            parameters.Add("@user_agent", JsonHelper.GetString(json, "user_agent"));
            return GetSingleResult<DbResult>("usp_content_read_hst_update", parameters);
        }

        public IEnumerable<SearchTerm> GetSearchTerms(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            parameters.Add("@lang", JsonHelper.GetString(json, "lang"));
            return GetResults<SearchTerm>("usp_main_search_term_select", parameters);
        }

        public DbResult SetSearchTerm(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid"));
            parameters.Add("@term", JsonHelper.GetString(json, "term"));
            parameters.Add("@lang", JsonHelper.GetString(json, "lang"));
            parameters.Add("@reg_uid", JsonHelper.GetString(json, "reg_uid", "0"));
            return GetSingleResult<DbResult>("usp_main_search_term_update", parameters);
        }
    }
}
