using System;

namespace HoweFramework
{
    /// <summary>
    /// 序列化器接口。
    /// </summary>
    public interface ISerializer : IDisposable
    {
        /// <summary>
        /// 注册自定义序列化器。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="serializer">序列化器。</param>
        void RegisterCustomSerializer<T>(ICustomSerializer<T> serializer);

        /// <summary>
        /// 注销自定义序列化器。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        void UnRegisterCustomSerializer<T>();

        /// <summary>
        /// 注销所有自定义序列化器。
        /// </summary>
        void UnRegisterAllCustomSerializers();

        /// <summary>
        /// 序列化对象。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="obj">对象。</param>
        void Serialize<T>(Span<byte> buffer, in T obj);

        /// <summary>
        /// 反序列化对象。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="buffer">缓冲区。</param>
        void Deserialize<T>(ReadOnlySpan<byte> buffer, ref T obj);
    }
}