using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.TRTBusinessZT.Sign
{
    public class SigntureHelp
    {
        public static string GetSignture(string appid, string appsecret, string timestamp)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("app_id", appid);
            data.Add("app_secret", appsecret);
            data.Add("timestamp", timestamp);

            string[] strdata = data.Values.ToArray();
            Array.Sort(strdata, string.CompareOrdinal);
            StringBuilder sb = new StringBuilder();
            foreach (string key in strdata)
            {
                sb.Append(key);
            }
            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] hashArray = System.Security.Cryptography.SHA1.Create().ComputeHash(buffer);
            return BitConverter.ToString(hashArray).Replace("-", "").ToLower();
        }

    }
}
