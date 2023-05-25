using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Server.Converter
{
    public static class JsonConverter
    {
        public static string GetJsonStr<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static TResult GetJsonObj<TResult>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<TResult>(jsonStr);
        }
    }
}
