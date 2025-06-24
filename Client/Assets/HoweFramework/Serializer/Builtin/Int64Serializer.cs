using System;

namespace HoweFramework
{
    /// <summary>
    /// long 类型的二进制序列化器。
    /// </summary>
    public sealed class Int64Serializer : ICustomSerializer<long>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 long 序列化器实例。
        /// </summary>
        public static ICustomSerializer<long> Create()
        {
            return ReferencePool.Acquire<Int64Serializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in long obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Int64");
            BitConverter.TryWriteBytes(buffer, obj);
            return 8;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref long obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Int64");
            obj = BitConverter.ToInt64(buffer);
            return 8;
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