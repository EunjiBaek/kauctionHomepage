using Dapper;
using KA.Entities.Helpers;
using KA.Entities.Models.Common;
using KA.Entities.Models.Configuration;
using KA.Entities.Models.Email;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace KA.Repositories
{
    public interface ILogRepository
    {
        public void SetLog(RequestLog requestLog);

        public void SetEmailLog(Email email);

        public IEnumerable<EmailHistory> GetEmailHistories(JObject json);

        public void SetErrorLog(string menu, string type, int memUid, Exception exception);

        public void SetErrorLog(string menu, string type, string message);

        public IEnumerable<UserLog> GetUserLogs(JObject json);
    }

    public class LogRepository : BaseRepository, ILogRepository
    {
        public LogRepository(IConfiguration configuration)
        {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");
        }

        public void SetLog(RequestLog requestLog)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@user_id", requestLog.UserID);
            parameters.Add("@path", requestLog.Path);
            parameters.Add("@referer", requestLog.Referer);
            parameters.Add("@ip", requestLog.Ip);
            parameters.Add("@user_agent", requestLog.UserAgent);
            parameters.Add("@token", requestLog.Token);
            ExecuteScalar("usp_homepage_call_log_update", parameters, _logConnectionString);
        }

        public void SetEmailLog(Email email)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@site", email.Site);
            parameters.Add("@type", email.Type);
            parameters.Add("@subject", email.SubJect);
            parameters.Add("@from", string.IsNullOrWhiteSpace(email.FromEmail) ? "" : $"{email.FromName} <{email.FromEmail}>");
            parameters.Add("@to", (email.ToEmail ?? "") + (email.AddToEmail == null ? "" : " / " + string.Join(",", email.AddToEmail)));
            parameters.Add("@cc", (email.CcEmail ?? "") + (email.AddCcEmail == null ? "" : " / " + string.Join(",", email.AddCcEmail)));
            parameters.Add("@body", email.Body);
            parameters.Add("@result", email.Result);
            parameters.Add("@error_message", email.ErrorMessage);
            parameters.Add("@error_stacktrace", email.ErrorStacktrace);
            parameters.Add("@reg_uid", email.RegUid);
            ExecuteScalar("usp_mail_log_update", parameters, _kauctionConnectionString);
        }

        public IEnumerable<EmailHistory> GetEmailHistories(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@type", JsonHelper.GetString(json, "type"));
            parameters.Add("@filter", JsonHelper.GetString(json, "filter"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            return GetResults<EmailHistory>("usp_mail_log_select", parameters);
        }

        public void SetErrorLog(string menu, string type, int memUid, Exception exception)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mem_uid", memUid);
            parameters.Add("@menu", menu);
            parameters.Add("@type", type);
            parameters.Add("@message", exception.Message);
            parameters.Add("@stacktrace", exception.StackTrace);
            ExecuteScalar("usp_homepage_error_log_update", parameters, _logConnectionString);
        }

        public void SetErrorLog(string menu, string type, string message)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mem_uid", "0");
            parameters.Add("@menu", menu);
            parameters.Add("@type", type);
            parameters.Add("@message", message);
            parameters.Add("@stacktrace", "");
            ExecuteScalar("usp_homepage_error_log_update", parameters, _logConnectionString);
        }

        public IEnumerable<UserLog> GetUserLogs(JObject json)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mode", JsonHelper.GetString(json, "mode"));
            parameters.Add("@page_size", JsonHelper.GetString(json, "page_size", "20"));
            parameters.Add("@page", JsonHelper.GetString(json, "page", "1"));
            parameters.Add("@mem_uid", JsonHelper.GetString(json, "mem_uid", "0"));
            return GetResults<UserLog>("usp_user_log_select", parameters);
        }
    }
}
