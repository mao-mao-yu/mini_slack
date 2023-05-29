using Common;

namespace Server.Models
{
    public class Response
    {
        private readonly Dictionary<string, string> _responseDict;

        /// <summary>
        /// request params
        /// </summary>
        /// <param name="action">動作</param>
        public Response(string action, bool result)
        {
            _responseDict = new Dictionary<string, string>
            {
                { "action", action },
                { "result", result.ToString()}
            };
        }

        /// <summary>
        /// 外部アクセス方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Response Add(string key, string value)
        {
            _responseDict.Add(key, value);
            return this;
        }

        public string Get(string key)
        {
            return _responseDict.Get(key);
        }

        public new string ToString()
        {
            return Json.Dump(_responseDict);
        }
    }
}
