using System;

namespace HoweFramework
{
    /// <summary>
    /// float 类型的二进制序列化器。
    /// </summary>
    public sealed class SingleSerializer : ICustomSerializer<float>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 float 序列化器实例。
        /// </summary>
        public static ICustomSerializer<float> Create()
        {
            return ReferencePool.Acquire<SingleSerializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in float obj)
        {
            if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Single");
            BitConverter.TryWriteBytes(buffer, obj);
            return 4;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref float obj)
        {
            if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Single");
            obj = BitConverter.ToSingle(buffer);
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