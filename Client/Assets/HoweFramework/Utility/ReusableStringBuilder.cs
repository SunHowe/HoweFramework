using System;
using System.Text;

namespace HoweFramework
{
    /// <summary>
    /// 可复用字符串构建器。
    /// </summary>
    public sealed class ReusableStringBuilder : IReference, IDisposable
    {
        private readonly StringBuilder m_StringBuilder = new StringBuilder();
        private bool m_IsReferenced = false;

        /// <summary>
        /// 追加字符串。
        /// </summary>
        /// <param name="str">要追加的字符串。</param>
        public void Append(string str)
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            m_StringBuilder.Append(str);
        }

        /// <summary>
        /// 追加字符串。
        /// </summary>
        /// <param name="str">要追加的字符串。</param>
        /// <param name="startIndex">开始索引。</param>
        /// <param name="length">长度。</param>
        /// <exception cref="ErrorCodeException">StringBuilder未引用时抛出。</exception>
        public void Append(string str, int startIndex, int length)
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            m_StringBuilder.Append(str, startIndex, length);
        }
        
        /// <summary>
        /// 追加字符。
        /// </summary>
        /// <param name="ch">要追加的字符。</param>
        public void Append(char ch)
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            m_StringBuilder.Append(ch);
        }

        /// <summary>
        /// 追加格式化字符串。
        /// </summary>
        /// <param name="format">格式化字符串。</param>
        /// <param name="args">格式化参数。</param>
        public void AppendFormat(string format, params object[] args)
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            m_StringBuilder.AppendFormat(format, args);
        }

        /// <summary>
        /// 追加换行符。
        /// </summary>
        public void AppendLine()
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            m_StringBuilder.AppendLine();
        }

        /// <summary>
        /// 追加一行文本并在结尾添加换行符。
        /// </summary>
        /// <param name="value">要追加的值。</param>
        public void AppendLine(string value)
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            m_StringBuilder.AppendLine(value);
        }
        
        public override string ToString()
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            return m_StringBuilder.ToString();
        }

        public string GetStringAndRelease()
        {
            if (!m_IsReferenced)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "StringBuilder is not referenced.");
            }

            var str = m_StringBuilder.ToString();
            Dispose();
            return str;
        }

        public void Clear()
        {
            m_IsReferenced = false;
            m_StringBuilder.Clear();
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static ReusableStringBuilder Create()
        {
            var sb = ReferencePool.Acquire<ReusableStringBuilder>();
            sb.m_IsReferenced = true;
            return sb;
        }
    }
}
