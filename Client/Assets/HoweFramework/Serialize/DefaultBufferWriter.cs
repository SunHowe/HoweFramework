using System;

namespace HoweFramework
{
    /// <summary>
    /// 默认缓冲区写入器。
    /// </summary>
    public sealed class DefaultBufferWriter : IBufferWriter, IReference
    {
        /// <inheritdoc/>
        public int BufferSize => m_Buffer.Length;

        /// <inheritdoc/>
        public int Position { get; set; }

        /// <summary>
        /// 缓冲区。
        /// </summary>
        private byte[] m_Buffer = new byte[1024];

        /// <inheritdoc/>
        public Span<byte> WrittenBuffer => m_Buffer.AsSpan(0, Position);

        /// <inheritdoc/>
        public void Clear()
        {
            Position = 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <inheritdoc/>
        public void WriteByte(byte value)
        {
            EnsureCapacity(1);
            m_Buffer[Position++] = value;
        }

        /// <inheritdoc/>
        public void WriteBytes(ReadOnlySpan<byte> value)
        {
            EnsureCapacity(value.Length);
            value.CopyTo(m_Buffer.AsSpan(Position));
            Position += value.Length;
        }

        public void WriteBool(bool value)
        {
            EnsureCapacity(1);
            m_Buffer[Position++] = value ? (byte)1 : (byte)0;
        }

        public void WriteChar(char value)
        {
            EnsureCapacity(2);
            ConverterUtility.GetBytes(value, m_Buffer, Position);
            Position += 2;
        }

        public void WriteInt16(short value)
        {
            EnsureCapacity(2);
            ConverterUtility.GetBytes(value, m_Buffer, Position);
            Position += 2;
        }

        public void WriteInt32(int value)
        {
            EnsureCapacity(4);
            ConverterUtility.GetBytes(value, m_Buffer, Position);
            Position += 4;
        }

        public void WriteInt64(long value)
        {
            EnsureCapacity(8);
            ConverterUtility.GetBytes(value, m_Buffer, Position);
            Position += 8;
        }

        public void WriteFloat(float value)
        {
            EnsureCapacity(4);
            ConverterUtility.GetBytes(value, m_Buffer, Position);
            Position += 4;
        }

        public void WriteDouble(double value)
        {
            EnsureCapacity(8);
            ConverterUtility.GetBytes(value, m_Buffer, Position);
            Position += 8;
        }

        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteInt32(0);
                return;
            }

            var byteCount = ConverterUtility.GetByteCount(value);
            EnsureCapacity(byteCount + 4);

            ConverterUtility.GetBytes(value, m_Buffer, Position + 4);
            WriteInt32(byteCount);

            Position += byteCount;
        }

        public void WriteObject<T>(T value) where T : ISerializable
        {
            if (value == null)
            {
                WriteInt32(-1);
                return;
            }

            var cachePosition = Position;
            var valueBeginPosition = Position + 4;

            Position = valueBeginPosition;
            value.Serialize(this);

            var valueEndPosition = Position;
            var byteCount = valueEndPosition - valueBeginPosition;

            Position = cachePosition;
            WriteInt32(byteCount);

            Position = valueEndPosition;
        }

        /// <summary>
        /// 确保缓冲区有足够的空间。
        /// </summary>
        /// <param name="size">需要确保的缓冲区大小。</param>
        private void EnsureCapacity(int size)
        {
            if (Position + size <= BufferSize)
            {
                return;
            }

            Array.Resize(ref m_Buffer, Position + size);
        }

        /// <summary>
        /// 创建缓冲区写入器。
        /// </summary>
        /// <returns>缓冲区写入器。</returns>
        public static DefaultBufferWriter Create()
        {
            return ReferencePool.Acquire<DefaultBufferWriter>();
        }
    }
}