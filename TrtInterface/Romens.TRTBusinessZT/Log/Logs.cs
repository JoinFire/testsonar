using NewLife.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.TRTBusinessZT
{
    public class Logs
    {
        public static void WriteLogForDebug(string flag, string message)
        {
            string writeMessage = FormatWriteLog(message);
            //logger.LogDebugInfo(flag, writeMessage);
            WriteDXLog("[{0}]{1}", flag, writeMessage);
        }

        public static void WriteJsonLogForDebug(string flag, object value)
        {
            string writeMessage = FormatWriteLog(JsonConvert.SerializeObject(value));
            //logger.LogDebugInfo(flag, writeMessage);
            WriteDXLog("[{0}]{1}", flag, writeMessage);
        }


        public static void WriteLogForError(Exception ex)
        {
            // logger.LogException(ex);
            WriteEXLog("错误[{0}]{1}", ex.Source, ex.Message);
        }

        public static string FormatWriteLog(string message)
        {
            string writeMessage = string.Empty;
            if (!string.IsNullOrEmpty(message))
            {
                writeMessage = message.Replace("'", "''");
            }
            return writeMessage;
        }

        public static void WriteSQLXLog(string sqlType, string sqlString)
        {
            WriteDXLog("[SQL][{0}] {1}", sqlType, sqlString);
        }

        public static void WriteDXLog(string message, params object[] format)
        {
            //if (CloudApiSetting.Current.EnabdleDebug)
            XTrace.Log.Info(message, format);
        }

        public static void WriteEXLog(string message, params object[] format)
        {
            XTrace.Log.Error(message, format);
        }

        public static void WriteIXLog(string message, params object[] format)
        {
            XTrace.Log.Info(message, format);
        }

        public static void WriteIWLog(string message, params object[] format)
        {
            XTrace.Log.Warn(message, format);
        }
    }
}
