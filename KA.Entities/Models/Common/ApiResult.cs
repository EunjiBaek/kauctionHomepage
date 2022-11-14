using Newtonsoft.Json;

namespace KA.Entities.Models.Common
{
    public class ApiResult
    {
        public ApiResult(string code, string message = "", object data = null) 
        {
            Code = code;
            Message = message;
            Data = data;
        }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
