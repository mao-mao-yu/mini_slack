using Newtonsoft.Json;
using System.Collections.Generic;
using Server.Common;

namespace Server.ServerCore
{
    public class Response
    {
        private readonly MyDictionary<string, string> RequestBaseData = new MyDictionary<string, string>();

        /// <summary>
        /// request params
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
        public Response Add(string key, string value)
        {
            RequestBaseData.Add(key, value);
            return this;
        }

        public Dictionary<string, string> Get()
        {
            return RequestBaseData;
        }

        public new string ToString()
        {
            return JsonConvert.SerializeObject(RequestBaseData);
        }
    }
}
