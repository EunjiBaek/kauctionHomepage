using System;
using System.Linq;
using KA.Entities.Models.Common;
using Newtonsoft.Json.Linq;

namespace KA.Entities.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// Object 를 JObject 개체로 리턴
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static JObject GetJObject(object target)
        {
            return JObject.FromObject(target);
        }

        /// <summary>
        /// JObject 개체의 key 에 해당하는 값을 리턴
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="key"></param>
        /// <param name="emptyValue"></param>
        /// <returns></returns>
        public static string GetString(JObject jObject, string key, string emptyValue = "")
        {
            return jObject[key] != null && !string.IsNullOrWhiteSpace(jObject[key].ToString()) ? jObject[key].ToString() : (string.IsNullOrWhiteSpace(emptyValue) ? "" : emptyValue);
        }

        /// <summary>
        /// JObject 개체의 key에 해당되는 배열 값을 반환
        /// </summary>
        /// <param name="jObject">json 오브젝트</param>
        /// <param name="key">키</param>
        /// <returns>배열</returns>
        public static string[] GetArray(JObject jObject, string key)
        {
            if (jObject[key] is null)
            {
                return Array.Empty<string>();
            }

            if (jObject[key].Type == JTokenType.String)
            {
                return new[] { jObject[key].ToString() };
            }

            if (jObject[key].Type == JTokenType.Array)
            {
                return jObject[key]?.Values<string>().ToArray();
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// JObject 개체의 key에 해당되는 DateTime 값을 반환
        /// </summary>
        /// <param name="jObject">json 오브젝트</param>
        /// <param name="key">키</param>
        /// <returns>DateTime</returns>
        public static DateTime? GetDateTime(JObject jObject, string key)
        {
            if (jObject[key] is null) return null;

            if (jObject[key].Type == JTokenType.Date)
            {
                return jObject[key].Value<DateTime>().ToLocalTime();
            }

            return null;
        }

        /// <summary>
        /// WebApi 처리 결과를 Json 포맷으로 리턴
        /// {
        ///     code: 00/99 (00 인 경우 Alert 에 success 타입으로 UI 처리)
        ///     message: ka.msg 의 키값으로 메세지 처리 (eval 처리)
        ///     data: 추가 데이터 (옵션)
        /// }
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JObject GetApiResult(string code, string message, object data = null)
        {
            return GetJObject(new ApiResult(code, message, data));
        }

        public static JObject GetApiResult(string resultCode, object data = null)
        {
            return GetJObject(new ApiResult(resultCode, resultCode.StartsWith("ka.") ? string.Empty : MessageHelper.Get(resultCode), data));
        }

        public static JObject GetApiResultLang(string resultCode, bool isKor, object data = null)
        {
            return GetJObject(new ApiResult(resultCode, resultCode.StartsWith("ka.") ? string.Empty : MessageHelper.Get(resultCode, isKor), data));
        }

        public static JObject GetApiListResult(object info, object data, int recordsTotal, int recordsFiltered, int draw = 1)
        {
            return GetJObject(new ApiBoardResult(info, data, recordsTotal, recordsFiltered, draw));
        }

        public static object GetResult(string resultCode, object data = null)
        {
            return new ApiResult(resultCode, resultCode.StartsWith("ka.") ? string.Empty : MessageHelper.Get(resultCode), data);
        }
    }
}
