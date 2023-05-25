using Newtonsoft.Json;
using System.Collections.Generic;
using Server.Common;
using System.IO;

namespace Server.Common
{
    public abstract class SettingBase
    {
        public static T LoadSetting<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            T setting = JsonConvert.DeserializeObject<T>(json);
            return setting;
        }

        public void SaveConfiguration(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
