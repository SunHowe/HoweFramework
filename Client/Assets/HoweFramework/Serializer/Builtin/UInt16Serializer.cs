using System;

namespace HoweFramework
{
    /// <summary>
    /// ushort 类型的二进制序列化器。
    /// </summary>
    public sealed class UInt16Serializer : ICustomSerializer<ushort>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 ushort 序列化器实例。
        /// </summary>
        public static ICustomSerializer<ushort> Create()
        {
            return ReferencePool.Acquire<UInt16Serializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in ushort obj)
        {
            if (buffer.Length < 2) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for UInt16");
            BitConverter.TryWriteBytes(buffer, obj);
            return 2;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref ushort obj)
        {
            if (buffer.Length < 2) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for UInt16");
            obj = BitConverter.ToUInt16(buffer);
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