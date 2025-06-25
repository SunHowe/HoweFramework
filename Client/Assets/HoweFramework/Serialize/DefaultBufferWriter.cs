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
            int byteCount = ConverterUtility.GetBytes(value, m_Buffer, Position + 4); // 预留4字节写长度
            WriteInt32(byteCount); // 实际写入长度
            Position += byteCount;
        }

        private void EnsureCapacity(int size)
        {
            if (Position + size <= BufferSize)
            {
                return;
            }

            Array.Resize(ref m_Buffer, Position + size);
        }
    }
}