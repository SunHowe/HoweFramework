namespace HoweFramework
{
    /// <summary>
    /// 日志。
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// 打印调试日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        public static void Debug(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// 打印警告日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        /// <summary>
        /// 打印日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        public static void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// 打印错误日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// 打印致命错误日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        public static void Fatal(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
