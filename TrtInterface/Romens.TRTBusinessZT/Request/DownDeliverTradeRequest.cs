using Newtonsoft.Json;
using ERP.TRTBusinessZT.Response;
using ERP.TRTBusinessZT.Sign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ERP.TRTBusinessZT.Request
{
    public class DownDeliverTradeRequest : IBaseRequest<DownDeliverTradeResponse>
    {
        /// <summary>
        /// 订单明细
        /// </summary>
        public List<Listitem> listItem { get; set; }
        /// <summary>
        /// 订单编号*
        /// </summary>
        public string originalTradeId { get; set; }
        /// <summary>
        /// 交易编号*
        /// </summary>
        public string serialNumber { get; set; }
        /// <summary>
        /// 订单类型*（1. 实物订单 2 服务订单）
        /// </summary>
        public int salesOrderType { get; set; }
        /// <summary>
        /// 履约方式*(1门店发货 2 客户自提 3电商发货7售药机自提)
        /// </summary>
        public int deliveryType { get; set; }
        /// <summary>
        /// 订单状态*(0 已下单
        /// 1 待付款
        /// 2 已付款（电商同步）
        /// 3 未接单
        /// 4 待拣选
        /// 5 待备货
        /// 6 待发货
        /// 8 待提货
        /// 9 待收货
        /// 10 已完成（pos同步）
        ///11 已关闭)
        /// </summary>
        public int salesOrderStatus { get; set; }
        /// <summary>
        /// 订单来源(1 APP  2 小程序 3.微商城 4.PC端)
        /// </summary>
        public int platFormCode { get; set; }
        /// <summary>
        /// 下单时间*
        /// </summary>
        public string orderDate { get; set; }
        /// <summary>
        /// 导购员id
        /// </summary>
        public int guideId { get; set; }
        /// <summary>
        /// 会员账号(手机号)
        /// </summary>
        public long memberAccount { get; set; }
        /// <summary>
        /// 商品金额*
        /// </summary>
        public decimal totalFee { get; set; }
        /// <summary>
        /// 优惠券
        /// </summary>
        public int couponFee { get; set; }
        /// <summary>
        /// 优惠券编码
        /// </summary>
        public int couponCode { get; set; }
        /// <summary>
        /// 积分抵扣
        /// </summary>
        public int pointDeduction { get; set; }
        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal discountAmount { get; set; }
        /// <summary>
        /// 活动优惠
        /// </summary>
        public decimal eventOffer { get; set; }
        /// <summary>
        /// 母单总优惠金额(精确到分)
        /// </summary>
        public string discountFee { get; set; }
        /// <summary>
        /// 优惠备注信息
        /// </summary>
        public int discountMemo { get; set; }
        /// <summary>
        /// 实付金额*
        /// </summary>
        public decimal payment { get; set; }
        /// <summary>
        /// 子单数量*
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 收货人名称
        /// </summary>
        public string consignee { get; set; }
        /// <summary>
        /// 收货人手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 收货地址所属省
        /// </summary>
        public string provinceName { get; set; }
        /// <summary>
        /// 收货地址所属市
        /// </summary>
        public string cityName { get; set; }
        /// <summary>
        /// 收货地址所属县/区
        /// </summary>
        public string cityAreaName { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public int zipCode { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string logistic { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string expressNumber { get; set; }
        /// <summary>
        /// 支付类型01 个人网银，02 快捷，03 单笔代扣，04 单笔代付，11 二维码正扫 12 二维码反扫， 94 小程序支付， 95 H5支付 ，96 APP支付 
        /// </summary>
        public int paymentType { get; set; }
        /// <summary>
        /// 买家备注(商城无买家留言和卖家备注)
        /// </summary>
        public string buyerMemo { get; set; }
        /// <summary>
        /// 支付时间*
        /// </summary>
        public string payTime { get; set; }
        /// <summary>
        /// 物流费用（0）
        /// </summary>
        public decimal expressCost { get; set; }
        /// <summary>
        /// 订单完结时间
        /// </summary>
        public string endTime { get; set; }
        /// <summary>
        /// 渠道id*(1 OMO商城渠道 2 新分销渠道 3 新零售门店要货渠道 4 云餐饮)
        /// </summary>
        public string channelId { get; set; }
        /// <summary>
        /// 是否有开发票(0 否 1 是)
        /// </summary>
        public int invoiceFlag { get; set; }
        /// <summary>
        /// 发票类型（1 个人发票 2 普通发票 3 专用发票）
        /// </summary>
        public int invoiceType { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string invoiceTitle { get; set; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string taxpayerIdentificationNumber { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string registeredAddress { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string invoiceContact { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string invoiceBank { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string bankAccount { get; set; }
        /// <summary>
        /// 派单门店/门店编号
        /// </summary>
        public string storeId { get; set; }
        /// <summary>
        /// 自提码
        /// </summary>
        public string sendCode { get; set; }
        /// <summary>
        /// 自提日期
        /// </summary>
        public string sinceDate { get; set; }
        /// <summary>
        /// 自提时间
        /// </summary>
        public string sinceTime { get; set; }
        /// <summary>
        /// 自提售药机（区分哪个售药机）
        /// </summary>
        public string sinceDrugMachine { get; set; }
        /// <summary>
        /// 提药时间限制
        /// </summary>
        public string timeLimit { get; set; }
        /// <summary>
        /// 渠道订单数据json大字段(云餐饮需要)
        /// </summary>
        public string tradeJson { get; set; }

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
            return "/v1/oms/order/original/trade/downDeliverTrade";
        }

        public class Listitem
        {
            /// <summary>
            /// 子单编号*
            /// </summary>
            public string originalOrderId { get; set; }
            /// <summary>
            /// 商品名称*
            /// </summary>
            public string productName { get; set; }
            /// <summary>
            /// 商品编号*
            /// </summary>
            public string productNumber { get; set; }
            /// <summary>
            /// sku编号*
            /// </summary>
            public string skuNumber { get; set; }
            /// <summary>
            /// 规格名称*
            /// </summary>
            public string skuName { get; set; }
            /// <summary>
            /// 规格图片
            /// </summary>
            public string skuUrl { get; set; }
            /// <summary>
            /// 子单交易商品数量*
            /// </summary>
            public string num { get; set; }
            /// <summary>
            /// 商品单价（精确到分）
            /// </summary>
            public decimal price { get; set; }
            /// <summary>
            /// 商品总金额（精确到分）
            /// </summary>
            public decimal totalFee { get; set; }
            /// <summary>
            /// 子单优惠券
            /// </summary>
            public decimal couponFee { get; set; }
            /// <summary>
            /// 子单积分抵扣
            /// </summary>
            public decimal pointDeduction { get; set; }
            /// <summary>
            /// 子单活动优惠
            /// </summary>
            public decimal eventOffer { get; set; }
            /// <summary>
            /// 子单折扣金额
            /// </summary>
            public decimal discountAmount { get; set; }
            /// <summary>
            /// 子单优惠金额（精确到分）
            /// </summary>
            public decimal discountFee { get; set; }
            /// <summary>
            /// 子单实付金额(精确到分)
            /// </summary>
            public decimal payment { get; set; }
        }
    }
}
