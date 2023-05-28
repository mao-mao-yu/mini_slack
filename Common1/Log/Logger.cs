using System;
using System.IO;
using System.Text;
using Common.Setting;

namespace Common.Log
{
    /// <summary>
    /// Logger
    /// </summary>
    public static class Logger
    {
        #region Fields
        /// <summary>
        /// Lock
        /// </summary>
        private static readonly object _lockObj = new object();

        /// <summary>
        /// 配置文件字典
        /// </summary>
        private static readonly LoggerSetting setting = SettingBase.LoadSetting<LoggerSetting>(Const.LOGGER_SETTING_PATH);

        /// <summary>
        /// 是否写入文件
        /// </summary>
        private static readonly bool _isWriteToFile = setting.IsWriteToFile;

        /// <summary>
        /// 控制台输出等级
        /// </summary>
        private static readonly LogLevel _consoleWriteLevel = setting.ConsoleWriteLevel;

        /// <summary>
        /// 文件输出等级
        /// </summary>
        private static readonly LogLevel _fileWriteLevel = setting.FileWriteLevel;
        #endregion

        #region Write to console
        /// <summary>
        /// 控制台输出
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public static void WriteMessage(string message, LogLevel logLevel)
        {
            if ((int)_consoleWriteLevel == (int)LogLevel.OFF)
            {
                return;
            }
            if ((int)logLevel < (int)_consoleWriteLevel)
            {
                return;
            }
            Console.WriteLine($"[{GetDayStr()}] [{logLevel}] : {message}");
        }

        public static void ALL(string msg)
        {
            WriteMessage(msg, LogLevel.ALL);
        }

        public static void DEBUG(string msg)
        {
            WriteMessage(msg, LogLevel.DEBUG);
        }

        public static void INFO(string msg)
        {
            WriteMessage(msg, LogLevel.INFO);
        }

        public static void WARNING(string msg)
        {
            WriteMessage(msg, LogLevel.WARNING);
        }

        public static void ERROR(string msg)
        {
            WriteMessage(msg, LogLevel.ERROR);
        }
        #endregion

        #region Write exception to file
        /// <summary>
        /// 控制台输出
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public static void WriteMessage(string message, Exception e, LogLevel logLevel)
        {
            message = $"[{ GetDayStr()}] [{logLevel}] : Message : {message}\nException message : {e.Message}\nStackTrace : {e.StackTrace}";
            WriteMessage(message, logLevel);
        }

        public static void ALL(string msg, Exception e)
        {
            WriteMessage(msg, e, LogLevel.ALL);
        }

        public static void DEBUG(string msg, Exception e)
        {
            WriteMessage(msg, e, LogLevel.DEBUG);
        }

        public static void INFO(string msg, Exception e)
        {
            WriteMessage(msg, e, LogLevel.INFO);
        }

        public static void WARNING(string msg, Exception e)
        {
            WriteMessage(msg, e, LogLevel.WARNING);
        }

        public static void ERROR(string msg, Exception e)
        {
            WriteMessage(msg, e, LogLevel.ERROR);
        }
        #endregion

        #region Write to file
        public static void FALL(string msg)
        {
            WriteFileMessage(msg, LogLevel.ALL);
        }

        public static void FDEBUG(string msg)
        {
            WriteFileMessage(msg, LogLevel.DEBUG);
        }

        public static void FINFO(string msg)
        {
            WriteFileMessage(msg, LogLevel.INFO);
        }

        public static void FWARNING(string msg)
        {
            WriteFileMessage(msg, LogLevel.WARNING);
        }

        public static void FERROR(string msg)
        {
            WriteFileMessage(msg, LogLevel.ERROR);
        }

        public static void WriteFileMessage(string message, LogLevel logLevel)
        {
            if ((int)_fileWriteLevel == (int)LogLevel.OFF)
            {
                return;
            }
            if ((int)logLevel < (int)_fileWriteLevel)
            {
                return;
            }
            if (_isWriteToFile)
            {
                FileWriter(message, logLevel);
            }
        }
        #endregion

        #region Write exception to file
        public static void FALL(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.ALL);
        }

        public static void FDEBUG(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.DEBUG);
        }

        public static void FINFO(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.INFO);
        }

        public static void FWARNING(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.WARNING);
        }

        public static void FERROR(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.ERROR);
        }

        public static void WriteError(Exception ex, string msg, LogLevel logLevel)
        {
            if ((int)_fileWriteLevel == (int)LogLevel.OFF)
            {
                return;
            }
            if ((int)logLevel < (int)_fileWriteLevel)
            {
                return;
            }
            if (_isWriteToFile)
            {
                FileWriter($"Message : {msg}\nException message : {ex.Message}\nStackTrace : {ex.StackTrace}", logLevel);
            }
        }
        #endregion

        #region Common
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        private static string GetDayStr()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff");
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool FileWriter<T>(string msg, T level)
        {
            DateTime dateTime = DateTime.Now;                                                                                           // 今の時間を更新
            string fileName = $"{dateTime:yyyy-MM-dd HH}：{dateTime.Minute / 10 * 10}~{dateTime.Minute / 10 * 10 + 10}.txt";                      // テキストファイルネーム
            (string dayFolderPath, string hourFolderPath) = SetPath();
            CheckFileExists(dayFolderPath, hourFolderPath);
            string filePath = Path.Combine(hourFolderPath, fileName);

            lock (_lockObj)
            {
                try
                {
                    using (FileStream file = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
                        {
                            string dumpMsg = string.Concat($"[{GetDayStr()}] ", $"[{level}] ", msg);
                            writer.WriteLine(dumpMsg);
                            DEBUG($"Written to file : {fileName}...");
                        }
                    }
                }
                catch (Exception e)
                {
                    ERROR("Log writing error...", e);
                }
            }
            return true; // 表示文件写入成功
        }

        private static (string, string) SetPath()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");                                            // 日付
            string hour = DateTime.Now.ToString("HH");                                                     // 時間

            string dayFolderPath = Path.Combine(Const.LOG_FOLDER_PATH, today);                          // 日付フォルダー
            string hourFolderPath = Path.Combine(dayFolderPath, hour + "H");                            // 時間区分フォルダー

            return (dayFolderPath, hourFolderPath);
        }

        private static void CheckFileExists(string dayFolderPath, string hourFolderPath)
        {
            if (!Directory.Exists(dayFolderPath))
            {
                DEBUG($"{dayFolderPath} is not exists. Will create...");
                Directory.CreateDirectory(dayFolderPath);
                DEBUG($"{dayFolderPath} is not exists. Folder is created");
            }

            if (!Directory.Exists(hourFolderPath))
            {
                DEBUG($"{hourFolderPath} is not exists. Will create...");
                Directory.CreateDirectory(hourFolderPath);
                DEBUG($"{hourFolderPath} is not exists. Folder is created");
            }
        }
        #endregion
    }

}
