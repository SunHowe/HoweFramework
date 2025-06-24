using System;

namespace HoweFramework
{
    /// <summary>
    /// double 类型的二进制序列化器。
    /// </summary>
    public sealed class DoubleSerializer : ICustomSerializer<double>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 double 序列化器实例。
        /// </summary>
        public static ICustomSerializer<double> Create()
        {
            return ReferencePool.Acquire<DoubleSerializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in double obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Double");
            BitConverter.TryWriteBytes(buffer, obj);
            return 8;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref double obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Double");
            obj = BitConverter.ToDouble(buffer);
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