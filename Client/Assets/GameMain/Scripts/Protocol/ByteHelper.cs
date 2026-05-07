using System;
using System.IO;

namespace Protocol
{
    public static class ByteHelper
    {
        /// <summary>
        /// 获取16位无符号整数。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>16位无符号整数。</returns>
        public static ushort ToUInt16(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 2)
            {
                throw new Exception("Buffer is too short to get 16-bit unsigned integer.");
            }

            var byte1 = buffer[0];
            var byte2 = buffer[1];
            return (ushort)((byte1 << 8) | byte2);
        }

        /// <summary>
        /// 获取32位整数。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>32位整数。</returns>
        public static int ToInt32(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 4)
            {
                throw new Exception("Buffer is too short to get 32-bit integer.");
            }

            var byte1 = buffer[0];
            var byte2 = buffer[1];
            var byte3 = buffer[2];
            var byte4 = buffer[3];
            return (int)((byte1 << 24) | (byte2 << 16) | (byte3 << 8) | byte4);
        }

        /// <summary>
        /// 获取64位整数。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>64位整数。</returns>
        public static long ToInt64(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 8)
            {
                throw new Exception("Buffer is too short to get 64-bit integer.");
            }

            var byte1 = buffer[0];
            var byte2 = buffer[1];
            var byte3 = buffer[2];
            var byte4 = buffer[3];
            var byte5 = buffer[4];
            var byte6 = buffer[5];
            var byte7 = buffer[6];
            var byte8 = buffer[7];
            return (long)((byte1 << 56) | (byte2 << 48) | (byte3 << 40) | (byte4 << 32) | (byte5 << 24) | (byte6 << 16) | (byte7 << 8) | byte8);
        }

        /// <summary>
        /// 序列化16位无符号整数。
        /// </summary>
        /// <param name="value">16位无符号整数。</param>
        /// <param name="destination">目标流。</param>
        public static void Serialize(ushort value, Stream destination)
        {
            var byte1 = (byte)(value >> 8);
            var byte2 = (byte)(value & 0xFF);
            destination.WriteByte(byte1);
            destination.WriteByte(byte2);
        }

        /// <summary>
        /// 序列化32位整数。
        /// </summary>
        /// <param name="value">32位整数。</param>
        /// <param name="destination">目标流。</param>
        public static void Serialize(int value, Stream destination)
        {
            var byte1 = (byte)(value >> 24);
            var byte2 = (byte)(value >> 16);
            var byte3 = (byte)(value >> 8);
            var byte4 = (byte)(value & 0xFF);
            destination.WriteByte(byte1);
            destination.WriteByte(byte2);
            destination.WriteByte(byte3);
            destination.WriteByte(byte4);
        }

        /// <summary>
        /// 序列化64位整数。
        /// </summary>
        /// <param name="value">64位整数。</param>
        /// <param name="destination">目标流。</param>
        public static void Serialize(long value, Stream destination)
        {
            var byte1 = (byte)(value >> 56);
            var byte2 = (byte)(value >> 48);
            var byte3 = (byte)(value >> 40);
            var byte4 = (byte)(value >> 32);
            var byte5 = (byte)(value >> 24);
            var byte6 = (byte)(value >> 16);
            var byte7 = (byte)(value >> 8);
            var byte8 = (byte)(value & 0xFF);
            destination.WriteByte(byte1);
            destination.WriteByte(byte2);
            destination.WriteByte(byte3);
            destination.WriteByte(byte4);
            destination.WriteByte(byte5);
            destination.WriteByte(byte6);
            destination.WriteByte(byte7);
            destination.WriteByte(byte8);
        }

        /// <summary>
        /// 序列化16位无符号整数。
        /// </summary>
        /// <param name="value">16位无符号整数。</param>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="offset">偏移量。</param>
        public static void Serialize(ushort value, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(value >> 8);
            buffer[offset + 1] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// 序列化32位整数。
        /// </summary>
        /// <param name="value">32位整数。</param>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="offset">偏移量。</param>
        public static void Serialize(int value, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// 序列化64位整数。
        /// </summary>
        /// <param name="value">64位整数。</param>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="offset">偏移量。</param>
        public static void Serialize(long value, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(value >> 56);
            buffer[offset + 1] = (byte)(value >> 48);
            buffer[offset + 2] = (byte)(value >> 40);
            buffer[offset + 3] = (byte)(value >> 32);
            buffer[offset + 4] = (byte)(value >> 24);
            buffer[offset + 5] = (byte)(value >> 16);
            buffer[offset + 6] = (byte)(value >> 8);
            buffer[offset + 7] = (byte)(value & 0xFF);
        }
    }
}