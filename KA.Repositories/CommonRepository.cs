using Dapper;
using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace KA.Repositories
{
    public interface ICommonRepository
    {
        /// <summary>
        /// 공통 코드 정보 조회 처리 함수
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IEnumerable<CommonCode> GetCodeList(string mainCode, string mode = "", string lang = "K", int uid = 0);

        public IEnumerable<CommonCode> GetMainCodeList(string mainCodeList);

        /// <summary>
        /// 공통 코드 정보 조회 처리 함수
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public CommonCode GetCode(JObject json);

        /// <summary>
        /// 데이터베이스 시간을 가져오는 함수
        /// </summary>
        /// <returns></returns>
        public DateTime GetDatabaseTime();

        /// <summary>
        /// DB 처리 에러로그 처리 함수
        /// </summary>
        /// <param name="dbResult"></param>
        /// <returns></returns>
        public string SetErrorLog(DbResult dbResult);

        /// <summary>
        /// 메뉴 정보 처리 함수
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public IEnumerable<Menu> GetMenus(string type = "TOP", string lang = "K", int memUid = 0);

        /// <summary>
        /// 암호화(SHA2) 처리 함수
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetEncrypt(string value);
    }

    public class CommonRepository : BaseRepository, ICommonRepository
    {
        public CommonRepository(IConfiguration configuration)
        {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");
        }

        public IEnumerable<CommonCode> GetCodeList(string mainCode, string mode = "", string lang = "K", int uid = 0)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", mode);
            parameters.Add("@main_code", mainCode);
            parameters.Add("@lang", lang);
            parameters.Add("@uid", uid);
            return GetResults<CommonCode>("usp_common_code_select", parameters);
        }

        public IEnumerable<CommonCode> GetMainCodeList(string mainCodeList)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "main_code");
            parameters.Add("@main_code_list", mainCodeList);
            return GetResults<CommonCode>("usp_common_code_select", parameters);
        }


        public CommonCode GetCode(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@main_code", JsonHelper.GetString(json, "main_code"));
            parameters.Add("@sub_code", JsonHelper.GetString(json, "sub_code"));
            parameters.Add("@lang", JsonHelper.GetString(json, "lang"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "0"));
            var result = GetResults<CommonCode>("usp_common_code_select", parameters);
            return result.AsList().Count > 0 ? result.AsList()[0] : null;
        }

        public DateTime GetDatabaseTime()
        {
            return GetSingleResult<DateTime>("usp_database_time_select", null);
        }

        public string SetErrorLog(DbResult dbResult)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@target", dbResult.Target);
            parameters.Add("@error_number", dbResult.ErrorNumber);
            parameters.Add("@error_serverity", dbResult.ErrorServerity);
            parameters.Add("@error_state", dbResult.ErrorState);
            parameters.Add("@error_procedure", dbResult.ErrorProcedure);
            parameters.Add("@error_line", dbResult.ErrorLine);
            parameters.Add("@error_message", dbResult.ErrorMessage);
            parameters.Add("@etc", dbResult.Etc);
            return GetSingleResult<string>("usp_error_log_update", parameters);
        }

        public IEnumerable<Menu> GetMenus(string type = "TOP", string lang = "K", int memUid = 0)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@type", type);
            parameters.Add("@lang", lang);
            parameters.Add("@mem_uid", memUid);
            return GetResults<Menu>("usp_menu_select", parameters);
        }

        public string GetEncrypt(string value)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@value", value);
            return GetSingleResult<string>("usp_common_encrypt_select", parameters);
        }
    }
}
