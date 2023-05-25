using System;
using System.Collections.Generic;
using System.Text;
using Server.Common;

namespace Server.Log
{
    public class LoggerSetting : SettingBase
    {
        public LogLevel ConsoleWriteLevel { get; set; }

        public LogLevel FileWriteLevel { get; set; }

        public bool IsWriteToFile { get; set; }
    }
}
