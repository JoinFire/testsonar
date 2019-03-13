using RomensInterfaceExtension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TrtInterface.Iot;
using TrtInterface.Log;

namespace TrtInterface
{
    public class DownJob : IDownJob
    {
        public bool ExecJob(JobPara paras, out string errorMsg)
        {
            if (paras.jobCode.Equals("20001"))
            {
                IotHelp iotHelp = new IotHelp();
                if (iotHelp.Operation())
                {
                    errorMsg = "";
                    return true;
                }
                else
                {
                    errorMsg = "";
                    return false;
                }
            }
            Logs.WriteDXLog("下载任务");
            errorMsg = "";
            return true;
        }
    }
}
