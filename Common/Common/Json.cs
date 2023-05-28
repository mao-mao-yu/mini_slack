using System;
using System.IO;
using Newtonsoft.Json;
using Common.Log;

namespace Common
{
    public static class Json
    {
        public static string Dump<T>(T obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }

        public static TResult Load<TResult>(string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<TResult>(jsonStr);
            }
            catch (Exception e)
            {
                Logger.FWARNING(e, $"Can't convert json str to obj...{jsonStr}");
            }
            return default;
        }

        public static void DumpToFile<TObj>(TObj obj, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, Dump(obj, Formatting.Indented));
            }
            catch (Exception e)
            {
                Logger.FWARNING(e, $"Writing {filePath} error...");
                Logger.WARNING($"Writing {filePath} error...");
            }
        }

        public static TResult LoadFromFile<TResult>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} not found...");
            }
            return Load<TResult>(File.ReadAllText(filePath));
        }
    }

}
