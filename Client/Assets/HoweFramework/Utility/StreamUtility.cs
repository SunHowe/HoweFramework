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
            return BitConverter.ToInt32(s_CachedBytes, 0);
        }

        /// <summary>
        /// 向流中写入一个32位整数。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <param name="value">要写入的32位整数。</param>
        public static void WriteInt32(this Stream stream, int value)
        {
            ConverterUtility.GetBytes(value, s_CachedBytes, 0);
            stream.Write(s_CachedBytes, 0, 4);
        }

        public static void WriteInt64(this Stream stream, long value)
        {
            ConverterUtility.GetBytes(value, s_CachedBytes, 0);
            stream.Write(s_CachedBytes, 0, 8);
        }
    }
}