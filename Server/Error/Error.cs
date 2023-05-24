using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Error
{
    class BytesDataHeaderError:Exception
    {
        public BytesDataHeaderError(string msg):base(msg)
        {
        }
    }
}
