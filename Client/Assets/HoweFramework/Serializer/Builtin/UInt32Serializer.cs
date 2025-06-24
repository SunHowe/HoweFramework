using System;

namespace HoweFramework
{
    /// <summary>
    /// uint 类型的二进制序列化器。
    /// </summary>
    public sealed class UInt32Serializer : ICustomSerializer<uint>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 uint 序列化器实例。
        /// </summary>
        public static ICustomSerializer<uint> Create()
        {
            return ReferencePool.Acquire<UInt32Serializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in uint obj)
        {
            if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for UInt32");
            BitConverter.TryWriteBytes(buffer, obj);
            return 4;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref uint obj)
        {
            if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for UInt32");
            obj = BitConverter.ToUInt32(buffer);
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