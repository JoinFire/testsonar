using Newtonsoft.Json;
using ERP.TRTBusinessZT.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ERP.TRTBusinessZT.Request
{
    /// <summary>
    /// 订单批次同步至订单中台
    /// </summary>
    public class SynBatchRequest : IBaseRequest<SynBatchResponse>
    {
        /// <summary>
        /// 子单编号
        /// </summary>
        public string originalOrderId { get; set; }
        /// <summary>
        /// 母单编号
        /// </summary>
        public string originalTradeId { get; set; }
        /// <summary>
        /// 要更新的批次列表
        /// </summary>
        public List<Batchs> batchs { get; set; }

        public class Batchs
        {
            /// <summary>
            /// 批次号
            /// </summary>
            public string batch { get; set; }
            /// <summary>
            /// 批号
            /// </summary>
            public string no { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public string num { get; set; }
        }

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
            return "/v1/oms/order/original/synBatch";
        }
    }
}
