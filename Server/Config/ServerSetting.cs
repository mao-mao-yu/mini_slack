using Server.Common;

namespace Server.Config
{
    public class ServerSetting : SettingBase
    {
        #region Fields

        #endregion

        #region Properties
        /// <summary>
        /// Server listening ip 
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// Server listening port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Max num of client
        /// </summary>
        public int MaxClientNum { get; set; }

        /// <summary>
        /// Ring buffer size
        /// </summary>
        public int RingBufferSize { get; set; }

        /// <summary>
        /// BufferManager size
        /// </summary>
        public int BufferManagerSize { get; set; }
        #endregion

    }
}
