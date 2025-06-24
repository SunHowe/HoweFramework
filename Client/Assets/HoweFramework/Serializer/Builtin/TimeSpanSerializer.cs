using System;

namespace HoweFramework
{
    /// <summary>
    /// TimeSpan 类型的二进制序列化器。
    /// </summary>
    public sealed class TimeSpanSerializer : ICustomSerializer<TimeSpan>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 TimeSpan 序列化器实例。
        /// </summary>
        public static ICustomSerializer<TimeSpan> Create()
        {
            return ReferencePool.Acquire<TimeSpanSerializer>();
        }

        public int Serialize(Span<byte> buffer, in TimeSpan obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for TimeSpan");
            BitConverter.TryWriteBytes(buffer, obj.Ticks);
            return 8;
        }

        public int Deserialize(ReadOnlySpan<byte> buffer, ref TimeSpan obj)
        {
            if (buffer.Length < 8) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for TimeSpan");
            obj = new TimeSpan(BitConverter.ToInt64(buffer));
            return 8;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Clear() { }
    }
} 