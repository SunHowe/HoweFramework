using System;

namespace HoweFramework
{
    /// <summary>
    /// int 类型的二进制序列化器。
    /// </summary>
    public sealed class Int32Serializer : ICustomSerializer<int>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 int 序列化器实例。
        /// </summary>
        public static ICustomSerializer<int> Create()
        {
            return ReferencePool.Acquire<Int32Serializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in int obj)
        {
            if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Int32");
            BitConverter.TryWriteBytes(buffer, obj);
            return 4;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref int obj)
        {
            if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Int32");
            obj = BitConverter.ToInt32(buffer);
            return 4;
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