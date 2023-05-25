using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server.Config;
using Server.Log;

namespace Server
{
    class Program
    {
        private static void Main(string[] args)
        {
            //ServerSetting serverSetting = new ServerSetting();
            LoggerSetting setting = new LoggerSetting();

            string currentPath = Directory.GetCurrentDirectory();
            string ServerSettingPath = Path.Combine(currentPath, @"Config", @"ServerSetting.json");
            string LoggerSettingPath = Path.Combine(currentPath, @"Config", @"LoggerSetting.json");
            setting.ConsoleWriteLevel = LogLevel.DEBUG;
            setting.FileWriteLevel = LogLevel.INFO;
            setting.IsWriteToFile = true;
            setting.SaveConfiguration(LoggerSettingPath);
            //serverSetting.ServerIP = "127.0.0.1";
            //serverSetting.Port = 8888;
            //serverSetting.MaxClientNum = 500;
            //serverSetting.RingBufferSize = 4096;
            //serverSetting.BufferManagerSize = 2048;

            //serverSetting.SaveConfiguration(ServerSettingPath);
            //ServerSetting serverSetting = Common.SettingBase.LoadSetting<ServerSetting>(ServerSettingPath);
            //var properties = serverSetting.GetType().GetProperties();
            //foreach (var item in properties)
            //{
            //    string propertyName = item.Name;
            //    object propertyValue = item.GetValue(serverSetting);
            //    Console.WriteLine($"{propertyName} = {propertyValue}");
            //}
        }
    }

}
