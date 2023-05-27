using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Setting
{
    public class ClientSetting:SettingBase
    {
        #region Properties
        /// <summary>
        /// Server listening ip 
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// Server port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// buffer size
        /// </summary>
        public int BufferSize { get; set; }
        #endregion
    }
}
