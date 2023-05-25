using System;
using System.Collections.Generic;
using System.Text;
using Server.Error;
using Server.Log;

namespace Server.SocketAsyncCore
{
    public class SocketAsyncRingBuffer
    {

        public int ReadIndex { get; private set; }
        public int WriteIndex { get; private set; }
        public int Size { get; private set; }

        private Logger lg = new Logger();

        /// <summary>
        /// RingBuffer
        /// </summary>
        private readonly byte[] buffer;

        public bool IsEmpty => ReadIndex == WriteIndex;

        public bool IsFull => (WriteIndex + 1) % Size == ReadIndex;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="size">Buffer size</param>
        public SocketAsyncRingBuffer(int size)
        {
            buffer = new byte[size];
            Size = size;
            ReadIndex = 0;
            WriteIndex = 0;
        }

        public byte this[int index]
        {
            get => buffer[index];
            set => buffer[index] = value;
        }

        public byte[] Read(int length)
        {
            if (length > Size)
            {
                throw new ArgumentException("Length is larger than buffer size");
            }

            byte[] result = new byte[length];
            lg.DEBUG($"Read method start ReadIndex is {ReadIndex}");
            int space = Size - ReadIndex;
            if (length <= space)
            {
                Buffer.BlockCopy(buffer, ReadIndex, result, 0, length);
                ReadIndex += length;
            }
            else
            {
                Buffer.BlockCopy(buffer, ReadIndex, result, 0, space);
                Buffer.BlockCopy(buffer, 0, result, space, length - space);
                ReadIndex = length - space;
            }
            lg.DEBUG($"Read method end ReadIndex is {ReadIndex}");
            return result;
        }

        public void Write(byte[] data)
        {
            if (data.Length > Size)
            {
                throw new ArgumentException("Length is larger than buffer size");
            }
            lg.DEBUG($"Write method start WriteIndex is {WriteIndex}");
            int space = Size - WriteIndex;
            if (data.Length <= space)
            {
                Array.Copy(data, 0, buffer, WriteIndex, data.Length);
                WriteIndex += data.Length;
            }
            else
            {
                Array.Copy(data, 0, buffer, WriteIndex, space);
                Array.Copy(data, space, buffer, 0, data.Length - space);
                WriteIndex = data.Length - space;
            }
            lg.DEBUG($"Write method end WriteIndex is {WriteIndex}");
        }

        public int ReadHeader()
        {
            byte[] dataLengthBytes = new byte[4];
            for (int i = 0; i < dataLengthBytes.Length; i++)
            {
                dataLengthBytes[i] = buffer[ReadIndex + i];
            }
            int dataLength = BitConverter.ToInt32(dataLengthBytes);
            return dataLength;
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            WriteIndex = ReadIndex = 0;
        }

        /// <summary>
        /// Having data of datalength?
        /// </summary>
        /// <param name="dataLength"></param>
        /// <returns></returns>
        public bool HavingData(int dataLength)
        {
            int availableDataLength = WriteIndex - ReadIndex;

            if (availableDataLength < 0)
            {
                availableDataLength += Size;
            }

            return dataLength <= availableDataLength;
        }

        /// <summary>
        /// Can write
        /// </summary>
        /// <returns></returns>
        public bool HavingSpace(int dataLength)
        {
            int freeSpace = ReadIndex - WriteIndex;
            if (freeSpace < 0)
            {
                freeSpace += Size;
            }

            return dataLength <= freeSpace;
        }
    }
}
