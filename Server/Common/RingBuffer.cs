using System;
using System.Collections.Generic;
using System.Text;
using Server.Error;

namespace Server.Common
{
    public class RingBuffer
    {

        public int ReadIndex { get; private set; }
        public int WriteIndex { get; private set; }
        public int Size { get; private set; }

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
        public RingBuffer(int size = 4096)
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

            return result;
        }

        public void Write(byte[] data)
        {
            if (data.Length > Size)
            {
                throw new ArgumentException("Length is larger than buffer size");
            }

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
        }

        public int ReadHeader()
        {
            byte[] dataLengthBytes = Read(4);
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
            int freeSpace = ReadIndex - WriteIndex - 1;
            if (freeSpace < 0)
            {
                freeSpace += Size;
            }

            return dataLength <= freeSpace;
        }
    }
}
