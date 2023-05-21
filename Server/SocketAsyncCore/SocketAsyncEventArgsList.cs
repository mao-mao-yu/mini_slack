using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.SocketAsyncCore
{
    public class SocketAsyncEventArgsList : Object
    {
        private List<SocketAsyncEventArgs> m_list;

        public SocketAsyncEventArgsList()
        {
            m_list = new List<SocketAsyncEventArgs>();
        }

        public void Add(SocketAsyncEventArgs s)
        {
            lock (m_list)
            {
                m_list.Add(s);
            }
        }

        public void Remove(SocketAsyncEventArgs s)
        {
            lock (m_list)
            {
                m_list.Remove(s);
            }
        }

        public void CopyList(ref SocketAsyncEventArgs[] array)
        {
            lock (m_list)
            {
                array = new SocketAsyncEventArgs[m_list.Count];
                m_list.CopyTo(array);
            }
        }
        public void CloseAll()
        {
            lock (m_list)
            {
                foreach(SocketAsyncEventArgs socketAsyncEventArgs in m_list)
                {
                    AsyncUserToken token = (AsyncUserToken)socketAsyncEventArgs.UserToken;
                    if (token.Socket != null)
                    {
                        token.Socket.Close();
                    }
                }
            }
        }
    }
}
