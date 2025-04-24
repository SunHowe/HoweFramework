using System;

namespace HoweFramework
{
    /// <summary>
    /// Json 辅助器接口。
    /// </summary>
    public interface IJsonHelper : IDisposable
    {
        /// <summary>
        /// 将对象序列化为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化后的 JSON 字符串。</returns>
        string ToJson(object obj);

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        T ToObject<T>(string json);

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <param name="objectType">对象类型。</param>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        object ToObject(Type objectType, string json);
    }

    /// <summary>
    /// Json 工具类。
    /// </summary>
    public static class JsonUtility
    {
        private static IJsonHelper s_JsonHelper = null;

        /// <summary>
        /// 设置 Json 辅助器。
        /// </summary>
        /// <param name="jsonHelper">Json 辅助器。</param>
        public static void SetJsonHelper(IJsonHelper jsonHelper)
        {
            s_JsonHelper = jsonHelper ?? throw new ErrorCodeException(ErrorCode.InvalidParam, "Json helper is invalid.");
        }

        /// <summary>
        /// 销毁 Json 辅助器。
        /// </summary>
        public static void DisposeJsonHelper()
        {
            if (s_JsonHelper != null)
            {
                s_JsonHelper.Dispose();
                s_JsonHelper = null;
            }
        }

        /// <summary>
        /// 将对象序列化为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化后的 JSON 字符串。</returns>
        public static string ToJson(object obj)
        {
            if (s_JsonHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Json helper is not set.");
            }

            return s_JsonHelper.ToJson(obj);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        public static T ToObject<T>(string json)
        {
            if (s_JsonHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Json helper is not set.");
            }

            return s_JsonHelper.ToObject<T>(json);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <param name="objectType">对象类型。</param>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        public static object ToObject(Type objectType, string json)
        {
            if (s_JsonHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Json helper is not set.");
            }

            return s_JsonHelper.ToObject(objectType, json);
        }
    }
}
