using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.ServerCore
{
    /// <summary>
    /// SocketAsyncEventArgsList class
    /// </summary>
    public class SocketAsyncEventArgsList : object
    {
        private readonly List<SocketAsyncEventArgs> m_list;

        /// <summary>
        /// ctor
        /// Create a SocketAsyncEventArgs list
        /// </summary>
        public SocketAsyncEventArgsList()
        {
            m_list = new List<SocketAsyncEventArgs>();
        }

        /// <summary>
        /// Add a SocketAsyncEventArgs to list
        /// </summary>
        /// <param name="s"></param>
        public void Add(SocketAsyncEventArgs s)
        {
            lock (m_list)
            {
                m_list.Add(s);
            }
        }


        /// <summary>
        /// Remove a SocketAsyncEventArgs from list
        /// </summary>
        /// <param name="s"></param>
        public void Remove(SocketAsyncEventArgs s)
        {
            lock (m_list)
            {
                m_list.Remove(s);
            }
        }

        /// <summary>
        /// CopyList
        /// </summary>
        /// <param name="array"></param>
        public void CopyList(ref SocketAsyncEventArgs[] array)
        {
            lock (m_list)
            {
                array = new SocketAsyncEventArgs[m_list.Count];
                m_list.CopyTo(array);
            }
        }

        /// <summary>
        /// Close all 
        /// </summary>
        public void CloseAll()
        {
            lock (m_list)
            {
                foreach (SocketAsyncEventArgs socketAsyncEventArgs in m_list)
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
