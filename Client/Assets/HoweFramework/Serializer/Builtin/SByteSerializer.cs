using System;

namespace HoweFramework
{
    /// <summary>
    /// sbyte 类型的二进制序列化器。
    /// </summary>
    public sealed class SByteSerializer : ICustomSerializer<sbyte>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 sbyte 序列化器实例。
        /// </summary>
        public static ICustomSerializer<sbyte> Create()
        {
            return ReferencePool.Acquire<SByteSerializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in sbyte obj)
        {
            if (buffer.Length < 1) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for SByte");
            buffer[0] = (byte)obj;
            return 1;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref sbyte obj)
        {
            if (buffer.Length < 1) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for SByte");
            obj = (sbyte)buffer[0];
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