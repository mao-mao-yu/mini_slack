using System.IO;
using System.Net;
using Server.Converter;
using System.Collections.Generic;
using Server.Common;

namespace Server.Config
{
    public class ServerSetting
    {

        #region Properties
        /// <summary>
        /// Server listening ip 
        /// </summary>
        public IPAddress ServerIP { get; private set; }

        /// <summary>
        /// Server listening port
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Server IPEndpoint
        /// </summary>
        public IPEndPoint ServiceIPEndPoint { get; private set; }

        /// <summary>
        /// Max num of client
        /// </summary>
        public int MaxClientNum { get; private set; }

        /// <summary>
        /// Ring buffer size
        /// </summary>
        public int RingBufferSize { get; private set; }

        /// <summary>
        /// BufferManager size
        /// </summary>
        public int BufferManagerSize { get; private set; }
        #endregion

        public ServerSetting(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Server setting file not found...");
            }

            string allText = File.ReadAllText(filePath);
            MyDictionary<string, object> configDict = JsonConverter.GetJsonObj<MyDictionary<string, object>>(allText);
            ServerIP = IPAddress.Parse((string)configDict.Get("ip"));
        }

    }
}
