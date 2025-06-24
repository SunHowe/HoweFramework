using System;

namespace HoweFramework
{
    /// <summary>
    /// short 类型的二进制序列化器。
    /// </summary>
    public sealed class Int16Serializer : ICustomSerializer<short>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 short 序列化器实例。
        /// </summary>
        public static ICustomSerializer<short> Create()
        {
            return ReferencePool.Acquire<Int16Serializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in short obj)
        {
            if (buffer.Length < 2) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Int16");
            BitConverter.TryWriteBytes(buffer, obj);
            return 2;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref short obj)
        {
            if (buffer.Length < 2) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Int16");
            obj = BitConverter.ToInt16(buffer);
            return 2;
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