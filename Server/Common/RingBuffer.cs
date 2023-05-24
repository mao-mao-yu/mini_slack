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

        public RingBuffer(int size = 4096)
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

        public byte Read()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Buffer is empty");
            }

            byte result = buffer[head];
            head = (head + 1) % size;

            return result;
        }

        public void Write(byte[] data)
        {
            foreach(byte bytedata in data)
            {
                Write(bytedata);
            }
        }

        public byte[] Read(int length)
        {
            if (length > size)
            {
                throw new ArgumentException("Length is larger than buffer size");
            }

            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = Read();
            }

            return result;
        }


    }
}
