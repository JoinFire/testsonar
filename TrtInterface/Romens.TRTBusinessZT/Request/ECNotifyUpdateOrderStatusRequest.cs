using Newtonsoft.Json;
using ERP.TRTBusinessZT.Response;
using ERP.TRTBusinessZT.Sign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ERP.TRTBusinessZT.Request
{
    public class ECNotifyUpdateOrderStatusRequest : IBaseRequest<ECNotifyUpdateOrderStatusResponse>
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string originalTradeId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }

        public string GetMethod()
        {
            return "POST";
        }

        public string GetParam()
        {
            string Param = string.Empty;
            JsonSerializer serializer = new JsonSerializer();
            StringWriter sw = new StringWriter();
            serializer.Serialize(new JsonTextWriter(sw), this);
            Param = sw.GetStringBuilder().ToString();
            return Param;
        }

        public string GetUrl()
        {
            return "/v1/oms/order/original/status";
        }
    }
}
