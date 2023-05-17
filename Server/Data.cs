using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Server.Data
{
    class Response
    {
        private Dictionary<string, string> JsonDictResponse { get; set; }

        public Response(byte[] metaData)
        {
            string jsonStr = Encoding.UTF8.GetString(metaData);
            JsonDictResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
        }

        public Response(string jsonStr)
        {
            JsonDictResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
        }

        public Dictionary<string, string> Get()
        {
            return JsonDictResponse;
        }
    }

    class Request
    {
        private readonly Dictionary<string, string> RequestBaseData = new Dictionary<string, string>();

        /// <summary>
        /// request paras
        /// </summary>
        /// <param name="action">動作</param>
        public Request(string action)
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
    }
}
