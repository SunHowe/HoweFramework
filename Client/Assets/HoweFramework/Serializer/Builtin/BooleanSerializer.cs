using System;

namespace HoweFramework
{
    /// <summary>
    /// bool 类型的二进制序列化器。
    /// </summary>
    public sealed class BooleanSerializer : ICustomSerializer<bool>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 bool 序列化器实例。
        /// </summary>
        public static ICustomSerializer<bool> Create()
        {
            return ReferencePool.Acquire<BooleanSerializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in bool obj)
        {
            if (buffer.Length < 1) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Boolean");
            buffer[0] = obj ? (byte)1 : (byte)0;
            return 1;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref bool obj)
        {
            if (buffer.Length < 1) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Boolean");
            obj = buffer[0] != 0;
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