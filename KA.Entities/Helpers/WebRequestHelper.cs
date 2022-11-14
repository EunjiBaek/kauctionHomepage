using System.IO;
using System.Net;

namespace KA.Entities.Helpers
{
    public static class WebRequestHelper
    {
        /// <summary>
        /// 통화 포맷으로 (콤마) 처리하여 리턴
        /// </summary>
        /// <param name="value"></param>
        /// <param name="empty"></param>
        /// <returns></returns>
        public static string GetResponseContent(string url, out HttpStatusCode statusCode)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = WebRequestMethods.Http.Get;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    statusCode = response.StatusCode;
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                statusCode = (ex.Response as HttpWebResponse).StatusCode;
                return default(string);
            }
        }
    }
}
