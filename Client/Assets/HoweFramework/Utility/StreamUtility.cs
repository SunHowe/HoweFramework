using System;
using System.IO;

namespace HoweFramework
{
    /// <summary>
    /// 对 Stream 的扩展方法。
    /// </summary>
    public static class StreamUtility
    {
        private static readonly byte[] s_CachedBytes = new byte[8];

        /// <summary>
        /// 从流中读取一个32位整数。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>读取的32位整数。</returns>
        public static int ReadInt32(this Stream stream)
        {
            stream.Read(s_CachedBytes, 0, 4);
            return (s_CachedBytes[0] << 24) | (s_CachedBytes[1] << 16) | (s_CachedBytes[2] << 8) | s_CachedBytes[3];
        }
        
        /// <summary>
        /// 从流中读取一个64位整数。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>读取的64位整数。</returns>
        public static long ReadInt64(this Stream stream)
        {
            stream.Read(s_CachedBytes, 0, 8);
            return (s_CachedBytes[0] << 56) | (s_CachedBytes[1] << 48) | (s_CachedBytes[2] << 40) | (s_CachedBytes[3] << 32) | (s_CachedBytes[4] << 24) | (s_CachedBytes[5] << 16) | (s_CachedBytes[6] << 8) | s_CachedBytes[7];
        }

        /// <summary>
        /// 向流中写入一个32位整数。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <param name="value">要写入的32位整数。</param>
        public static void WriteInt32(this Stream stream, int value)
        {
            s_CachedBytes[0] = (byte)(value >> 24);
            s_CachedBytes[1] = (byte)(value >> 16);
            s_CachedBytes[2] = (byte)(value >> 8);
            s_CachedBytes[3] = (byte)value;
            stream.Write(s_CachedBytes, 0, 4);
        }

        /// <summary>
        /// 向流中写入一个64位整数。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <param name="value">要写入的64位整数。</param>
        public static void WriteInt64(this Stream stream, long value)
        {
            s_CachedBytes[0] = (byte)(value >> 56);
            s_CachedBytes[1] = (byte)(value >> 48);
            s_CachedBytes[2] = (byte)(value >> 40);
            s_CachedBytes[3] = (byte)(value >> 32);
            s_CachedBytes[4] = (byte)(value >> 24);
            s_CachedBytes[5] = (byte)(value >> 16);
            s_CachedBytes[6] = (byte)(value >> 8);
            s_CachedBytes[7] = (byte)value;
            stream.Write(s_CachedBytes, 0, 8);
        }
    }
}