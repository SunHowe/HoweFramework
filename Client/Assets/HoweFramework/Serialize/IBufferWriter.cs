using System;

namespace HoweFramework
{
    /// <summary>
    /// 缓冲区写入器。
    /// </summary>
    public interface IBufferWriter : IDisposable
    {
        /// <summary>
        /// 缓冲区大小。
        /// </summary>
        int BufferSize { get; }

        /// <summary>
        /// 当前位置。
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// 获取已写入的缓冲区。
        /// </summary>
        Span<byte> WrittenBuffer { get; }

        /// <summary>
        /// 写入一个字节。
        /// </summary>
        /// <param name="value">字节。</param>
        void WriteByte(byte value);

        /// <summary>
        /// 写入一个字节数组。
        /// </summary>
        /// <param name="value">字节数组。</param>
        void WriteBytes(ReadOnlySpan<byte> value);

        /// <summary>
        /// 写入一个布尔值。
        /// </summary>
        /// <param name="value">布尔值。</param>
        void WriteBool(bool value);

        /// <summary>
        /// 写入一个字符。
        /// </summary>
        /// <param name="value">字符。</param>
        void WriteChar(char value);

        /// <summary>
        /// 写入一个16位整数。
        /// </summary>
        /// <param name="value">16位整数。</param>
        void WriteInt16(short value);

        /// <summary>
        /// 写入一个32位整数。
        /// </summary>
        /// <param name="value">32位整数。</param>
        void WriteInt32(int value);

        /// <summary>
        /// 写入一个64位整数。
        /// </summary>
        /// <param name="value">64位整数。</param>
        void WriteInt64(long value);

        /// <summary>
        /// 写入一个单精度浮点数。
        /// </summary>
        /// <param name="value">单精度浮点数。</param>
        void WriteFloat(float value);

        /// <summary>
        /// 写入一个双精度浮点数。
        /// </summary>
        /// <param name="value">双精度浮点数。</param>
        void WriteDouble(double value);

        /// <summary>
        /// 写入一个字符串。
        /// </summary>
        /// <param name="value">字符串。</param>
        void WriteString(string value);

        /// <summary>
        /// 写入可序列化对象。
        /// </summary>
        /// <param name="value">可序列化对象。</param>
        void WriteObject<T>(T value) where T : ISerializable;
    }
}