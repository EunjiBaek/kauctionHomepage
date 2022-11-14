using Dapper;
using KA.Entities.Helpers;
using KA.Entities.Models.Auction;
using KA.Entities.Models.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;

namespace KA.Repositories
{
    public interface IAuctionRepository
    {
        /// <summary>
        /// 경매 일정 목록 정보를 가져오는 함수
        /// </summary>
        /// <returns></returns>
        IEnumerable<AuctionSchedule> GetAuctionSchedules(string aucKind, string mode = "list", JObject json = null);

        /// <summary>
        /// 경매 일정 상세 정보를 가져오는 함수
        /// </summary>
        /// <param name="aucSeq"></param>
        /// <returns></returns>
        AuctionSchedule GetAuctionSchedule(int aucUid, string mode = "detail");

        /// <summary>
        /// 경매 일정 상세 정보를 가져오는 함수
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <param name="mode">mode가 work 인 경우 tbl_auction_work 테이블의 work_seq 로 경매일정 조회</param>
        /// <returns></returns>
        AuctionSchedule GetAuctionSchedule(string aucKind, int aucNum, int memUid, string mode = "");

        /// <summary>
        /// 해당 경매의 작품 목록 정보를 가져오는 함수
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <returns></returns>
        IEnumerable<AuctionWork> GetAuctionWorks(string aucKind, int aucNum, int memUid, JObject json = null, string mode = "list");

        /// <summary>
        /// 해당 경매의 작품 아티스트 정보를 가져오는 함수
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <returns></returns>
        IEnumerable<AuctionWork> GetAuctionWorkArtists(JObject json);

        /// <summary>
        /// 전체 검색 작품 정보 가져오는 함수
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        IEnumerable<AuctionWork> GetAuctionWorksSearch(string aucKind, int memUid, JObject json = null, string mode = "search");

        /// <summary>
        /// 낙찰 통보서 정보 가져오기
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        IEnumerable<AuctionNak> GetAuctionBidsNak(string aucKind, int aucNum, int memUid, string mode = "nak_info");

        /// <summary>
        /// 해당 경매의 작품 상세 정보를 가져오는 함수
        /// </summary>
        /// <param name="workSeq"></param>
        /// <returns></returns>
        AuctionWork GetAuctionWork(int uid, int memUid = 0);

        /// <summary>
        /// 해당 경매의 작품 상세 정보를 가져오는 함수 (경매유형, 경매회차, 랏번호로 조회)
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <param name="lotNum"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        AuctionWork GetAuctionWork(string aucKind, int aucNum, int lotNum, int memUid = 0);

        /// <summary>
        /// 해당 경매 작품 변경 처리 함수
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="workUid"></param>
        /// <returns></returns>
        string SetAuctionWork(string mode, int workUid);

        /// <summary>
        /// 해당 경매 작품 변경 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string SetAuctionWork(JObject json);

        /// <summary>
        /// 작품의 이미지 목록 정보를 가져오는 함수
        /// </summary>
        IEnumerable<AuctionWorkImage> GetAuctionWorkImages(int uid);

        /// <summary>
        /// 해당 경매의 작품 상세 정보를 가져오는 함수
        /// </summary>
        /// <param name="workSeq"></param>
        /// <returns></returns>
        IEnumerable<AuctionWorkCondition> GetAuctionWorkCondition(int uid, string lang_type);

        /// <summary>
        /// 작품의 응찰 현황 정보를 가져오는 함수
        /// </summary>
        /// <param name="workSeq"></param>
        /// <returns></returns>
        IEnumerable<AuctionBid> GetAuctionBids(int workUid, int memUid = -1);

        /// <summary>
        /// 작품의 응찰 현황 정보를 가져오는 함수(페이징 처리)
        /// </summary>
        /// <param name="workUid"></param>
        /// <param name="memUid"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<AuctionBid> GetAuctionBidsByAdmin(int workUid, int memUid = -1, int page = 1, int pageSize = 20);

        IEnumerable<AuctionBid> GetAuctionBidsByAdmin(JObject json);

        /// <summary>
        /// 작품의 내 응찰 현황 정보를 가져오는 함수
        /// </summary>
        /// <param name="workUid"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        IEnumerable<AuctionBid> GetAuctionMyBids(int workUid, int memUid = -1);

        /// <summary>
        /// 사용자 별 응찰 정보가 있는 경매 일정 정보를 가져오는 함수
        /// </summary>
        /// <param name="memUid"></param>
        /// <returns></returns>
        IEnumerable<AuctionWorkByUser> GetAuctionWorkByUserBid(JObject json);

        /// <summary>
        /// 경매 별 상세 응찰 정보를 가져오는 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<AuctionWork> GetAuctionBidWork(JObject json);

        /// <summary>
        /// 응찰 처리 함수
        /// </summary>
        /// <param name="workSeq"></param>
        /// <param name="currentPrice"></param>
        /// <param name="bidPrice"></param>
        /// <param name="memUid"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        DbResult SetBid(JObject json);

        /// <summary>
        /// 응찰 처리 함수
        /// </summary>
        /// <param name="mode"></param>      
        /// <param name="bid_uid"></param>      
        /// <param name="mng_uid"></param>      
        /// <returns></returns>
        DbResult SetBidByAdmin(JObject json);

        /// <summary>
        /// 관리자 낙찰 처리 함수
        /// </summary>
        /// <param name="mode"></param>      
        /// <param name="bid_uid"></param>      
        /// <param name="mng_uid"></param>      
        /// <returns></returns>
        DbResult SetBidNakByAdmin(JObject json);

        /// <summary>
        /// 작가의 연관작품 정보 처리 함수
        /// </summary>
        /// <param name="artistUid"></param>
        /// <param name="workSeq"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        IEnumerable<AuctionWork> GetArtistWorks(int artistUid, int workSeq, int memUid = 0);

        /// <summary>
        /// 작가명으로 검색 (자동완성 처리)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<AuctionWork> GetArtists(JObject json);

        /// <summary>
        /// 배송비 정보 처리 함수
        /// </summary>
        /// <param name="aucKind"></param>
        /// <param name="aucNum"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        DeliveryPrice GetDeliveryPrice(string aucKind, int aucNum, int memUid);

        /// <summary>
        /// 작품 별 자동응찰 가격 정보를 가져오는 함수
        /// </summary>
        /// <param name="workUid"></param>
        /// <returns></returns>
        IEnumerable<AuctionPriceBid> GetAuctionBidPre(int workUid, string mode = "", int memUid = 0);

        /// <summary>
        /// 작품의 추가 정보를 가져오는 함수
        /// </summary>
        /// <param name="workUid"></param>
        /// <param name="kofficeUid"></param>
        /// <returns></returns>
        IEnumerable<AuctionWorkAdditionalInfo> GetAuctionWorkAdditionalInfos(string code, int workUid, int kofficeUid);

        /// <summary>
        /// 작가의 사진정보를 가져오는 함수
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        IEnumerable<Image> GetImages(int uid);

        /// <summary>
        /// 메이저 서면/전화 응찰 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetAuctionBidPre(JObject json);

        /// <summary>
        /// [KOFFICE] 메이저 온라인응찰 신청 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string SetAuctionLiveRequest(JObject json);

        /// <summary>
        /// [KOFFICE] 메이저 온라인 라이브 신청 목록 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        AuctionLiveRequest GetAuctionLiveRequestInfo(JObject json);

        /// <summary>
        /// [KOFFICE] 메이저 온라인 라이브 신청 목록 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<AuctionBid> GetMemberBidPreinfo(int workUid, int memUid);

        /// <summary>
        /// 경매 작품 검색 이력 처리 함수
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="memUid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        DbResult SetAuctionWorkSearchHst(JObject json);

        /// <summary>
        /// 경매 작품 검색 이력 목록 처리 함수
        /// </summary>
        /// <returns></returns>
        IEnumerable<AuctionWorkSearchHistory> GetAuctionWorkSearchHistories(JObject json);

        /// <summary>
        /// 경매 낙찰 통보서 정보 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<AuctionReport> GetAuctionReports(JObject json);

        /// <summary>
        /// 작품 정보 (케이오피스)
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        AuctionWorkKoffice GetAuctionWorkKoffice(int uid, int memUid = 0);

        IEnumerable<AuctionBidProc> GetAuctionBidProcs(JObject jobject);
    }

    public class AuctionRepository : BaseRepository, IAuctionRepository
    {
        public AuctionRepository(IConfiguration configuration)
        {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");
        }

        public IEnumerable<AuctionSchedule> GetAuctionSchedules(string aucKind, string mode = "list", JObject json = null)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", mode, DbType.String, ParameterDirection.Input, 20);
            parameter.Add("@auc_kind", aucKind, DbType.String, ParameterDirection.Input, 1);
            if (json != null)
            {
                parameter.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"), DbType.Int32, ParameterDirection.Input);
                parameter.Add("@page", JsonHelper.GetString(json, "page", "1"), DbType.Int32, ParameterDirection.Input);
                parameter.Add("@sort_column", JsonHelper.GetString(json, "sort_column", ""), DbType.String, ParameterDirection.Input, 20);
                parameter.Add("@sort_option", JsonHelper.GetString(json, "sort_option", ""), DbType.String, ParameterDirection.Input, 5);
                parameter.Add("@search", JsonHelper.GetString(json, "search", ""), DbType.String, ParameterDirection.Input, 50);
                parameter.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"), DbType.Int32, ParameterDirection.Input);
            }
            return GetResults<AuctionSchedule>("usp_auction_schedule_select", parameter);
        }

        public AuctionSchedule GetAuctionSchedule(string aucKind, int aucNum, int memUid, string mode = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", string.IsNullOrWhiteSpace(mode) ? "detail" : mode);
            parameters.Add("@auc_kind", aucKind);
            if (mode.Equals("work"))
            {
                parameters.Add("@work_seq", aucNum);
            }
            else
            {
                parameters.Add("@auc_num", aucNum);
            }
            parameters.Add("@mem_uid", memUid);
            return GetSingleResult<AuctionSchedule>("usp_auction_schedule_select", parameters);
        }

        public AuctionSchedule GetAuctionSchedule(int aucUid, string mode = "detail")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@auc_uid", aucUid);
            return GetSingleResult<AuctionSchedule>("usp_auction_schedule_select", parameters);
        }

        public IEnumerable<AuctionWork> GetAuctionWorks(string aucKind, int aucNum, int memUid, JObject json = null, string mode = "list")
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", mode, DbType.String, ParameterDirection.Input, 20);
            parameter.Add("@auc_kind", aucKind, DbType.String, ParameterDirection.Input, 1);
            parameter.Add("@auc_num", aucNum, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@mem_uid", memUid, DbType.Int32, ParameterDirection.Input);
            if (json != null)
            {
                parameter.Add("@page", JsonHelper.GetString(json, "page", "1"), DbType.Int32, ParameterDirection.Input);
                parameter.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"), DbType.Int32, ParameterDirection.Input);
                parameter.Add("@search", JsonHelper.GetString(json, "search"), DbType.String, ParameterDirection.Input, 50);
                parameter.Add("@work_type", JsonHelper.GetString(json, "work_type"), DbType.String, ParameterDirection.Input, 1000);
                parameter.Add("@price_from", JsonHelper.GetString(json, "price_from", "-1"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_to", JsonHelper.GetString(json, "price_to", "-1"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@sort_column", JsonHelper.GetString(json, "sort_column"), DbType.String, ParameterDirection.Input, 20);
                parameter.Add("@sort_option", JsonHelper.GetString(json, "sort_option"), DbType.String, ParameterDirection.Input, 5);
                parameter.Add("@token", JsonHelper.GetString(json, "token"), DbType.String, ParameterDirection.Input, 50);
                parameter.Add("@artist", JsonHelper.GetString(json, "artist"), DbType.String, ParameterDirection.Input, 1000);
                parameter.Add("@material", JsonHelper.GetString(json, "material"), DbType.String, ParameterDirection.Input, 100);
                parameter.Add("@title", JsonHelper.GetString(json, "title"), DbType.String, ParameterDirection.Input, 150);
                parameter.Add("@price_start_from", JsonHelper.GetString(json, "price_start_from", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_start_to", JsonHelper.GetString(json, "price_start_to", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_low_from", JsonHelper.GetString(json, "price_estimated_low_from", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_low_to", JsonHelper.GetString(json, "price_estimated_low_to", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_high_from", JsonHelper.GetString(json, "price_estimated_high_from", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_high_to", JsonHelper.GetString(json, "price_estimated_high_to", "0"), DbType.Decimal, ParameterDirection.Input);
            }
            return GetResults<AuctionWork>("usp_auction_work_select", parameter);
        }

        public IEnumerable<AuctionWork> GetAuctionWorkArtists(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "artist");
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind"));
            parameters.Add("@auc_num", JsonHelper.GetString(json, "auc_num", "0"));
            parameters.Add("@sort_column", JsonHelper.GetString(json, "sort_column"));
            parameters.Add("@sort_option", JsonHelper.GetString(json, "sort_option"));
            return GetResults<AuctionWork>("usp_auction_work_select", parameters);
        }

        public IEnumerable<AuctionWork> GetAuctionWorksSearch(string aucKind, int memUid, JObject json = null, string mode = "search")
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", mode, DbType.String, ParameterDirection.Input, 20);
            parameter.Add("@auc_kind", aucKind, DbType.String, ParameterDirection.Input, 10);            
            parameter.Add("@mem_uid", memUid, DbType.Int32, ParameterDirection.Input);
            if (json != null)
            {
                parameter.Add("@page", JsonHelper.GetString(json, "page", "1"), DbType.Int32, ParameterDirection.Input);
                parameter.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"), DbType.Int32, ParameterDirection.Input);
                parameter.Add("@search", JsonHelper.GetString(json, "search"), DbType.String, ParameterDirection.Input, 50);
                parameter.Add("@auction_range", JsonHelper.GetString(json, "auction_range"), DbType.String, ParameterDirection.Input, 1);
                parameter.Add("@material", JsonHelper.GetString(json, "material"), DbType.String, ParameterDirection.Input, 100);
                parameter.Add("@title", JsonHelper.GetString(json, "title"), DbType.String, ParameterDirection.Input, 150);
                parameter.Add("@price_start_from", JsonHelper.GetString(json, "price_start_from", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_start_to", JsonHelper.GetString(json, "price_start_to", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_low_from", JsonHelper.GetString(json, "price_estimated_low_from", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_low_to", JsonHelper.GetString(json, "price_estimated_low_to", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_high_from", JsonHelper.GetString(json, "price_estimated_high_from", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@price_estimated_high_to", JsonHelper.GetString(json, "price_estimated_high_to", "0"), DbType.Decimal, ParameterDirection.Input);
                parameter.Add("@sort", JsonHelper.GetString(json, "sort"), DbType.String, ParameterDirection.Input, 10);
            }
            return GetResults<AuctionWork>("usp_auction_work_search_select_v2", parameter);
        }

        public IEnumerable<AuctionNak> GetAuctionBidsNak(string aucKind, int aucNum, int memUid, string mode = "nak_info")
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@mode", mode, DbType.String, ParameterDirection.Input, 20);
            parameter.Add("@auc_kind", aucKind, DbType.String, ParameterDirection.Input, 1);
            parameter.Add("@auc_num", aucNum, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@mem_uid", memUid, DbType.Int32, ParameterDirection.Input);
            
            return GetResults<AuctionNak>("usp_auction_bid_select", parameter);
        }

        public string SetAuctionWork(string mode, int workUid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode, DbType.String, ParameterDirection.Input, 20);
            parameters.Add("@work_uid", workUid, DbType.Int32);
            return GetSingleResult<string>("usp_auction_work_update", parameters);
        }

        public string SetAuctionWork(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"), DbType.String, ParameterDirection.Input, 20);
            parameters.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"), DbType.Int32);
            parameters.Add("@value", JsonHelper.GetString(json, "value"), DbType.String, ParameterDirection.Input, 1);
            parameters.Add("@section_uid", JsonHelper.GetString(json, "section_uid", "0"), DbType.Int32);
            parameters.Add("@auction_uid", JsonHelper.GetString(json, "auction_uid", "0"), DbType.Int32);
            parameters.Add("@exhi_yn", JsonHelper.GetString(json, "exhi_yn"), DbType.String, ParameterDirection.Input, 1);
            parameters.Add("@mng_uid", JsonHelper.GetString(json, "mng_uid", "0"), DbType.Int32);
            return GetSingleResult<string>("usp_auction_work_update", parameters);
        }

        public AuctionWork GetAuctionWork(int uid, int memUid = 0)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", "detail");
            parameter.Add("@uid", uid);
            parameter.Add("@mem_uid", memUid);
            return GetSingleResult<AuctionWork>("usp_auction_work_select", parameter);
        }

        public AuctionWork GetAuctionWork(string aucKind, int aucNum, int lotNum, int memUid = 0)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", "detail");
            parameter.Add("@auc_kind", aucKind);
            parameter.Add("@auc_num", aucNum);
            parameter.Add("@lot_num", lotNum);
            parameter.Add("@mem_uid", memUid);
            return GetSingleResult<AuctionWork>("usp_auction_work_select", parameter);
        }


        public IEnumerable<AuctionWorkCondition> GetAuctionWorkCondition(int uid, string lang_type)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", "condition");
            parameter.Add("@uid", uid);
            parameter.Add("@lang_type", lang_type);
            return GetResults<AuctionWorkCondition>("usp_auction_work_select", parameter);
        }


        public IEnumerable<AuctionWorkImage> GetAuctionWorkImages(int uid)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", "list");
            parameter.Add("@uid", uid);
            return GetResults<AuctionWorkImage>("usp_auction_work_img_select", parameter);
        }

        public IEnumerable<AuctionBid> GetAuctionBids(int workUid, int memUid = -1)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "list");
            parameters.Add("@work_uid", workUid);
            parameters.Add("@mem_uid", memUid);
            return GetResults<AuctionBid>("usp_auction_bid_select", parameters);
        }

        public IEnumerable<AuctionBid> GetAuctionBidsByAdmin(int workUid, int memUid = -1, int page = 1, int pageSize = 20)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "admin");
            parameters.Add("@page", page);
            parameters.Add("@page_size", pageSize);
            parameters.Add("@work_uid", workUid);
            parameters.Add("@mem_uid", memUid);
            return GetResults<AuctionBid>("usp_auction_bid_select", parameters);
        }

        public IEnumerable<AuctionBid> GetAuctionBidsByAdmin(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "admin");
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            parameters.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@bid_uid", JsonHelper.GetString(json, "bid_uid", "0"));
            return GetResults<AuctionBid>("usp_auction_bid_select", parameters);
        }

        public IEnumerable<AuctionBid> GetAuctionMyBids(int workUid, int memUid = -1)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "user_bid_list");
            parameters.Add("@work_uid", workUid);
            parameters.Add("@mem_uid", memUid);
            return GetResults<AuctionBid>("usp_auction_bid_select", parameters);
        }

        public IEnumerable<AuctionWorkByUser> GetAuctionWorkByUserBid(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode", ""));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "-1"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "10"));
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind", ""));
            var aucKinds = JsonHelper.GetArray(json, "auc_kinds");
            parameters.Add("@auc_kinds", aucKinds.Length > 0 ? string.Join(" ", aucKinds) : string.Empty);
            parameters.Add("@start_date", JsonHelper.GetDateTime(json, "start_date"));
            parameters.Add("@end_date", JsonHelper.GetDateTime(json, "end_date"));
            parameters.Add("@nak_yn", JsonHelper.GetString(json, "nak_yn", string.Empty));
            parameters.Add("@search", JsonHelper.GetString(json, "search", string.Empty));
            return GetResults<AuctionWorkByUser>("usp_auction_bid_select", parameters);
        }

        public IEnumerable<AuctionWork> GetAuctionBidWork(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "bid_detail");
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "-1"));
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind", ""));
            parameters.Add("@auc_num", JsonHelper.GetString(json, "auc_num", "0"));
            return GetResults<AuctionWork>("usp_auction_bid_select", parameters);
        }

        public IEnumerable<AuctionWork> GetArtistWorks(int artistUid, int workSeq, int memUid = 0)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@artist_uid", artistUid);
            parameter.Add("@work_seq", workSeq);
            parameter.Add("@mem_uid", memUid);
            return GetResults<AuctionWork>("usp_artist_work_select", parameter);
        }

        public IEnumerable<AuctionWork> GetArtists(JObject json)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameter.Add("@artist_uid", JsonHelper.GetString(json, "artist_uid", "0"));
            parameter.Add("@work_seq", JsonHelper.GetString(json, "work_seq", "0"));
            parameter.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameter.Add("@keyword", JsonHelper.GetString(json, "keyword"));
            return GetResults<AuctionWork>("usp_artist_work_select", parameter);
        }

        public DbResult SetBid(JObject json)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", JsonHelper.GetString(json, "mode", ""));
            parameter.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"));
            parameter.Add("@price_bid", JsonHelper.GetString(json, "price_bid", "0"));
            parameter.Add("@price_bid_pre", JsonHelper.GetString(json, "price_bid_pre", "0"));
            parameter.Add("@price_max", JsonHelper.GetString(json, "price_max", "0"));
            parameter.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameter.Add("@reg_ip", JsonHelper.GetString(json, "reg_ip", ""));
            parameter.Add("@user_agent", JsonHelper.GetString(json, "user_agent"));
            return GetSingleResult<DbResult>("usp_auction_bid_update", parameter);
        }

        public DbResult SetBidByAdmin(JObject json)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", JsonHelper.GetString(json, "mode", ""));
            parameter.Add("@bid_uid", JsonHelper.GetString(json, "bid_uid", "0"));            
            parameter.Add("@mng_uid", JsonHelper.GetString(json, "mng_uid", ""));
            return GetSingleResult<DbResult>("usp_admin_auction_bid_update", parameter);
        }

        public DbResult SetBidNakByAdmin(JObject json)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", JsonHelper.GetString(json, "mode", ""));
            parameter.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"));
            parameter.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameter.Add("@price_bid", JsonHelper.GetString(json, "price_bid", "0"));
            parameter.Add("@mng_uid", JsonHelper.GetString(json, "mng_uid", "0"));
            parameter.Add("@reg_ip", JsonHelper.GetString(json, "reg_ip", "0"));
            return GetSingleResult<DbResult>("usp_admin_auction_bid_nak_update", parameter);
        }

        public DeliveryPrice GetDeliveryPrice(string aucKind, int aucNum, int memUid)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@auc_kind", aucKind);
            parameter.Add("@auc_num", aucNum);
            parameter.Add("@mem_uid", memUid);
            return GetSingleResult<DeliveryPrice>("usp_delivery_price_select", parameter);
        }

        public IEnumerable<AuctionPriceBid> GetAuctionBidPre(int workUid, string mode = "", int memUid = 0)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@work_uid", workUid);
            parameters.Add("@mem_uid", memUid);
            return GetResults<AuctionPriceBid>("usp_auction_bid_pre_select", parameters);
        }

        public IEnumerable<AuctionWorkAdditionalInfo> GetAuctionWorkAdditionalInfos(string code, int workUid, int kofficeUid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@code", code);
            parameters.Add("@uid", workUid);
            parameters.Add("@koffice_uid", kofficeUid);
            return GetResults<AuctionWorkAdditionalInfo>("usp_koffice_work_additional_info_select", parameters);
        }

        public IEnumerable<Image> GetImages(int uid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@uid", uid);
            return GetResults<Image>("usp_koffice_images_select", parameters);
        }

        public DbResult SetAuctionBidPre(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@work_uid", JsonHelper.GetString(json, "work_uid", "0"));
            parameters.Add("@price_bid_pre", JsonHelper.GetString(json, "price_bid_pre", "0"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@bid_type", JsonHelper.GetString(json, "bid_type"));
            parameters.Add("@lang", JsonHelper.GetString(json, "lang", "K"));
            parameters.Add("@reg_ip", JsonHelper.GetString(json, "reg_ip"));
            return GetSingleResult<DbResult>("usp_auction_bid_pre_update", parameters);
        }

        public string SetAuctionLiveRequest(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind"));
            parameters.Add("@auc_num", JsonHelper.GetString(json, "auc_num", "0"));
            parameters.Add("@mem_seq", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@bid_type", JsonHelper.GetString(json, "bid_type"));
            parameters.Add("@location_flag", JsonHelper.GetString(json, "location_flag"));
            parameters.Add("@reg_ip", JsonHelper.GetString(json, "reg_ip"));
            return GetSingleResult<string>("db_koffice.dbo.usp_auction_live_request_insert_www", parameters);
        }

        public AuctionLiveRequest GetAuctionLiveRequestInfo(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@auc_num", JsonHelper.GetString(json, "auc_num", "0"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            return GetSingleResult<AuctionLiveRequest>("usp_koffice_auction_live_request_info_select", parameters);
        }

        public IEnumerable<AuctionBid> GetMemberBidPreinfo(int workUid, int memUid = 0)
        {
            DynamicParameters parameters = new DynamicParameters();            
            parameters.Add("@work_uid", workUid);
            parameters.Add("@mem_uid", memUid);
            return GetResults<AuctionBid>("usp_member_bid_pre_select", parameters);
        }

        public DbResult SetAuctionWorkSearchHst(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@key", JsonHelper.GetString(json, "key"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@token", JsonHelper.GetString(json, "token"));
            return GetSingleResult<DbResult>("usp_auction_work_search_hst_update", parameters);
        }

        public IEnumerable<AuctionWorkSearchHistory> GetAuctionWorkSearchHistories(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind"));
            parameters.Add("@auc_num", JsonHelper.GetString(json, "auc_num", "0"));
            parameters.Add("@reg_date", JsonHelper.GetString(json, "reg_date"));
            parameters.Add("@start_date", JsonHelper.GetString(json, "start_date"));
            parameters.Add("@end_date", JsonHelper.GetString(json, "end_date"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@token", JsonHelper.GetString(json, "token"));
            return GetResults<AuctionWorkSearchHistory>("usp_auction_work_search_hst_select", parameters);
        }

        public IEnumerable<AuctionReport> GetAuctionReports(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "admin_mail");
            parameters.Add("@auc_kind", JsonHelper.GetString(json, "auc_kind"));
            parameters.Add("@auc_num", JsonHelper.GetString(json, "auc_num"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid"));
            return GetResults<AuctionReport>("db_koffice.dbo.usp_auction_work_list", parameters);
        }

        public AuctionWorkKoffice GetAuctionWorkKoffice(int uid, int memUid = 0)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@uid", uid);
            return GetSingleResult<AuctionWorkKoffice>("usp_koffice_work_select", parameters);
        }

        public IEnumerable<AuctionBidProc> GetAuctionBidProcs(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "admin");
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            return GetResults<AuctionBidProc>("dbo.usp_auction_bid_proc_koffice_select", parameters);
        }
    }
}
