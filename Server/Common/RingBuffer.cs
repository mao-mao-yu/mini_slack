using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Common
{
    public class RingBuffer
    {
        private byte[] buffer;
        private int head;
        private int tail;
        private int size;

        public RingBuffer(int size)
        {
            buffer = new byte[size];
            this.size = size;
            head = 0;
            tail = 0;
        }

        public bool IsEmpty
        {
            get { return head == tail; }
        }

        public bool IsFull
        {
            get { return (tail + 1) % size == head; }
        }

        public void Write(byte data)
        {
            buffer[tail] = data;
            tail = (tail + 1) % size;
        }
    }
}
