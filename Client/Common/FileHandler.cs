using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Client.Error;

namespace Client.FileHandler
{
    public static class JsonFileHandler
    {
        public static Dictionary<string, object> LoadJsonObjFromFile(string filePath)
        {
            // 拡張子判断 .jsonかどうか
            if (!Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                throw new FileLoadException($"{filePath}はJsonファイルではない...");
            }

            // ファイル存在するかどうか判断
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath}は存在しない...");
            }

            // utf8でファイルを開く
            string jsonData = File.ReadAllText(filePath, Encoding.UTF8);

            // dictに変換
            Dictionary<string, object> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>> (jsonData);

            return jsonDict;
        }

        /// <summary>
        /// jsonDataをファイルに書き込む
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonData"></param>
        /// <param name="filePath"></param>
        public static void DumpJsonToFile<T>(T jsonData, string filePath)
        {
            // 拡張子判断 .jsonかどうか
            if (!Path.GetExtension(filePath).Equals(".json", StringComparison.Ordinal))
            {
                throw new FileLoadException($"{filePath}はJsonファイルではない...");
            };

            // stringに変換
            string jsonStr = JsonConvert.SerializeObject(jsonData);

            // utf8でファイルに書き込む
            File.WriteAllText(filePath, jsonStr, Encoding.UTF8);
        }
    }
}
