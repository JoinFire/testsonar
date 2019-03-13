using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.TRTBusinessZT.Config
{
    public interface IConfig
    {
        string Host { get; }
        string Url { get; }
        string AppId { get; }
        string AppSecret { get; }
    }
}
