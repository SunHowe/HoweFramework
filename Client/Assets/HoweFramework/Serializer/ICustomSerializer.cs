using System;

namespace HoweFramework
{
    /// <summary>
    /// 自定义序列化器泛型接口。
    /// </summary>
    public interface ICustomSerializer<T> : IDisposable
    {
        /// <summary>
        /// 序列化对象。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="obj">对象。</param>
        /// <returns>写入的字节数。</returns>
        int Serialize(Span<byte> buffer, in T obj);

        /// <summary>
        /// 反序列化对象。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>读取的字节数。</returns>
        int Deserialize(ReadOnlySpan<byte> buffer, ref T obj);
    }
}