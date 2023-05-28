namespace Server.Setting
{
    public class ServerSetting : Common.Setting.SettingBase
    {
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

        /// <summary>
        /// Default encoding
        /// </summary>
        public string  DefaultEncoding { get; set; }

        /// <summary>
        /// Aes key size
        /// </summary>
        public int AesKeySize { get; set; }
        #endregion
    }
}
