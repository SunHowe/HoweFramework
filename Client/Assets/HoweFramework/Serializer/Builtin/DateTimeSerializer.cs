using System;

namespace HoweFramework
{
    /// <summary>
    /// DateTime 类型的二进制序列化器。
    /// </summary>
    public sealed class DateTimeSerializer : ICustomSerializer<DateTime>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 DateTime 序列化器实例。
        /// </summary>
        public static ICustomSerializer<DateTime> Create()
        {
            return ReferencePool.Acquire<DateTimeSerializer>();
        }

        public int Serialize(Span<byte> buffer, in DateTime obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for DateTime");
            BitConverter.TryWriteBytes(buffer, obj.Ticks);
            return 8;
        }

        public int Deserialize(ReadOnlySpan<byte> buffer, ref DateTime obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for DateTime");
            obj = new DateTime(BitConverter.ToInt64(buffer));
            return 8;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Clear() { }
    }
} 