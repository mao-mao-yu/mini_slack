using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Server.Common;

namespace Server.ServerCore
{
    public class Request
    {
        private MyDictionary<string, string> JsonDict { get; set; }

        public Request(byte[] metaData)
        {
            string jsonStr = Encoding.UTF8.GetString(metaData);
            JsonDict = JsonConvert.DeserializeObject<MyDictionary<string, string>>(jsonStr);
        }

        public Request(string jsonStr)
        {
            JsonDict = JsonConvert.DeserializeObject<MyDictionary<string, string>>(jsonStr);
        }

        public Dictionary<string, string> GetDict()
        {
            return JsonDict;
        }

        /// <summary>
        /// Use key to try get a value.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Return value if key in dict,else return null</returns>
        public string Get(string key)
        {
            return JsonDict.Get(key);
        }
    }
}
