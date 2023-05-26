using Server.Log;

namespace Server.Setting
{
    public class LoggerSetting : SettingBase
    {
        public LogLevel ConsoleWriteLevel { get; set; }

        public LogLevel FileWriteLevel { get; set; }

        public bool IsWriteToFile { get; set; }
    }
}
