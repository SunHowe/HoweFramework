using System;

namespace HoweFramework
{
    /// <summary>
    /// 默认缓冲区读取器。
    /// </summary>
    public class DefaultBufferReader : IBufferReader, IReference
    {
        /// <inheritdoc/>
        public int BufferSize { get; private set; }

        /// <inheritdoc/>
        public int Position { get; set; }

        /// <summary>
        /// 缓冲区。
        /// </summary>
        private byte[] m_Buffer;

        /// <inheritdoc/>
        public void Clear()
        {
            m_Buffer = null;
            BufferSize = 0;
            Position = 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <inheritdoc/>
        public byte ReadByte()
        {
            if (Position >= BufferSize)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            }

            return m_Buffer[Position++];
        }

        /// <inheritdoc/>
        public ReadOnlySpan<byte> ReadBytes(int size)
        {
            if (Position + size > BufferSize)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            }

            var result = new ReadOnlySpan<byte>(m_Buffer, Position, size);
            Position += size;
            return result;
        }

        /// <summary>
        /// 创建缓冲区读取器。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <returns>缓冲区读取器。</returns>
        public static DefaultBufferReader Create(byte[] buffer)
        {
            var reader = ReferencePool.Acquire<DefaultBufferReader>();
            reader.m_Buffer = buffer;
            reader.BufferSize = buffer.Length;
            return reader;
        }

        public bool ReadBool()
        {
            if (Position + 1 > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            bool value = m_Buffer[Position] != 0;
            Position += 1;
            return value;
        }

        public char ReadChar()
        {
            if (Position + 2 > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            char value = ConverterUtility.GetChar(m_Buffer, Position);
            Position += 2;
            return value;
        }

        public short ReadInt16()
        {
            if (Position + 2 > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            short value = ConverterUtility.GetInt16(m_Buffer, Position);
            Position += 2;
            return value;
        }

        public int ReadInt32()
        {
            if (Position + 4 > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            int value = ConverterUtility.GetInt32(m_Buffer, Position);
            Position += 4;
            return value;
        }

        public long ReadInt64()
        {
            if (Position + 8 > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            long value = ConverterUtility.GetInt64(m_Buffer, Position);
            Position += 8;
            return value;
        }

        public float ReadFloat()
        {
            if (Position + 4 > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            float value = ConverterUtility.GetSingle(m_Buffer, Position);
            Position += 4;
            return value;
        }

        public double ReadDouble()
        {
            if (Position + 8 > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            double value = ConverterUtility.GetDouble(m_Buffer, Position);
            Position += 8;
            return value;
        }

        public string ReadString()
        {
            int length = ReadInt32();
            if (length == 0) return string.Empty;
            if (Position + length > BufferSize)
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "缓冲区读取器已到达末尾。");
            string value = ConverterUtility.GetString(m_Buffer, Position, length);
            Position += length;
            return value;
        }
    }
}