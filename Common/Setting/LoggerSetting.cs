namespace Common.Setting
{
    public class LoggerSetting : SettingBase
    {
        public Log.LogLevel ConsoleWriteLevel { get; set; }

        public Log.LogLevel FileWriteLevel { get; set; }

        public bool IsWriteToFile { get; set; }
    }
}
