using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Client.Converter
{
    public static class JsonConverter
    {
        public static string GetJsonStr<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T GetJsonObj<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}
