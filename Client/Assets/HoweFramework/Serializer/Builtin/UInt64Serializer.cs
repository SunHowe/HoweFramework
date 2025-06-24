using System;

namespace HoweFramework
{
    /// <summary>
    /// ulong 类型的二进制序列化器。
    /// </summary>
    public sealed class UInt64Serializer : ICustomSerializer<ulong>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 ulong 序列化器实例。
        /// </summary>
        public static ICustomSerializer<ulong> Create()
        {
            return ReferencePool.Acquire<UInt64Serializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in ulong obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for UInt64");
            BitConverter.TryWriteBytes(buffer, obj);
            return 8;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref ulong obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for UInt64");
            obj = BitConverter.ToUInt64(buffer);
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