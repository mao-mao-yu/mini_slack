using System;
using System.Collections.Generic;
using System.Text;

namespace Server.IServer
{
    interface IAppServer
    {
        public void Send();
        public void Listening();
        public void FileHandler();
    }
}
