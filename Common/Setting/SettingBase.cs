namespace Common.Setting
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
            return Json.LoadFromFile<TObj>(filePath);
        }

        /// <summary>
        /// Save configuration file
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveSetting(string filePath)
        {
            string directoryPath = System.IO.Path.GetDirectoryName(filePath);
            if (!System.IO.File.Exists(filePath))
            {

                System.IO.Directory.CreateDirectory(directoryPath);
            }
            Json.DumpToFile(this, filePath);
        }
    }
}
