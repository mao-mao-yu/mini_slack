using System.IO;

namespace Common
{
    public static class Const
    {
        /// <summary>
        /// Working directory
        /// </summary>
        public static readonly string WORKING_DIRECTORY = Directory.GetCurrentDirectory();

        /// <summary>
        /// Log folder path
        /// </summary>
        public static readonly string LOG_FOLDER_PATH = Path.Combine(WORKING_DIRECTORY, "Log");

        /// <summary>
        /// Setting folder path
        /// </summary>
        public static readonly string SETTING_FOLDER_PATH = Path.Combine(WORKING_DIRECTORY,"Setting");

        /// <summary>
        /// Server setting path
        /// </summary>
        public static readonly string SERVER_SETTING_PATH = Path.Combine(SETTING_FOLDER_PATH, "ServerSetting.json");

        /// <summary>
        /// Server setting path
        /// </summary>
        public static readonly string CLIENT_SETTING_PATH = Path.Combine(SETTING_FOLDER_PATH, "ClientSetting.json");

        /// <summary>
        /// Logger setting path
        /// </summary>
        public static readonly string LOGGER_SETTING_PATH = Path.Combine(SETTING_FOLDER_PATH, "LoggerSetting.json");

        /// <summary>
        /// Int size
        /// </summary>
        public const int INT_SIZE = sizeof(int);

        /// <summary>
        /// RSA_PUBLICKEY_SIZE
        /// </summary>
        public const int RSA_PUBLICKEY_SIZE = 415;

        /// <summary>
        /// GUID_SIZE
        /// </summary>
        public const int GUID_SIZE = 36;
    }
}
