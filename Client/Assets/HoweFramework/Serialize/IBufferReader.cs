using System;

namespace HoweFramework
{
    /// <summary>
    /// 缓冲区读取器。
    /// </summary>
    public interface IBufferReader : IDisposable
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
        /// 读取一个字节。
        /// </summary>
        byte ReadByte();
        
        /// <summary>
        /// 读取一个字节数组。
        /// </summary>
        /// <param name="size">字节数组长度。</param>
        /// <returns>字节数组。</returns>
        ReadOnlySpan<byte> ReadBytes(int size);

        /// <summary>
        /// 读取一个布尔值。
        /// </summary>
        /// <returns>布尔值。</returns>
        bool ReadBool();

        /// <summary>
        /// 读取一个字符。
        /// </summary>
        /// <returns>字符。</returns>
        char ReadChar();

        /// <summary>
        /// 读取一个16位整数。
        /// </summary>
        /// <returns>16位整数。</returns>
        short ReadInt16();

        /// <summary>
        /// 读取一个32位整数。
        /// </summary>
        /// <returns>32位整数。</returns>
        int ReadInt32();

        /// <summary>
        /// 读取一个64位整数。
        /// </summary>
        /// <returns>64位整数。</returns>
        long ReadInt64();

        /// <summary>
        /// 读取一个单精度浮点数。
        /// </summary>
        /// <returns>单精度浮点数。</returns>
        float ReadFloat();

        /// <summary>
        /// 读取一个双精度浮点数。
        /// </summary>
        /// <returns>双精度浮点数。</returns>
        double ReadDouble();

        /// <summary>
        /// 读取一个字符串。
        /// </summary>
        /// <returns>字符串。</returns>
        string ReadString();

        /// <summary>
        /// 读取可序列化对象。
        /// </summary>
        /// <returns>可序列化对象。</returns>
        T ReadObject<T>() where T : ISerializable;
    }
}