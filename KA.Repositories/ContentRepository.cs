using Dapper;
using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Entities.Models.Content;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KA.Repositories
{
    public interface IContentRepository
    {
        /// <summary>
        /// 게시판 정보 가져오는 함수
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        BoardInfo GetBoardInfo(object board);
        
        /// <summary>
        /// 사용한 게시판 정보 목록 가져오는 함수
        /// </summary>
        /// <returns></returns>
        IEnumerable<BoardInfo> GetBoardInfos(string mode);

        /// <summary>
        /// 게시판의 게시글 정보 가져오는 함수
        /// </summary>
        /// <param name="board"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        List<BoardDoc> GetBoardDocs(string mode, object board, string page, string noticeYN = "");

        /// <summary>
        /// 게시판의 게시글 정보 가져오는 함수 (파라미터 json)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        List<BoardDoc> GetBoardDocs(JObject json);

        /// <summary>
        /// 게시글 상세 정보 가져오는 함수
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        BoardDoc GetBoardDoc(object uid);

        /// <summary>
        /// 게시글 상세 정보 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        string SetBoardDoc(JObject json);

        /// <summary>
        /// 조회수 업데이터 처리 함수
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        string UpdateRead(object uid, string ip = "", int loginUid = -1);

        /// <summary>
        /// 파일 업로드 목록 정보 가져오는 함수
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        IEnumerable<FileUpload> GetFileUploads(string mode, string type, string search, int page, string seq = "");

        /// <summary>
        /// 파일 업로드 정보 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        DbResult SetFileUpload(JObject json);
    }

    public class ContentRepository : BaseRepository, IContentRepository
    {
        public ContentRepository(IConfiguration configuration)
        {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");
        }

        public BoardInfo GetBoardInfo(object board)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", "detail");
            parameter.Add("@board_key", board);
            return GetSingleResult<BoardInfo>("usp_board_info_select", parameter);
        }

        public IEnumerable<BoardInfo> GetBoardInfos(string mode)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            return GetResults<BoardInfo>("usp_board_info_select", parameters);
        }

        public List<BoardDoc> GetBoardDocs(string mode, object board, string page = "1", string noticeYN = "")
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", string.IsNullOrWhiteSpace(mode) ? "list" : mode);
            parameter.Add("@board_key", board);
            parameter.Add("@page", page);
            parameter.Add("@notice_yn", noticeYN);            
            return GetResults<BoardDoc>("usp_board_doc_select", parameter).ToList();
        }

        public List<BoardDoc> GetBoardDocs(JObject json)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", JsonHelper.GetString(json, "mode", "list"));
            parameter.Add("@board_key", JsonHelper.GetString(json, "board_key"));
            parameter.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameter.Add("@notice_yn", JsonHelper.GetString(json, "notice_yn"));
            parameter.Add("@search", JsonHelper.GetString(json, "search"));
            return GetResults<BoardDoc>("usp_board_doc_select", parameter).ToList();
        }

        public BoardDoc GetBoardDoc(object uid)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", "detail");
            parameter.Add("@board_doc_uid", uid);
            return GetSingleResult<BoardDoc>("usp_board_doc_select", parameter);
        }

        public string SetBoardDoc(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@board_key", JsonHelper.GetString(json, "board_key"));
            parameters.Add("@board_info_uid", JsonHelper.GetString(json, "board_info_uid", "0"));
            parameters.Add("@board_doc_uid", JsonHelper.GetString(json, "board_doc_uid", "0"));
            parameters.Add("@grp_no", JsonHelper.GetString(json, "grp_no", "0"));
            parameters.Add("@grp_lvl", JsonHelper.GetString(json, "grp_lvl", "0"));
            parameters.Add("@grp_dept", JsonHelper.GetString(json, "grp_dept", "0"));
            parameters.Add("@mem_seq", JsonHelper.GetString(json, "mem_seq", "0"));
            parameters.Add("@mem_id", JsonHelper.GetString(json, "mem_id"));
            parameters.Add("@mem_nick", JsonHelper.GetString(json, "mem_nick"));
            parameters.Add("@mem_pwd", JsonHelper.GetString(json, "mem_pwd"));
            parameters.Add("@doc_title", JsonHelper.GetString(json, "doc_title"));
            parameters.Add("@doc_contents", JsonHelper.GetString(json, "doc_contents"));
            parameters.Add("@html_yn", JsonHelper.GetString(json, "html_yn"));
            parameters.Add("@redirect_url", JsonHelper.GetString(json, "redirect_url"));
            parameters.Add("@mem_email", JsonHelper.GetString(json, "mem_email"));
            parameters.Add("@mem_mobile", JsonHelper.GetString(json, "mem_mobile"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            parameters.Add("@period_yn", JsonHelper.GetString(json, "period_yn"));
            parameters.Add("@start_date", JsonHelper.GetString(json, "start_date"));
            parameters.Add("@end_date", JsonHelper.GetString(json, "end_date"));
            parameters.Add("@notice_yn", JsonHelper.GetString(json, "notice_yn"));
            parameters.Add("@del_yn", JsonHelper.GetString(json, "del_yn"));
            parameters.Add("@filenames", JsonHelper.GetString(json, "filenames"));
            parameters.Add("@admin_seq", JsonHelper.GetString(json, "admin_seq", "0"));
            parameters.Add("@reg_ip", JsonHelper.GetString(json, "reg_ip"));
            return GetSingleResult<string>("usp_board_doc_update", parameters);
        }

        public string UpdateRead(object uid, string ip = "", int loginUid = -1)
        {
            using IDbConnection connection = new SqlConnection(_kauctionConnectionString);
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            DefaultTypeMap.MatchNamesWithUnderscores = true;

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", "UPDATE_READ");
            parameter.Add("@board_doc_uid", uid);
            parameter.Add("@mem_uid", loginUid);
            parameter.Add("@reg_ip", ip);
            return connection.Query<string>("usp_board_doc_update", parameter, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<FileUpload> GetFileUploads(string mode, string type, string search, int page, string seq = "")
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@mode", string.IsNullOrWhiteSpace(mode) ? "list" : mode);
            parameter.Add("@type", type);
            parameter.Add("@search", search);
            parameter.Add("@seq", seq);
            return GetResults<FileUpload>("usp_content_file_upload_select", parameter).ToList();
        }

        public DbResult SetFileUpload(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@path", JsonHelper.GetString(json, "path"));
            parameters.Add("@name", JsonHelper.GetString(json, "name"));
            parameters.Add("@fullpath", JsonHelper.GetString(json, "fullpath"));
            parameters.Add("@url", JsonHelper.GetString(json, "url"));
            parameters.Add("@explain_text", JsonHelper.GetString(json, "explain_text"));
            parameters.Add("@reg_uid", JsonHelper.GetString(json, "reg_uid", "0"));
            return GetSingleResult<DbResult>("usp_content_file_upload_update", parameters);
        }
    }
}
