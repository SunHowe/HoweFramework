using System;
using System.Text;

namespace HoweFramework
{
    /// <summary>
    /// string 类型的二进制序列化器，采用 UTF-8 编码，前4字节为长度，支持 null。
    /// </summary>
    public sealed class StringSerializer : ICustomSerializer<string>, IReference
    {
        /// <summary>
        /// 从引用池获取一个新的 string 序列化器实例。
        /// </summary>
        public static ICustomSerializer<string> Create()
        {
            return ReferencePool.Acquire<StringSerializer>();
        }

        /// <inheritdoc/>
        public int Serialize(Span<byte> buffer, in string obj)
        {
            if (obj == null)
            {
                if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for null string");
                BitConverter.TryWriteBytes(buffer, -1);
                return 4;
            }
            var bytes = Encoding.UTF8.GetBytes(obj);
            if (buffer.Length < 4 + bytes.Length) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for string");
            BitConverter.TryWriteBytes(buffer, bytes.Length);
            bytes.CopyTo(buffer.Slice(4));
            return 4 + bytes.Length;
        }

        /// <inheritdoc/>
        public int Deserialize(ReadOnlySpan<byte> buffer, ref string obj)
        {
            if (buffer.Length < 4) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for string length");
            int len = BitConverter.ToInt32(buffer);
            if (len == -1) { obj = null; return 4; }
            if (buffer.Length < 4 + len) throw new ErrorCodeException(ErrorCode.InvalidParam, "Buffer too small for string content");
            obj = Encoding.UTF8.GetString(buffer.Slice(4, len));
            return 4 + len;
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