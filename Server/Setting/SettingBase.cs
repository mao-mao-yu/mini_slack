using System.IO;
using Server.Common;

namespace Server.Setting
{
    public class SettingBase
    {
        /// <summary>
        /// Load setting file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TObj LoadSetting<TObj>(string filePath)
        {
            return Json.Loads<TObj>(filePath);
        }

        /// <summary>
        /// Save configuration file
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveSetting(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!File.Exists(filePath))
            {
                
                Directory.CreateDirectory(directoryPath);
            }
            Json.Dumps(this, filePath);
        }
    }
}
