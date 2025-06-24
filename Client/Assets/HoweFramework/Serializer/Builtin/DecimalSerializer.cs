using System;

namespace HoweFramework
{
    /// <summary>
    /// decimal 类型的二进制序列化器。
    /// </summary>
    public sealed class DecimalSerializer : ICustomSerializer<decimal>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 decimal 序列化器实例。
        /// </summary>
        public static ICustomSerializer<decimal> Create()
        {
            return ReferencePool.Acquire<DecimalSerializer>();
        }

        public int Serialize(Span<byte> buffer, in decimal obj)
        {
            if (buffer.Length < 16) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Decimal");
            var bits = decimal.GetBits(obj);
            for (int i = 0; i < 4; i++)
            {
                BitConverter.TryWriteBytes(buffer.Slice(i * 4, 4), bits[i]);
            }
            return 16;
        }

        public int Deserialize(ReadOnlySpan<byte> buffer, ref decimal obj)
        {
            if (buffer.Length < 16) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Decimal");
            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
            {
                bits[i] = BitConverter.ToInt32(buffer.Slice(i * 4, 4));
            }
            obj = new decimal(bits);
            return 16;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Clear() { }
    }
} 