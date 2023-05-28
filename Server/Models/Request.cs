using Common;

namespace Server.Models
{
    public class Request
    {
        private readonly Dictionary<string, string> _requestDict;

        public Request(byte[] metaData)
        {
            string jsonStr = Text.GetString(metaData);
            _requestDict = Json.Load<Dictionary<string, string>>(jsonStr);
        }

        public Request(string jsonStr)
        {
            _requestDict = Json.Load<Dictionary<string, string>>(jsonStr);
        }

        /// <summary>
        /// Use key to try get a value.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Return value if key in dict,else return null</returns>
        public string Get(string key)
        {
            return _requestDict.Get(key);
        }
    }
}
