using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Server
{
    /// <summary>
    /// Logger
    /// </summary>
    public class Logger
    {
        #region Fields
        /// <summary>
        /// 文件锁
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// 本地工作目录
        /// </summary>
        private static string _currentPath;

        /// <summary>
        /// log文件夹输出目录
        /// </summary>
        private readonly string _logFolderPath;

        /// <summary>
        /// 当前日期时间
        /// </summary>
        private DateTime _dateTime;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private readonly string _configPath;

        /// <summary>
        /// 配置文件字典
        /// </summary>
        private readonly Dictionary<string, int> _configDict;

        /// <summary>
        /// 配置文件控制台输出等级
        /// </summary>
        private readonly int _configLogLevel;

        /// <summary>
        /// 配置文件输出text等级
        /// </summary>
        private readonly int _configOperateLevel;

        /// <summary>
        /// 是否写入文件
        /// </summary>
        private readonly bool _configAllWrite;
        #endregion

        #region Property
        /// <summary>
        /// AllWrite属性 是否写入文件
        /// </summary>
        public bool AllWrite => _configAllWrite;
        #endregion

        #region Ctor
        public Logger()
        {
            // 获取当前工作路径
            _currentPath = Directory.GetCurrentDirectory();
            // Log输出路径
            _logFolderPath = Path.Combine(_currentPath, "Log");
            // 配置文件路径
            _configPath = Path.Combine(_currentPath, "LogConfig.json");
            _configDict = GetConfigDictionary();
            _configLogLevel = GetConfigLogLevel();
            _configOperateLevel = GetConfigOperateLevel();
            _configAllWrite = GetConfigAllWrite();

        }
        #endregion

        #region Get config
        /// <summary>
        /// 获取配置文件并转为字典
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> GetConfigDictionary()
        {
            string configJson = File.ReadAllText(_configPath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(configJson);
        }

        /// <summary>
        /// 获取日志输出等级
        /// </summary>
        /// <returns></returns>
        private int GetConfigLogLevel()
        {
            if (_configDict.ContainsKey("LogLevel"))
            {
                return _configDict["LogLevel"];
            }
            return 1;
        }

        /// <summary>
        /// 获取日志输出文件等级
        /// </summary>
        /// <returns></returns>
        private int GetConfigOperateLevel()
        {
            if (_configDict.ContainsKey("OperateLevel"))
            {
                return _configDict["OperateLevel"];
            }
            return 1;
        }

        /// <summary>
        /// 获取是否写入文件bool
        /// </summary>
        /// <returns></returns>
        private bool GetConfigAllWrite()
        {
            if (_configDict.ContainsKey("UseFileWriter"))
            {
                if (_configDict["UseFileWriter"] == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
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
            if ((int)logLevel < _configLogLevel)
            {
                return;
            }
            Console.WriteLine($"[{GetDayStr()}] [{logLevel}] : {message}");
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
            if ((int)logLevel < _configLogLevel)
            {
                return;
            }
            if (_configAllWrite)
            {
                FileWriter(message, logLevel);
            }
        }
        #endregion

        #region Exception To File
        public void INSIGNIFICANT(Exception e, string msg)
        {
            WriteError(e, msg, OperateLevel.INSIGNIFICANT);
        }

        public void SMALLEFFECT(Exception e, string msg)
        {
            WriteError(e, msg, OperateLevel.SMALLEFFECT);
        }

        public void NORMAL(Exception e, string msg)
        {
            WriteError(e, msg, OperateLevel.NORMAL);
        }

        public void IMPORTTANT(Exception e, string msg)
        {
            WriteError(e, msg, OperateLevel.IMPORTTANT);
        }

        public void WriteError(Exception ex, string msg, OperateLevel operateLevel)
        {
            if (_configAllWrite)
            {
                FileWriter($"{msg} message : {ex.Message}. StackTrace : {ex.StackTrace}", operateLevel);
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
            string fileName = $"{_dateTime:yyyy-MM-dd HH}-{(_dateTime.Minute / 10 * 10)}.txt";                      // テキストファイルネーム
            (string dayFolderPath, string hourFolderPath) = SetPath();
            CheckFileExists(dayFolderPath, hourFolderPath);
            string filePath = Path.Combine(hourFolderPath, fileName);

            using (FileStream file = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                file.Lock(0,file.Length);
                using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"[{GetDayStr()}] ");
                    sb.Append($"[{level}] ");
                    sb.Append(msg);
                    writer.WriteLineAsync(sb.ToString());
                    DEBUG($"Written to file : {fileName}...");
                }
                file.Unlock(0, file.Length);
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
