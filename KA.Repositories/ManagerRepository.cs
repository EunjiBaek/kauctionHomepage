using Dapper;
using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Entities.Models.Manager;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace KA.Repositories
{
    public interface IManagerRepository
    {
        /// <summary>
        /// [SSO] Token을 받아 유효한 경우 uid를 리턴, 오류시 -1 리턴
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetUIDByToken(string token);

        /// <summary>
        /// [SSO] 로그인 후 전달받은 정보로 담당자 정보 조회
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Manager GetManagerFromLogin(object uID, string ip = "", string token = "");

        /// <summary>
        /// [관리자 > Login] 관리자 정보 조회
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="mode"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public Manager GetManager(Manager manager, string mode, string ip = "");

        /// <summary>
        /// [관리자 > Configuration > Manager] 관리자 정보 조회
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Manager> GetManagers();

        /// <summary>
        /// [관리자 > Configuration > Manager] 관리자 동기화
        /// </summary>
        /// <returns></returns>
        public DbResult SetManagerUpdateBatch();

        /// <summary>
        /// [관리자 > Configuration > Manager] 담당자 권한 설정
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DbResult SetManagerAuth(JObject json);

        /// <summary>
        /// [관리자 > Member > Detail] 회원 정보 조회 이력 처리
        /// </summary>
        /// <param name="mngUid"></param>
        /// <param name="memUid"></param>
        /// <returns></returns>
        public DbResult SetManagerViewHst(JObject json);
    }

    public class ManagerRepository : BaseRepository, IManagerRepository
    {
        public ManagerRepository(IConfiguration configuration)
        {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");
        }

        public string GetUIDByToken(string token)
        {
            // if (string.IsNullOrWhiteSpace(token)) return "-1";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", "token_check");
            parameters.Add("@token", token);
            parameters.Add("@system_name", "Homepage");

            return ExecuteScalar("usp_sso_call_log_select", parameters, _logConnectionString).ToString();
        }

        public Manager GetManagerFromLogin(object uID, string ip = "", string token = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("mode", "login");
            parameters.Add("uid", uID);
            parameters.Add("ip", ip);
            parameters.Add("token", token);

            return GetSingleResult<Manager>("usp_manager_select", parameters);
        }

        public Manager GetManager(Manager manager, string mode, string ip = "")
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("mode", mode);
            parameters.Add("uid", manager.UID);
            parameters.Add("id", manager.ID);
            parameters.Add("pw", manager.Pw);
            parameters.Add("token", manager.Token);
            parameters.Add("ip", ip);

            return GetSingleResult<Manager>("usp_manager_select", parameters);
        }

        public IEnumerable<Manager> GetManagers()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("mode", "list");

            return GetResults<Manager>("usp_manager_select", parameters);
        }

        public DbResult SetManagerUpdateBatch()
        {
            return GetSingleResult<DbResult>("usp_manager_update_batch", null);
        }

        public DbResult SetManagerAuth(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@uid", JsonHelper.GetString(json, "uid", "-1"));
            parameters.Add("@auth", JsonHelper.GetString(json, "auth"));
            parameters.Add("@reg_uid", JsonHelper.GetString(json, "reg_uid", "-1"));
            return GetSingleResult<DbResult>("usp_manager_auth_update", parameters);
        }

        public DbResult SetManagerViewHst(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mng_uid", JsonHelper.GetString(json, "mng_uid", "0"));
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@target", JsonHelper.GetString(json, "target"));
            parameters.Add("@etc1", JsonHelper.GetString(json, "etc1"));
            parameters.Add("@etc2", JsonHelper.GetString(json, "etc2"));
            parameters.Add("@etc3", JsonHelper.GetString(json, "etc3"));
            return GetSingleResult<DbResult>("usp_manager_view_hst_update", parameters);
        }
    }
}
