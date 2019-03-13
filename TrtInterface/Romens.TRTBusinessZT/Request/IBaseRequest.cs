using ERP.TRTBusinessZT.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.TRTBusinessZT.Request
{
    public interface IBaseRequest<T> where T: BaseResponse
    {
        /// <summary> 
        /// 获取访问方式
        /// </summary>
        /// <returns></returns>
        string GetMethod();
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        string GetParam();

        string GetUrl();
    }
}
