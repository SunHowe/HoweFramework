using System;

namespace HoweFramework
{
    /// <summary>
    /// 时间工具。
    /// </summary>
    public static class TimeUtility
    {
        /// <summary>
        /// 1970年1月1日0时0分0秒。
        /// </summary>
        private static readonly DateTime m_UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取Unix时间戳。
        /// </summary>
        /// <returns>Unix时间戳。</returns>
        public static int ToUnixTimestamp(this DateTime dateTime)
        {
            return (int)(dateTime - m_UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// 获取Unix时间戳毫秒。
        /// </summary>
        /// <returns>Unix时间戳毫秒。</returns>
        public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
        {
            return (long)(dateTime - m_UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// 从Unix时间戳转换为DateTime。
        /// </summary>
        /// <param name="unixTimestamp">Unix时间戳。</param>
        /// <returns>DateTime。</returns>
        public static DateTime FromUnixTimestamp(int unixTimestamp)
        {
            return m_UnixEpoch.AddSeconds(unixTimestamp);
        }

        /// <summary>
        /// 从Unix时间戳毫秒转换为DateTime。
        /// </summary>
        /// <param name="unixTimestampMilliseconds">Unix时间戳毫秒。</param>
        /// <returns>DateTime。</returns>
        public static DateTime FromUnixTimestampMilliseconds(long unixTimestampMilliseconds)
        {
            return m_UnixEpoch.AddMilliseconds(unixTimestampMilliseconds);
        }
    }
}