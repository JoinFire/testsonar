using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Romens.OSS;
using Romens.TRTIoT.Client;
using Romens.TRTIoT.Log;
using Romens.TRTIoT.Request;
using Romens.TRTIoT.Response;
using Romens.TRTIoT.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using TrtInterface.Sql;
using static Romens.TRTIoT.Response.CheckBodyReportResponse;
using static System.Net.Mime.MediaTypeNames;

namespace TrtInterface.Iot
{
    public class IotHelp
    {
        #region Iot需要的公共参数
        public static string Version { get; private set; } = ConfigurationManager.AppSettings["version"].ToString();
        public static string AccessKeyId { get; private set; } = ConfigurationManager.AppSettings["accessKeyId"].ToString();
        public static string SignatureMethod { get; private set; } = ConfigurationManager.AppSettings["signatureMethod"].ToString();
        public static string SignatureVersion { get; private set; } = ConfigurationManager.AppSettings["signatureVersion"].ToString();
        public static string PartnerId { get; private set; } = ConfigurationManager.AppSettings["partnerId"].ToString();
        public static string Host { get; private set; } = ConfigurationManager.AppSettings["host"].ToString();
        public static string Url { get; private set; } = ConfigurationManager.AppSettings["url"].ToString();

        //<add key = "version" value="2018-03-15"/>
        //<add key = "accessKeyId" value="Trtjk123eHJuMOrT"/>
        //<add key = "signatureMethod" value="HMAC-SHA1"/>
        //<add key = "signatureVersion" value="1.0"/>
        //<add key = "partnerId" value="DaShuJuPingTai"/>

        //<add key = "host" value="http://10.8.152.151:8010"/>
        //<add key = "url" value="/api/v1/tianyan/"/>
        #endregion

        public bool Operation()
        {
            bool result = false;
            try
            {
                string errorMessage = string.Empty;
                NewConfig config = new NewConfig();
                Romens.TRTIoT.Client.Client client = new Client(config);

                GetCheckBodyReportListRequest Request = new GetCheckBodyReportListRequest();
                #region 固定参数
                Request.Version = Version;
                Request.AccessKeyId = AccessKeyId;
                Request.SignatureMethod = SignatureMethod;
                Request.Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                Request.SignatureVersion = SignatureVersion;
                Request.SignatureNonce = Guid.NewGuid().ToString();
                Request.PartnerId = PartnerId;
                #endregion
                Request.Size = "10";

                List<SqlsInfo> Sqls = new List<SqlsInfo>();
                string res = client.Execut(Request, ref errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Logs.WriteLogForDebug("错误：", errorMessage);
                    result = false;
                    return result;
                }
                CheckBodyReportResponse response = JsonConvert.DeserializeObject<CheckBodyReportResponse>(res);
                if (response.Code != 0)
                {
                    Logs.WriteJsonLogForDebug("访问接口出现问题",response);
                    result = false;
                    return result;
                }

                if (response.Body.Length > 0)
                {
                    foreach (BodyInfo item in response.Body)
                    {
                        OracleParameter oracleParameter = new OracleParameter("CONTENT", OracleDbType.Clob);
                        oracleParameter.Value = item.Content;
                        string GUID = Guid.NewGuid().ToString();

                        string phone = string.Empty;
                        Logs.WriteDXLog(item.GUID);

                        #region 获取手机号
                        JObject jsonObj = JObject.Parse(item.Content);
                        //判断设备号(不同设备返回的json数据格式不一样)
                        if (item.DeviceUID == "400")
                        {
                            if (jsonObj.Property("Mobile_Number") != null)
                            {
                                phone = jsonObj.GetValue("Mobile_Number").ToString();
                            }
                        }
                        else if (item.DeviceUID == "900")
                        {
                            if (jsonObj.Property("Paint") != null)
                            {
                                JObject Paint = JObject.Parse(jsonObj.GetValue("Paint").ToString());
                                if (Paint.Property("电话") != null)
                                {
                                    phone = Paint.GetValue("电话").ToString();
                                }
                            }
                        }
                        #endregion

                        //插入主表TRT_IOTCHECKBODYREPORT
                        SqlsInfo sqlsInfo = new SqlsInfo(string.Format(string.Format("INSERT INTO TRT_IOTCHECKBODYREPORT(GUID,CONTENT,FGUID,DEVICEUID,PHONE)VALUES('{0}',:CONTENT,'{1}','{2}','{3}')", GUID, item.GUID, item.DeviceUID, phone)), oracleParameter, 0);
                        Sqls.Add(sqlsInfo);

                        //如果附件数量大于0再进行请求
                        if (item.AttachmentCount > 0)
                        {
                            GetCheckBodyReportAttachmentRequest itemRequset = new GetCheckBodyReportAttachmentRequest();
                            #region 固定参数
                            itemRequset.Version = Version;
                            itemRequset.AccessKeyId = AccessKeyId;
                            itemRequset.SignatureMethod = SignatureMethod;
                            itemRequset.Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                            itemRequset.SignatureVersion = SignatureVersion;
                            itemRequset.SignatureNonce = Guid.NewGuid().ToString();
                            itemRequset.PartnerId = PartnerId;
                            #endregion
                            itemRequset.CheckBodyReportGUID = item.GUID;
                            string itemres = client.Execut(itemRequset, ref errorMessage);
                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                Logs.WriteLogForDebug("错误：", errorMessage);
                                result = false;
                                return result;
                            }
                            GetCheckBodyReportAttachmentResponse itemresponse = JsonConvert.DeserializeObject<GetCheckBodyReportAttachmentResponse>(itemres);
                            if (itemresponse.Code != 0)
                            {
                                Logs.WriteJsonLogForDebug("访问接口出现问题", itemresponse);
                                result = false;
                                return result;
                            }
                            Logs.WriteJsonLogForDebug("", itemresponse);
                            foreach (GetCheckBodyReportAttachmentResponse.BodyInfo itembody in itemresponse.Body)
                            {
                                //下载pdf
                                DateTime time = DateTime.Now;
                                //上传到oss的返回地址
                                string outUrl = string.Empty;
                                string RndFileName = string.Format("{0}{1}{2}", DateTime.Now.ToString("yyMMdd"), Path.GetRandomFileName().Replace(".", string.Empty), ".pdf");

                                string FilePath = string.Format("{0}/download/{1}/{2}/{3}/{4}", GetAssemblyPath(), time.Year, time.Month, time.Day, RndFileName);

                                if (!HttpHelper.DownLoadFiles(itembody.StoragePath, FilePath))
                                {
                                    Logs.WriteDXLog("pdf文件转存失败");
                                    Logs.WriteJsonLogForDebug("Body信息", itembody);
                                    Logs.WriteJsonLogForDebug("所有信息", itemresponse);
                                }
                                else
                                {
                                    outUrl = OSSOperator.UpLoadLimit(FilePath, true, null, out errorMessage);
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        Logs.WriteDXLog("oss上传失败 原因：" + errorMessage);
                                        Logs.WriteDXLog("本地文件地址：" + FilePath);
                                        Logs.WriteJsonLogForDebug("Body信息", itembody);
                                        Logs.WriteJsonLogForDebug("所有信息", itemresponse);
                                    }
                                }
                                //插入子表TRT_IOTCHECKBODYREPORTDETAIL
                                Sqls.Add(new SqlsInfo(string.Format(" INSERT INTO TRT_IOTCHECKBODYREPORTDETAIL (MGUID,PATH,TYPE,OSSURL) VALUES ('{0}','{1}','{2}','{3}')", GUID, itembody.StoragePath, 1,outUrl), null, 1));
                            }
                        }
                    }
                    //执行sql语句
                    DBOraHelper.ExecuteSql(Sqls);
                    //清空 errorMessage
                    errorMessage = string.Empty;
                    #region 处理完成后调用UpdateCheckBodyReportToSynced接口 
                    UpdateCheckBodyReportToSyncedRequest URequest = new UpdateCheckBodyReportToSyncedRequest();
                    #region 固定参数
                    URequest.Version = Version;
                    URequest.AccessKeyId = AccessKeyId;
                    URequest.SignatureMethod =SignatureMethod;
                    URequest.Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    URequest.SignatureVersion = SignatureVersion;
                    URequest.SignatureNonce = Guid.NewGuid().ToString();
                    URequest.PartnerId = PartnerId;
                    #endregion
                    URequest.RequestID = response.RequestID;
                    string ures = client.Execut(URequest, ref errorMessage);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Logs.WriteLogForDebug("错误：", errorMessage);
                        result = false;
                        return result;
                    }
                    UpdateCheckBodyReportToSyncedResponse updateCheckBodyReportToSyncedResponse = JsonConvert.DeserializeObject<UpdateCheckBodyReportToSyncedResponse>(ures);
                    if (updateCheckBodyReportToSyncedResponse.Code != 0)
                    {
                        Logs.WriteJsonLogForDebug("访问接口出现问题", updateCheckBodyReportToSyncedResponse);
                        result = false;
                        return result;
                    }
                    #endregion
                    result = true;
                    return result;
                }
                else
                {
                    result = true;
                    Logs.WriteDXLog("暂无数据");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logs.WriteJsonLogForDebug("错误", ex);
                result = false;
                return result;
            }
        }

        /// <summary>
        /// 获取Assembly的运行路径
        /// </summary>
        ///<returns></returns>
        private string GetAssemblyPath()
        {
            string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8);    // 8是file:// 的长度
            string[] arrSection = _CodeBase.Split(new char[] { '/' });
            string _FolderPath = "";
            for (int i = 0; i < arrSection.Length - 1; i++)
            {
                _FolderPath += arrSection[i] + "/";
            }
            return _FolderPath;
        }

        public class SqlsInfo
        {
            public SqlsInfo(string sql, OracleParameter parameter, int type)
            {
                this.Sql = sql;
                this.Parameter = parameter;
                //0 带参数 1 不带参数
                this.Type = type;
            }
            public string Sql { get; set; }
            public OracleParameter Parameter { get; set; }
            public int Type { get; set; }
        }

        public class NewConfig : Romens.TRTIoT.Client.Config
        {
            public string Host => IotHelp.Host;
            public string Url => IotHelp.Url;

        }
    }
}
