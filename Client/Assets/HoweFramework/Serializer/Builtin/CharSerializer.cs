using System;

namespace HoweFramework
{
    /// <summary>
    /// char 类型的二进制序列化器。
    /// </summary>
    public sealed class CharSerializer : ICustomSerializer<char>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 char 序列化器实例。
        /// </summary>
        public static ICustomSerializer<char> Create()
        {
            return ReferencePool.Acquire<CharSerializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in char obj)
        {
            if (buffer.Length < 2) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Char");
            BitConverter.TryWriteBytes(buffer, obj);
            return 2;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref char obj)
        {
            if (buffer.Length < 2) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for Char");
            obj = BitConverter.ToChar(buffer);
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