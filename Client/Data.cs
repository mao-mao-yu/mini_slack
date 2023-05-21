using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Client.Encryption;

namespace Client.DataType
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
        private readonly Dictionary<string, string> RequestBaseDict = new Dictionary<string, string>();

        /// <summary>
        /// request paras
        /// </summary>
        /// <param name="action">動作</param>
        /// <param name="username">ユーザネーム</param>
        /// <param name="password">パスワード</param>
        public Request(string action, string username, string password)
        {
            RequestBaseDict.Add("action", action);
            RequestBaseDict.Add("username", username);
            
            RequestBaseDict.Add("password", PasswordEncryption.Encrypt(password));
        }

        public Request(string action)
        {
            RequestBaseDict.Add("action", action);
        }

        /// <summary>
        /// 外部アクセス方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Request Add(string key, string value)
        {
            RequestBaseDict.Add(key, value);
            return this;
        }

        public Dictionary<string, string> Get()
        {
            return RequestBaseDict;
        }

        public new string ToString()
        {
            return JsonConvert.SerializeObject(RequestBaseDict);
        }
    }
}
