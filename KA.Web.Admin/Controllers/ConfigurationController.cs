using KA.Entities.Helpers;
using KA.Repositories;
using KA.Web.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace KA.Web.Admin.Controllers
{
    [Authorize]
    public class ConfigurationController : Controller
    {
        #region # Constructor #

        private readonly ICommonRepository _commonRepository;
        private readonly IManagerRepository _managerRepository;
        private readonly ILogRepository _logRepository;

        public ConfigurationController(ICommonRepository commonRepository, 
            IManagerRepository managerRepository,
            ILogRepository logRepository)
        {
            _commonRepository = commonRepository;
            _managerRepository = managerRepository;
            _logRepository = logRepository;
        }

        #endregion

        #region # Configuration > Manager #

        #region [WebApi]

        /// <summary>
        /// [API] 담당자 정보 처리 결과 Json 리턴
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Configuration/GetManagers")]
        public JObject GetData([FromBody] JObject json)
        {
            var data = _managerRepository.GetManagers();
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        /// <summary>
        /// [API] 담당자 동기화 처리 결과 Json 리턴
        /// </summary>
        /// <returns></returns>
        [Route("api/Configuration/SetManagerUpdateBatch")]
        public JObject SetManagerUpdateBatch()
        {
            var result = _managerRepository.SetManagerUpdateBatch();
            return JsonHelper.GetApiResult(result.Result);
        }

        [Route("api/Configuration/GetManagerAuth")]
        public JObject GetManagerAuth([FromBody] JObject json)
        {
            var data = _commonRepository.GetCodeList("", "manager_auth", "K", int.TryParse(JsonHelper.GetString(json, "uid", "-1"), out int uid) ? uid : -1);
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, 0, 0, 1));
        }

        [Route("api/Configuration/SetManagerAuth")]
        public JObject SetManagerAuth([FromBody] JObject json)
        {
            json["reg_uid"] = LoginInfo.UID;
            var result = _managerRepository.SetManagerAuth(json);
            return JsonHelper.GetApiResult(result.Result);
        }

        #endregion

        #region [View]

        [Route("/Configuration/Manager")]
        public IActionResult ManagerList()
        {
            return View();
        }

        #endregion

        #endregion

        #region # Configuration > CommonCode #

        #region [WebApi]

        [HttpPost]
        [Route("api/Configuration/GetCommonCodeTreeView")]
        public JObject GetCommonCodeTreeView()
        {
            return JsonHelper.GetApiResult("00", _commonRepository.GetCodeList("", "treeview"));
        }

        [HttpPost]
        [Route("/api/Configuration/GetCommonCodeList")]
        public JObject GetCommonCodeList([FromBody] JObject json)
        {
            return JsonHelper.GetApiResult("00", _commonRepository.GetCodeList(json["main_code"].ToString()));
        }

        #endregion

        #region [View]

        public IActionResult CommonCode()
        {
            return View();
        }

        #endregion

        #endregion

        #region # Configuration > Mail Log #

        #region [WebApi]

        [Route("api/Configuration/GetEmailHistories")]
        public JObject GetEmailHistories([FromBody] JObject json)
        {
            var data = _logRepository.GetEmailHistories(json);
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;

            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));            
        }

        #endregion

        #region [View]

        public IActionResult MailHistory()
        {
            ViewBag.MailCode = _commonRepository.GetCodeList("MAIL_TYPE");
            return View();
        }

        #endregion

        #endregion

        #region # Configuration > Encryption #

        #region [WebApi]

        [Route("api/Configuration/GetEncryptData")]
        public JObject GetEncryptData([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            var value = JsonHelper.GetString(json, "value");
            var mode = JsonHelper.GetString(json, "mode");
            var result = string.Empty;

            if (mode.Equals("E"))
            {
                result = DESCryptoHelper.DESEncrypt(value);
            }
            else if (mode.Equals("D"))
            {
                result = DESCryptoHelper.DESDecrypt(value);
            }

            return JsonHelper.GetApiResult("00", result);
        }

        #endregion

        #region [View]

        public IActionResult Encryption()
        {
            return View();
        }

        #endregion

        #endregion

        #region # Configuration > User Log #

        #region [WebApi]

        [Route("api/Configuration/GetUserLog")]
        public JObject GetUserLog([FromBody] JObject json)
        {
            if (json == null) return JsonHelper.GetApiResult("90");

            var type = JsonHelper.GetString(json, "type");

            if (!type.Equals("004") && !int.TryParse(JsonHelper.GetString(json, "mem_uid"), out int r)) return JsonHelper.GetApiResult("90");

            if (type.Equals("001"))
            {
                json["mode"] = "user_log";
            }
            else if (type.Equals("002"))
            {
                json["mode"] = "certificate_doc";
            }
            else if (type.Equals("003"))
            {
                json["mode"] = "error_log";
            }
            else if (type.Equals("004"))
            {
                json["mode"] = "mobile_auth_log";
            }

            var data = _logRepository.GetUserLogs(json);
            var totalCount = data.ToList().Count > 0 ? data.ToList()[0].TotalCount : 0;
            return JsonHelper.GetApiResult("00", JsonHelper.GetApiListResult(null, data, totalCount, totalCount, int.Parse(JsonHelper.GetString(json, "page", "1"))));
        }

        #endregion

        #region [View]

        public IActionResult UserLog()
        {
            return View();
        }

        #endregion

        #endregion
    }
}
