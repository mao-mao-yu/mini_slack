using System;
using System.IO;
using Newtonsoft.Json;
using Server.Log;

namespace Server.Common
{
    public static class Json
    {
        public static string Dump<T>(T obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }

        public static TResult Load<TResult>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<TResult>(jsonStr);
        }

        public static void Dumps<TObj>(TObj obj, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, Dump(obj, Formatting.Indented));
            }
            catch (Exception e)
            {
                Logger lg = new Logger();
                lg.FWARNING(e, $"Writing {filePath} error...");
                lg.WARNING($"Writing {filePath} error...");
                throw;
            }
        }

        public static TResult Loads<TResult>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} not found...");
            }

            try
            { 
                return Load<TResult>(File.ReadAllText(filePath));
            }
            catch (Exception e)
            {
                Logger lg = new Logger();
                lg.FWARNING(e, $"Writing {filePath} error...");
                lg.WARNING($"Writing {filePath} error...");
                throw;
            }
        }
    }
}
