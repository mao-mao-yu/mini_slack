using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.ServerCore
{
    public class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> m_pool;

        /// <summary>  
        /// 初始化对象池  
        /// </summary>  
        /// <param name="capacity">最大连接数量</param>  
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        //回收对对象  
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        //分配对象  
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        //对象个数  
        public int Count
        {
            get { return m_pool.Count; }
        }

    }
}
