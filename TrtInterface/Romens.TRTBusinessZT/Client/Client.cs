using Newtonsoft.Json;
using ERP.TRTBusinessZT.Config;
using ERP.TRTBusinessZT.Request;
using ERP.TRTBusinessZT.Response;
using ERP.TRTBusinessZT.Sign;
using ERP.TRTBusinessZT.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.TRTBusinessZT.Client
{
    public class Client
    {
        public IConfig config { get; set; }
        public Client(IConfig config)
        {
            this.config = config;
        }

        private class ZTIConfig : IConfig
        {
            public string Host => "http://39.96.168.75:8001";

            public string Url => "";

            public string AppId => "TRT-oHWFfRWjhIOfiOT8";

            public string AppSecret => "d9nmtMZ5G3WP81NESX";
        }

        public Client()
        {
            config = new ZTIConfig();
        }

        public T Execut<T>(IBaseRequest<T> Request, ref string errorMessage) where T : BaseResponse
        {
            try
            {
                string Response = string.Empty;
                if (Request.GetMethod() == "POST")
                {
                    string Url = config.Host + Request.GetUrl();
                    Logs.WriteDXLog(string.Format("调用业务中台接口地址：{0}",Url));
                    Logs.WriteDXLog(string.Format("入参：{0}", Request.GetParam()));
                    Response = HttpHelper.DoPost(Url, Request.GetParam(), HttpHelper.HttpConstants.CTYPE_APP_JSON,GetHead(config.AppId, config.AppSecret),ref errorMessage);
                    Logs.WriteJsonLogForDebug("返回结果：", Response);
                }
                else if (Request.GetMethod() == "GET")
                {
                    //string Url = config.Host + config.Url + Request.GetUrl() + Request.GetParam();
                    //Response = HttpHelper.GetDataGetHtml(Url);
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Logs.WriteDXLog("错误："+ errorMessage);
                    return null;
                }
                T Result = JsonConvert.DeserializeObject<T>(Response);
                return Result;
            }
            catch (Exception ex)
            {
                Logs.WriteDXLog("错误：" + ex.Message);
                errorMessage = ex.Message;
                return null;
            }
            
        }

        public IDictionary<string, string> GetHead(string appid, string appSecret)
        {
            IDictionary<string, string> keyValues = new Dictionary<string, string>();
            string Timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString();
            string Sign = SigntureHelp.GetSignture(appid, appSecret, Timestamp);
            keyValues.Add("timestamp", Timestamp);
            keyValues.Add("Signature", Sign);
            keyValues.Add("app_id", appid);
            return keyValues;
        }
    }
}
