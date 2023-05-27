//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using Server.Setting;
//using Server.Log;

//namespace Server
//{
//    class Program
//    {
//        private static void Main(string[] args)
//        {
//            ServerSetting serverSetting = new ServerSetting();
//            //LoggerSetting setting = new LoggerSetting();

//            string currentPath = Directory.GetCurrentDirectory();
//            string ServerSettingPath = Path.Combine(currentPath, @"Config", @"ServerSetting.json");
//            string LoggerSettingPath = Path.Combine(currentPath, @"Config", @"LoggerSetting.json");
//            //setting.ConsoleWriteLevel = LogLevel.DEBUG;
//            //setting.FileWriteLevel = LogLevel.INFO;
//            //setting.IsWriteToFile = true;


//            //setting.SaveSetting(LoggerSettingPath);
//            Encoding encoding = Encoding.GetEncoding("UTF-8");
//            Console.WriteLine(encoding);
//            serverSetting.ServerIP = "0.0.0.0";
//            serverSetting.Port = 8888;
//            serverSetting.MaxClientNum = 500;
//            serverSetting.RingBufferSize = 2048;
//            serverSetting.BufferManagerSize = 1024;
//            serverSetting.DefaultEncoding = "UTF-8";
//            serverSetting.SaveSetting(ServerSettingPath);

//            //ServerSetting serverSetting = SettingBase.LoadSetting<ServerSetting>(ServerSettingPath);
//            //LoggerSetting loggerSetting = SettingBase.LoadSetting<LoggerSetting>(LoggerSettingPath);
//            //var properties = loggerSetting.GetType().GetProperties();
//            //foreach (var item in properties)
//            //{
//            //    string propertyName = item.Name;
//            //    object propertyValue = item.GetValue(loggerSetting);
//            //    Console.WriteLine($"{propertyName} = {propertyValue}");
//            //}

//        }
//    }

//}
