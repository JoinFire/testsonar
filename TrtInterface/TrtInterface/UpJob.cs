using AliCloudRocketMQSDK;
using ERP.TRTBusinessZT.Client;
using ERP.TRTBusinessZT.Config;
using ERP.TRTBusinessZT.Request;
using ERP.TRTBusinessZT.Response;
using RomensInterfaceExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using TRT_AliVipTicketSDK;
using TRT_AliVipTicketSDK.TargetModels.Member;
using TRT_AliVipTicketSDK.TargetModels.StockSync;

namespace TrtInterface
{
    public class UpJob :  IUpJob
    {
        public bool ExecJob(JobPara paras, DataSet dsData, out string errorMsg)
        {
            //订单上传
            if (paras.jobCode.Equals("10001"))
            {
                DataTable main = dsData.Tables["Head"];
                DataTable detail = dsData.Tables["Detail"];
                string errorMessage = string.Empty;
                Client client = new Client(new ZTConfig());

                #region 订单同步至订单中台
                DownDeliverTradeRequest downDeliverTradeRequest = new DownDeliverTradeRequest();
                downDeliverTradeRequest.originalTradeId = main.Rows[0]["originalTradeId"] == DBNull.Value ? string.Empty : main.Rows[0]["originalTradeId"].ToString();
                downDeliverTradeRequest.serialNumber = main.Rows[0]["serialNumber"] == DBNull.Value ? string.Empty : main.Rows[0]["serialNumber"].ToString();
                if (main.Rows[0]["salesOrderType"] != DBNull.Value)
                {
                    downDeliverTradeRequest.salesOrderType = Convert.ToInt32(main.Rows[0]["salesOrderType"]);
                }
                if (main.Rows[0]["deliveryType"] != DBNull.Value)
                {
                    downDeliverTradeRequest.deliveryType = Convert.ToInt32(main.Rows[0]["deliveryType"]);
                }
                downDeliverTradeRequest.storeId = main.Rows[0]["storeId"] == DBNull.Value ? string.Empty : main.Rows[0]["storeId"].ToString();
                if (main.Rows[0]["salesOrderStatus"] != DBNull.Value)
                {
                    downDeliverTradeRequest.salesOrderStatus = Convert.ToInt32(main.Rows[0]["salesOrderStatus"]);
                }
                downDeliverTradeRequest.orderDate = main.Rows[0]["orderDate"] == DBNull.Value ? string.Empty : main.Rows[0]["orderDate"].ToString();
                if (main.Rows[0]["totalFee"] != DBNull.Value)
                {
                    downDeliverTradeRequest.totalFee = Convert.ToDecimal(main.Rows[0]["totalFee"]);
                }
                if (main.Rows[0]["payment"] != DBNull.Value)
                {
                    downDeliverTradeRequest.payment = Convert.ToDecimal(main.Rows[0]["payment"]);
                }
                if (main.Rows[0]["num"] != DBNull.Value)
                {
                    downDeliverTradeRequest.num = Convert.ToInt32(main.Rows[0]["num"]);
                }
                downDeliverTradeRequest.channelId = main.Rows[0]["channelId"] == DBNull.Value ? string.Empty : main.Rows[0]["channelId"].ToString();
                downDeliverTradeRequest.tradeJson = main.Rows[0]["tradeJson"] == DBNull.Value ? string.Empty : main.Rows[0]["tradeJson"].ToString();
                downDeliverTradeRequest.payTime = main.Rows[0]["payTime"] == DBNull.Value ? string.Empty : main.Rows[0]["payTime"].ToString();
                downDeliverTradeRequest.endTime = main.Rows[0]["payTime"] == DBNull.Value ? string.Empty : main.Rows[0]["payTime"].ToString();

                List<DownDeliverTradeRequest.Listitem> listitems = new List<DownDeliverTradeRequest.Listitem>();
                foreach (DataRow item in detail.Rows)
                {
                    DownDeliverTradeRequest.Listitem listitem = new DownDeliverTradeRequest.Listitem();
                    listitem.originalOrderId = item["originalOrderId"] == DBNull.Value ? string.Empty : item["originalOrderId"].ToString();
                    listitem.productName = item["productName"] == DBNull.Value ? string.Empty : item["productName"].ToString();
                    listitem.productNumber = item["productNumber"] == DBNull.Value ? string.Empty : item["productNumber"].ToString();
                    listitem.skuNumber = item["skuNumber"] == DBNull.Value ? string.Empty : item["skuNumber"].ToString();
                    listitem.skuName = item["skuName"] == DBNull.Value ? string.Empty : item["skuName"].ToString();
                    listitem.num = item["num"] == DBNull.Value ? string.Empty : item["num"].ToString();
                    listitems.Add(listitem);
                }
                downDeliverTradeRequest.listItem = listitems;

                Log.Logs.WriteJsonLogForDebug(string.Format("单据号{0}调用订单同步至订单中台接口提交数据", paras.jobCode), downDeliverTradeRequest);
                DownDeliverTradeResponse downDeliverTradeResponse = client.Execut(downDeliverTradeRequest, ref errorMessage);
                Log.Logs.WriteJsonLogForDebug(string.Format("单据号{0}调用订单同步至订单中台接口返回数据", paras.jobCode), downDeliverTradeResponse);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    errorMsg = errorMessage;
                    return false;
                }
                if (downDeliverTradeResponse.httpcode.Equals(200) || downDeliverTradeResponse.httpcode.Equals(204))
                {
                    Log.Logs.WriteDXLog("调用订单同步至订单中台接口成功");
                }
                else
                {
                    errorMsg = downDeliverTradeResponse.httpmsg;
                    return false;
                }
                #endregion

                #region 订单批次同步至订单中台
                string synBatchError = string.Empty;

                foreach (DataRow item in detail.Rows)
                {
                    errorMessage = string.Empty;
                    //子数据
                    List<SynBatchRequest.Batchs> batchslist = new List<SynBatchRequest.Batchs>();
                    SynBatchRequest.Batchs batchs = new SynBatchRequest.Batchs();
                    batchs.no = item["NO"] == DBNull.Value ? string.Empty : item["NO"].ToString();
                    if (item["NUM"] != DBNull.Value)
                    {
                        batchs.num = item["NUM"].ToString();
                    }
                    batchs.batch = item["BATCH"] == DBNull.Value ? string.Empty : item["BATCH"].ToString();
                    batchslist.Add(batchs);
                    //主数据
                    SynBatchRequest synBatchRequest = new SynBatchRequest();
                    synBatchRequest.batchs = batchslist;
                    synBatchRequest.originalTradeId = main.Rows[0]["originalTradeId"] == DBNull.Value ? string.Empty : main.Rows[0]["originalTradeId"].ToString();
                    synBatchRequest.originalOrderId = item["originalOrderId"] == DBNull.Value ? string.Empty : item["originalOrderId"].ToString();
                    Log.Logs.WriteJsonLogForDebug(string.Format("单据号{0}调用订单批次同步至订单中台接口提交数据", paras.jobCode), synBatchRequest);
                    SynBatchResponse synBatchResponse = client.Execut(synBatchRequest, ref errorMessage);
                    Log.Logs.WriteJsonLogForDebug(string.Format("单据号{0}调用订单批次同步至订单中台接口返回数据", paras.jobCode), synBatchResponse);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        synBatchError += string.Format("子订单号为[{0}]的单据上传失败，失败原因：{1} \r\n", item["originalOrderId"], errorMessage);
                    }
                    if (synBatchResponse.httpcode == Constants.ZT_SUCCESS_CODE_1 || synBatchResponse.httpcode == Constants.ZT_SUCCESS_CODE_1)
                    {
                        Log.Logs.WriteDXLog("调用订单批次同步至订单中台接口成功");
                    }
                    else
                    {
                        synBatchError += string.Format("子订单号为[{0}]的单据上传失败，失败原因：{1} \r\n", item["originalOrderId"], synBatchResponse.httpmsg);
                        errorMsg = string.Empty;
                        return true;
                    }
                }

                if (string.IsNullOrEmpty(synBatchError))
                {
                    errorMsg = "";
                    return true;
                }
                else
                {
                    errorMsg = "部分批次信息上传失败 相信信息：\r\n" + synBatchError;
                    return false;
                }
                #endregion

            }
            //积分变更
            else if (paras.jobCode.Equals("10002"))
            {
                DataTable main = dsData.Tables["Head"];
                Hashtable bodyDic = new Hashtable();

                DataRow dataRow = main.Rows[0];

                bodyDic.Add("phone", dataRow["phone"] == DBNull.Value ? string.Empty : dataRow["phone"].ToString());//13955555555  //17086392879
                //增减类型 增：add 减：sub
                bodyDic.Add("calType", dataRow["calType"] == DBNull.Value ? string.Empty : dataRow["calType"].ToString());
                //积分变化时间
                bodyDic.Add("exchangeTime", dataRow["exchangeTime"] == DBNull.Value ? string.Empty : dataRow["exchangeTime"].ToString());
                //变化的积分
                bodyDic.Add("exchangPoints", dataRow["exchangPoints"] == DBNull.Value ? string.Empty : dataRow["exchangPoints"].ToString());
                //积分变化类型
                bodyDic.Add("pointsReason", dataRow["pointsReason"] == DBNull.Value ? string.Empty : dataRow["pointsReason"].ToString());
                //积分变化说明
                bodyDic.Add("pointsReasonName", dataRow["pointsReasonName"] == DBNull.Value ? string.Empty : dataRow["pointsReasonName"].ToString());
                //订单编号
                bodyDic.Add("orderNo", dataRow["orderNo"] == DBNull.Value ? string.Empty : dataRow["orderNo"].ToString());
                //bodyDic.Add("invalidTime", "2018-09-20 00:00:00");

                Log.Logs.WriteJsonLogForDebug("积分变更上传数据", bodyDic);
                TRT_AliVipTicketSDK.VipInfo vipInfo = new VipInfo();
                Rootobject result = vipInfo.MemberPointUpdate(bodyDic);
                Log.Logs.WriteJsonLogForDebug("积分变更返回数据", result);

                if (result.httpcode == Constants.ZT_SUCCESS_CODE_1 || result.httpcode == Constants.ZT_SUCCESS_CODE_2)
                {
                    int tag = result.data.isSuccess;
                    Log.Logs.WriteDXLog(string.Format("会员积分变更成功，成功状态为：{0}", tag));
                    errorMsg = "";
                    return true;
                }
                else if (result.httpcode == Constants.ZT_ERROR_CODE)
                {
                    Log.Logs.WriteDXLog(string.Format("会员积分变更失败，原因是：\r\n{0}", result.httpmsg));
                    errorMsg = result.httpmsg;
                    return false;
                }
            }
            //库存信息同步
            else if (paras.jobCode.Equals("10003") || paras.jobCode.Equals("10004") || paras.jobCode.Equals("10005"))
            {
                DataTable main = dsData.Tables["Head"];
                DataRow mainRow = main.Rows[0];
                StockSyncAPI stockSyncAPI = new StockSyncAPI();
                #region 数据组装
                StockSync stockSync = new StockSync();
                //线上商品sku
                stockSync.onlineProductSkuId = mainRow["onlineProductSkuId"] == DBNull.Value ? string.Empty : mainRow["onlineProductSkuId"].ToString();
                //中台仓库id
                stockSync.warehouseId = mainRow["warehouseId"] == DBNull.Value ? string.Empty : mainRow["warehouseId"].ToString();
                //商品编码
                stockSync.productNumber = mainRow["productNumber"] == DBNull.Value ? string.Empty : mainRow["productNumber"].ToString();
                //安全库存
                stockSync.safetyStock = mainRow["safetyStock"] == DBNull.Value ? string.Empty : mainRow["safetyStock"].ToString();
                //店铺id
                stockSync.shopId = mainRow["shopId"] == DBNull.Value ? string.Empty : mainRow["shopId"].ToString();
                //是否是自动售药机 1-是 2-不是
                stockSync.isMachineWarehouse = "2";

                
                List<SkuDetail> skuDetails = new List<SkuDetail>();

                SkuDetail skuDetail = new SkuDetail();
                //商品skunumber
                skuDetail.skuNumber = mainRow["onlineProductSkuId"] == DBNull.Value ? string.Empty : mainRow["onlineProductSkuId"].ToString();
                //数量
                skuDetail.quantity = mainRow["quantity"] == DBNull.Value ? string.Empty : mainRow["quantity"].ToString();
                //同步模式 1-全量 2-增量
                skuDetail.syncType = mainRow["syncType"] == DBNull.Value ? string.Empty : mainRow["syncType"].ToString();
                //同步时间
                skuDetail.syncDate = mainRow["syncDate"] == DBNull.Value ? string.Empty : mainRow["syncDate"].ToString();
                //单据号
                skuDetail.receiptNo = mainRow["receiptNo"] == DBNull.Value ? string.Empty : mainRow["receiptNo"].ToString();
                //单据类型
                skuDetail.receiptType = mainRow["receiptType"] == DBNull.Value ? string.Empty : mainRow["receiptType"].ToString();
                skuDetails.Add(skuDetail);
                stockSync.skus = skuDetails;
                #endregion
                Log.Logs.WriteJsonLogForDebug("库存信息同步入参", stockSync);
                Dictionary<string,string> result = stockSyncAPI.StockSync(ZTHost, ZTAppId, ZTAppSecret, stockSync);
                Log.Logs.WriteJsonLogForDebug("返回结果", result);
                if (result["Code"] == "00")
                {
                    errorMsg = "";
                    return true;
                }
                else
                {
                    errorMsg = result["Msg"];
                    return false;
                }
            }
            //储值卡充值
            else if (paras.jobCode.Equals("10006"))
            {
                DataTable main = dsData.Tables["Head"];
                DataRow dataRow = main.Rows[0];
                Hashtable ht = new Hashtable();
                //手机号
                ht.Add("phone", dataRow["phone"] == DBNull.Value ? string.Empty : dataRow["phone"].ToString());
                //储值金额
                ht.Add("balance_cash", dataRow["balance_cash"] == DBNull.Value ? string.Empty : dataRow["balance_cash"].ToString());
                //ht.Add("card_no", "");
                //渠道
                ht.Add("channel", dataRow["channel"] == DBNull.Value ? string.Empty : dataRow["channel"].ToString());
                //外部业务单号
                ht.Add("biz_id", dataRow["biz_id"] == DBNull.Value ? string.Empty : dataRow["biz_id"].ToString());

                VipInfo vipInfo = new VipInfo();

                Rootobject result = vipInfo.BalanceRecharge(ht);
                if (result.httpcode == Constants.ZT_SUCCESS_CODE_1 || result.httpcode == Constants.ZT_SUCCESS_CODE_2)
                {
                    Log.Logs.WriteDXLog(string.Format("会员储值卡进行充值成功\r\n成功状态为：{0}", result.data.isSuccess));
                    errorMsg = "";
                    return true;
                }
                else if (result.httpcode == Constants.ZT_ERROR_CODE)
                {
                    Log.Logs.WriteDXLog(string.Format("会员储值卡进行充值失败\r\n原因是：{0}", result.httpmsg));
                    errorMsg = result.httpmsg;
                    return false;
                }
            }
            //失败逆订单
            else if (paras.jobCode.Equals("10007"))
            {
                DataTable main = dsData.Tables["Head"];
                DataTable detail = dsData.Tables["Detail"];
                DataRow dataRow = main.Rows[0];

                VipInfo vipInfo = new VipInfo();
                #region 主数据
                Rootobject45 mainItems = new Rootobject45();
                mainItems.refundNumber = dataRow["refundNumber"] == DBNull.Value ? string.Empty : dataRow["refundNumber"].ToString();
                if (dataRow["type"] != DBNull.Value)
                {
                    mainItems.type = Convert.ToInt32(dataRow["type"]);
                }
                mainItems.originalTradeId = dataRow["originalTradeId"] == DBNull.Value ? string.Empty : dataRow["originalTradeId"].ToString();
                if (dataRow["channelId"] != DBNull.Value)
                {
                    mainItems.channelId = Convert.ToInt32(dataRow["channelId"]);
                }
                if (dataRow["refundFee"] != DBNull.Value)
                {
                    mainItems.refundFee = Convert.ToDouble(dataRow["refundFee"]);
                }
                if (dataRow["refundAmount"] != DBNull.Value)
                {
                    mainItems.refundAmount = Convert.ToDouble(dataRow["type"]);
                }
                if (dataRow["status"] != DBNull.Value)
                {
                    mainItems.status = Convert.ToInt32(dataRow["type"]);
                }
                mainItems.applyTime = dataRow["applyTime"] == DBNull.Value ? string.Empty : dataRow["applyTime"].ToString();
                if (dataRow["refundType"] != DBNull.Value)
                {
                    mainItems.refundType = Convert.ToInt32(dataRow["refundType"]);
                }

                //下面的是固定值。
                mainItems.address = "";
                mainItems.buyerNick = "";
                mainItems.cityAreaName = "";
                mainItems.cityName = "";
                mainItems.consignee = "";
                mainItems.description = "";
                mainItems.expressNumber = "";
                mainItems.goodsStatus = 0;
                mainItems.initiator = 0;
                mainItems.logistic = "";
                mainItems.mobile = "";
                mainItems.postageBearer = "";
                mainItems.reason = "";
                mainItems.returnMessag = "";
                mainItems.salesOrderStatus = 0;
                mainItems.sourceId = 0;
                mainItems.provinceName = "";
                mainItems.telephone = "";
                mainItems.zipCode = "";

                #endregion

                #region 子数据
                List<Refunditem> detailItems = new List<Refunditem>();
                foreach (DataRow itemRow in detail.Rows)
                {
                    Refunditem refunditem = new Refunditem();
                    refunditem.originalOrderId = itemRow["originalOrderId"] == DBNull.Value ? string.Empty : itemRow["originalOrderId"].ToString();
                    refunditem.productName = itemRow["productName"] == DBNull.Value ? string.Empty : itemRow["productName"].ToString();
                    refunditem.productNumber = itemRow["productNumber"] == DBNull.Value ? string.Empty : itemRow["productNumber"].ToString();
                    refunditem.skuName = itemRow["skuName"] == DBNull.Value ? string.Empty : itemRow["skuName"].ToString();
                    refunditem.skuNumber = itemRow["skuNumber"] == DBNull.Value ? string.Empty : itemRow["skuNumber"].ToString();
                    if (itemRow["num"] != DBNull.Value)
                    {
                        refunditem.num = Convert.ToInt32(itemRow["num"]);
                    }
                    if (itemRow["totalFee"] != DBNull.Value)
                    {
                        refunditem.totalFee = Convert.ToInt32(itemRow["totalFee"]);
                    }
                    //下面是固定值。
                    refunditem.productPicUrl = "";
                    detailItems.Add(refunditem);
                }
                #endregion

                mainItems.refundItem = detailItems.ToArray();
                bool b = vipInfo.ReverseOrder(mainItems, out string error);
                if (string.IsNullOrEmpty(error))
                {
                    errorMsg = "";
                    return true;
                }
                else
                {
                    Log.Logs.WriteDXLog(error);
                    errorMsg = error;
                    return false;
                }
            }

            errorMsg = "";
            return true;
        }

        #region 业务中台接口公共参数
        public static string ZTHost { get; private set; } = ConfigurationManager.AppSettings["zthost"].ToString();
        public static string ZTAppId { get; private set; } = ConfigurationManager.AppSettings["ztappid"].ToString();
        public static string ZTAppSecret { get; private set; } = ConfigurationManager.AppSettings["ztappsecret"].ToString();
        //<add key = "zthost" value="http://39.96.168.75:8001"/>
        //<add key = "ztappid" value="TRT-oHWFfRWjhIOfiOT8"/>
        //<add key = "ztappsecret" value="d9nmtMZ5G3WP81NESX"/>
        #endregion

        #region 业务中台配置
        public class ZTConfig : IConfig
        {
            public string Host => ZTHost;

            public string Url => "";

            public string AppId => ZTAppId;

            public string AppSecret => ZTAppSecret;
        }
        #endregion
    }
}
