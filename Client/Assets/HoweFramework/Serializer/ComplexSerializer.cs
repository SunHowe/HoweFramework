using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 通用复杂类型序列化器实现，支持自定义序列化器注册与泛型序列化。
    /// </summary>
    public sealed class ComplexSerializer : ISerializer, IReference
    {
        private readonly Dictionary<Type, IDisposable> m_CustomSerializers = new();

        /// <inheritdoc/>
        public void RegisterCustomSerializer<T>(ICustomSerializer<T> serializer)
        {
            if (serializer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "Serializer is null");
            }

            if (m_CustomSerializers.ContainsKey(typeof(T)))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Serializer already registered");
            }

            m_CustomSerializers[typeof(T)] = serializer;
        }

        /// <inheritdoc/>
        public void UnRegisterCustomSerializer<T>()
        {
            m_CustomSerializers.Remove(typeof(T));
        }

        /// <inheritdoc/>
        public void UnRegisterAllCustomSerializers()
        {
            foreach (var ser in m_CustomSerializers.Values)
            {
                ser.Dispose();
            }

            m_CustomSerializers.Clear();
        }

        /// <inheritdoc/>
        public void Serialize<T>(Span<byte> buffer, in T obj)
        {
            if (m_CustomSerializers.TryGetValue(typeof(T), out var serObj) && serObj is ICustomSerializer<T> serializer)
            {
                serializer.Serialize(buffer, in obj);
            }
            else
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"No custom serializer registered for type {typeof(T)}");
            }
        }

        /// <inheritdoc/>
        public void Deserialize<T>(ReadOnlySpan<byte> buffer, ref T obj)
        {
            if (m_CustomSerializers.TryGetValue(typeof(T), out var serObj) && serObj is ICustomSerializer<T> serializer)
            {
                serializer.Deserialize(buffer, ref obj);
            }
            else
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"No custom serializer registered for type {typeof(T)}");
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            UnRegisterAllCustomSerializers();
        }
    }
}