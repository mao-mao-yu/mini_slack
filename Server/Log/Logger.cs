using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Server.Common;

namespace Server.Log
{
    /// <summary>
    /// Logger
    /// </summary>
    public class Logger
    {
        #region Fields
        /// <summary>
        /// 本地工作目录
        /// </summary>
        private static string _currentPath;

        /// <summary>
        /// Lock
        /// </summary>
        private static readonly object _lockObj = new object();

        /// <summary>
        /// log文件夹输出目录
        /// </summary>
        private readonly string _logFolderPath;

        /// <summary>
        /// 当前日期时间
        /// </summary>
        private DateTime _dateTime;

        /// <summary>
        /// 配置文件字典
        /// </summary>
        private LoggerSetting setting;

        /// <summary>
        /// 是否写入文件
        /// </summary>
        private bool _isWriteToFile;

        /// <summary>
        /// 控制台输出等级
        /// </summary>
        private readonly LogLevel _consoleWriteLevel;

        /// <summary>
        /// 文件输出等级
        /// </summary>
        private readonly LogLevel _fileWriteLevel;
        #endregion

        #region Ctor
        public Logger()
        {
            // 获取当前工作路径
            _currentPath = Directory.GetCurrentDirectory();
            // Log输出路径
            _logFolderPath = Path.Combine(_currentPath, "Log");
            // 配置文件路径
            string settingPath = Path.Combine(_currentPath, "Config","LoggerSetting.json");
            setting = SettingBase.LoadSetting<LoggerSetting>(settingPath);
            _consoleWriteLevel = setting.ConsoleWriteLevel;
            _fileWriteLevel = setting.FileWriteLevel;
            _isWriteToFile = setting.IsWriteToFile;
        }
        #endregion

        #region To console
        /// <summary>
        /// 控制台输出
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public void WriteMessage(string message, LogLevel logLevel)
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

        public void ALL(string msg)
        {
            WriteMessage(msg, LogLevel.ALL);
        }

        public void DEBUG(string msg)
        {
            WriteMessage(msg, LogLevel.DEBUG);
        }

        public void INFO(string msg)
        {
            WriteMessage(msg, LogLevel.INFO);
        }

        public void WARNING(string msg)
        {
            WriteMessage(msg, LogLevel.WARNING);
        }

        public void ERROR(string msg)
        {
            WriteMessage(msg, LogLevel.ERROR);
        }
        #endregion

        #region Write to file
        public void FALL(string msg)
        {
            WriteFileMessage(msg, LogLevel.ALL);
        }

        public void FDEBUG(string msg)
        {
            WriteFileMessage(msg, LogLevel.DEBUG);
        }

        public void FINFO(string msg)
        {
            WriteFileMessage(msg, LogLevel.INFO);
        }

        public void FWARNING(string msg)
        {
            WriteFileMessage(msg, LogLevel.WARNING);
        }

        public void FERROR(string msg)
        {
            WriteFileMessage(msg, LogLevel.ERROR);
        }

        public void WriteFileMessage(string message, LogLevel logLevel)
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

        #region Exception To File
        public void FALL(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.ALL);
        }

        public void FDEBUG(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.DEBUG);
        }

        public void FINFO(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.INFO);
        }

        public void FWARNING(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.WARNING);
        }

        public void FERROR(Exception e, string msg)
        {
            WriteError(e, msg, LogLevel.ERROR);
        }

        public void WriteError(Exception ex, string msg, LogLevel logLevel)
        {
            if ((int)_fileWriteLevel == (int)LogLevel.OFF)
            {
                return;
            }
            if((int)logLevel < (int)_fileWriteLevel)
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
        private void GetNowTime()
        {
            _dateTime = DateTime.Now;
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        private string GetDayStr()
        {
            GetNowTime();
            return _dateTime.ToString("yyyy-MM-dd HH:mm:ss ffff");
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool FileWriter<T>(string msg, T level)
        {
            GetNowTime();                                                                                           // 今の時間を更新
            string fileName = $"{_dateTime:yyyy-MM-dd HH}：{_dateTime.Minute / 10 * 10}~{_dateTime.Minute / 10 * 10 + 10}.txt";                      // テキストファイルネーム
            (string dayFolderPath, string hourFolderPath) = SetPath();
            CheckFileExists(dayFolderPath, hourFolderPath);
            string filePath = Path.Combine(hourFolderPath, fileName);

            lock (_lockObj)
            {
                using (FileStream file = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
                    {
                        string dumpMsg = string.Concat($"[{GetDayStr()}] ", $"[{level}] ", msg);
                        writer.WriteLineAsync(dumpMsg);
                        DEBUG($"Written to file : {fileName}...");
                    }
                }
            }

            return true; // 表示文件写入成功
        }

        private (string, string) SetPath()
        {
            string today = _dateTime.ToString("yyyy-MM-dd");                                            // 日付
            string hour = _dateTime.ToString("HH");                                                     // 時間

            string dayFolderPath = Path.Combine(_logFolderPath, today);                                 // 日付フォルダー
            string hourFolderPath = Path.Combine(dayFolderPath, hour + "H");                            // 時間区分フォルダー

            return (dayFolderPath, hourFolderPath);
        }

        private void CheckFileExists(string dayFolderPath, string hourFolderPath)
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
