using System;

namespace HoweFramework
{
    /// <summary>
    /// Guid 类型的二进制序列化器。
    /// </summary>
    public sealed class GuidSerializer : ICustomSerializer<Guid>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 Guid 序列化器实例。
        /// </summary>
        public static ICustomSerializer<Guid> Create()
        {
            return ReferencePool.Acquire<GuidSerializer>();
        }

        public int Serialize(Span<byte> buffer, in Guid obj)
        {
            if (buffer.Length < 16) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Guid");
            obj.TryWriteBytes(buffer);
            return 16;
        }

        public int Deserialize(ReadOnlySpan<byte> buffer, ref Guid obj)
        {
            if (buffer.Length < 16) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Guid");
            obj = new Guid(buffer.Slice(0, 16));
            return 16;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Clear() { }
    }
} 