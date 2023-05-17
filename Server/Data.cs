using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Server.Data
{
    public class Response
    {
        private readonly Dictionary<string, string> RequestBaseData = new Dictionary<string, string>();

        /// <summary>
        /// request paras
        /// </summary>
        /// <param name="action">動作</param>
        public Response(string action)
        {
            RequestBaseData.Add("action", action);
        }

        /// <summary>
        /// 外部アクセス方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            RequestBaseData.Add(key, value);
        }

        public Dictionary<string, string> Get()
        {
            return RequestBaseData;
        }

        public string GetJsonStr()
        {
            return JsonConvert.SerializeObject(RequestBaseData);
        }
    }

    public class Request
    {
        private Dictionary<string, string> JsonDictResponse { get; set; }

        public Request(byte[] metaData)
        {
            string jsonStr = Encoding.UTF8.GetString(metaData);
            JsonDictResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
        }

        public Request(string jsonStr)
        {
            JsonDictResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
        }

        public Dictionary<string, string> Get()
        {
            return JsonDictResponse;
        }
        
    }
}
