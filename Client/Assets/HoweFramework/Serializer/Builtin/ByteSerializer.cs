using System;

namespace HoweFramework
{
    /// <summary>
    /// byte 类型的二进制序列化器。
    /// </summary>
    public sealed class ByteSerializer : ICustomSerializer<byte>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 byte 序列化器实例。
        /// </summary>
        public static ICustomSerializer<byte> Create()
        {
            return ReferencePool.Acquire<ByteSerializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in byte obj)
        {
            if (buffer.Length < 1) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Byte");
            buffer[0] = obj;
            return 1;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref byte obj)
        {
            if (buffer.Length < 1) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Byte");
            obj = buffer[0];
            return 1;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <inheritdoc/>
        public void Clear() { }
    }
} 