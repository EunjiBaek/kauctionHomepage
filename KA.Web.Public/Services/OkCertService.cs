using KA.Entities.Models.OkCert;
using KA.Web.Public.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OkCert3Com;
using System;
using System.Text;

namespace KA.Web.Public.Services
{
    public class OkCertService
    {
        private readonly string SITE_NAME;
        private readonly string SITE_URL;
        private readonly string SITE_SCHEME;
        
        private readonly string CP_CD;
        private readonly string TARGET;
        private readonly string MOBILE_POPUP_URL;
        private readonly string MOBILE_LICENSE;
        private readonly string MOBILE_SERVICE_START_NAME;
        private readonly string MOBILE_SERVICE_RESULT_NAME;
        private readonly string CARD_POPUP_URL;
        private readonly string CARD_LICENSE;
        private readonly string CARD_SERVICE_START_NAME;
        private readonly string CARD_SERVICE_RESULT_NAME;

        public readonly string REQUEST_KEY;

        public OkCertService(HttpRequest request)
        {
            this.SITE_NAME = "K-Auction";
            this.SITE_URL = request.Host.Value;
            this.SITE_SCHEME = request.Scheme;

            this.CP_CD = Config.OkCert3.CP_CD;
            this.TARGET = Config.OkCert3.TARGET;
            this.MOBILE_POPUP_URL = Config.OkCert3.MOBILE_POPUP_URL;
            this.MOBILE_LICENSE = Config.OkCert3.MOBILE_LICENSE;
            this.MOBILE_SERVICE_START_NAME = Config.OkCert3.MOBILE_SERVICE_START_NAME;
            this.MOBILE_SERVICE_RESULT_NAME = Config.OkCert3.MOBILE_SERVICE_RESULT_NAME;
            this.CARD_POPUP_URL = Config.OkCert3.CARD_POPUP_URL;
            this.CARD_LICENSE = Config.OkCert3.CARD_LICENSE;
            this.CARD_SERVICE_START_NAME = Config.OkCert3.CARD_SERVICE_START_NAME;
            this.CARD_SERVICE_RESULT_NAME = Config.OkCert3.CARD_SERVICE_RESULT_NAME;

            this.REQUEST_KEY = Guid.NewGuid().ToString();
        }

        public SendResult SendRequest(string auth, string returnUrl, string returnMsg = "")
        {
            // OkCert3 JSON 요청파라미터
            // [Mobile]
            // - RQST_CAUS_CD (00:회원가입,01:성인인증,02:회원정보수정,03:비밀번호찾기,04:상품구매,99:기타)
            StringBuilder sb = new("{");
            if (auth.Equals("C"))
            {
                sb.AppendFormat("\"RTN_URL\":\"{0}\",", SITE_SCHEME + "://" + SITE_URL + returnUrl);
                sb.AppendFormat("\"REQ_SITE_NM\":\"{0}\"", SITE_NAME);
            }
            else
            {
                sb.AppendFormat("\"RETURN_URL\":\"{0}\",", SITE_SCHEME + "://" + SITE_URL + returnUrl);
                sb.AppendFormat("\"SITE_NAME\":\"{0}\",", SITE_NAME);
                sb.AppendFormat("\"SITE_URL\":\"{0}\",", SITE_URL);
                sb.AppendFormat("\"RQST_CAUS_CD\":\"{0}\",", "00");
                sb.AppendFormat("\"RETURN_MSG\":\"{0}\"", string.IsNullOrWhiteSpace(returnMsg) ? REQUEST_KEY : returnMsg);
            }
            sb.Append('}');

            string reqJson = sb.ToString();
            string param = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(reqJson));

            int returnValue;

            try
            {
                OkCert3 okcert3 = new();
                if (Config.OkCert3.ENCODING.Equals("U"))
                {
                    returnValue = (int)okcert3.callOkCert3U(TARGET,
                        CP_CD,
                        auth.Equals("C") ? CARD_SERVICE_START_NAME : MOBILE_SERVICE_START_NAME,
                        param,
                        auth.Equals("C") ? CARD_LICENSE : MOBILE_LICENSE,
                        out object output);

                    if (returnValue != 0)
                    {
                        return new SendResult()
                        {
                            RSLT = false,
                            RSLT_CD = "",
                            RSLT_MSG = "ERROR"
                        };
                    }
                    else
                    {
                        var obj = JsonConvert.DeserializeObject<SendResult>((string)output);
                        obj.RSLT = true;
                        obj.CP_CD = CP_CD;
                        obj.POPUP_URL = auth.Equals("C") ? CARD_POPUP_URL : MOBILE_POPUP_URL;
                        return obj;
                    }
                }
                else
                {
                    returnValue = (int)okcert3.callOkCert3(TARGET,
                        CP_CD,
                        auth.Equals("C") ? CARD_SERVICE_START_NAME : MOBILE_SERVICE_START_NAME,
                        param,
                        auth.Equals("C") ? CARD_LICENSE : MOBILE_LICENSE,
                        out object output);

                    if (returnValue != 0)
                    {
                        return new SendResult()
                        {
                            RSLT = false,
                            RSLT_CD = "",
                            RSLT_MSG = "ERROR"
                        };
                    }
                    else
                    {
                        var obj = JsonConvert.DeserializeObject<SendResult>((string)output);
                        obj.RSLT = true;
                        obj.CP_CD = CP_CD;
                        obj.POPUP_URL = auth.Equals("C") ? CARD_POPUP_URL : MOBILE_POPUP_URL;
                        return obj;
                    }
                }
            }
            catch (Exception error)
            {
                return new SendResult()
                {
                    RSLT = false,
                    RSLT_MSG = error.ToString()
            };
            }
        }

        public ReceiveResult ReceiveResult(string auth, string MdlTkn)
        {
            // OkCert3 JSON 요청파라미터
            StringBuilder sb = new("{");
            sb.AppendFormat("\"MDL_TKN\":\"{0}\"", MdlTkn);
            sb.Append('}');

            string reqJson = sb.ToString();
            string param = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(reqJson));

            int returnValue;

            try
            {
                OkCert3 okcert3 = new();
                if (Config.OkCert3.ENCODING.Equals("U"))
                {
                    returnValue = (int)okcert3.callOkCert3U(TARGET,
                        CP_CD,
                        auth.Equals("C") ? CARD_SERVICE_RESULT_NAME : MOBILE_SERVICE_RESULT_NAME,
                        param,
                        auth.Equals("C") ? CARD_LICENSE : MOBILE_LICENSE,
                        out object output);

                    if (returnValue != 0)
                    {
                        return new ReceiveResult()
                        {
                            RSLT = false
                        };
                    }
                    else
                    {
                        var obj = JsonConvert.DeserializeObject<ReceiveResult>((String)output);
                        if (auth.Equals("C"))
                        {
                            obj.RSLT = obj.RSLT_CD.Equals("T000");
                        }
                        else
                        {
                            obj.RSLT = obj.RSLT_CD.Equals("B000");
                        }

                        return obj;
                    }
                }
                else
                {
                    returnValue = (int)okcert3.callOkCert3(TARGET,
                        CP_CD,
                        auth.Equals("C") ? CARD_SERVICE_RESULT_NAME : MOBILE_SERVICE_RESULT_NAME,
                        param,
                        auth.Equals("C") ? CARD_LICENSE : MOBILE_LICENSE,
                        out object output);

                    if (returnValue != 0)
                    {
                        return new ReceiveResult()
                        {
                            RSLT = false
                        };
                    }
                    else
                    {
                        var obj = JsonConvert.DeserializeObject<ReceiveResult>((String)output);
                        if (auth.Equals("C"))
                        {
                            obj.RSLT = obj.RSLT_CD.Equals("T000");
                        }
                        else
                        {
                            obj.RSLT = obj.RSLT_CD.Equals("B000");
                        }

                        return obj;
                    }
                }
            }
            catch (Exception error)
            {
                return new ReceiveResult()
                {
                    RSLT = false,
                    RSLT_MSG = error.ToString()
                };
            }
        }
    }
}
