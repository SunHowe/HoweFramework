using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 文本模板辅助器。
    /// </summary>
    public interface ITextTemplateHelper : IDisposable
    {
        /// <summary>
        /// 解析文本模板。
        /// </summary>
        /// <param name="template">文本模板。</param>
        /// <param name="dictionary">参数。</param>
        /// <returns>解析后的文本。</returns>
        string ParseTemplate(string template, Dictionary<string, string> dictionary);

        /// <summary>
        /// 添加全局文本模板值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        void AddGlobalTemplateValue(string key, string value);

        /// <summary>
        /// 移除全局文本模板值。
        /// </summary>
        /// <param name="key">键。</param>
        void RemoveGlobalTemplateValue(string key);
    }

    /// <summary>
    /// 文本工具类。
    /// </summary>
    public static class TextUtility
    {
        private static ITextTemplateHelper s_TextTemplateHelper;

        /// <summary>
        /// 设置文本模板辅助器。
        /// </summary>
        /// <param name="textTemplateHelper">文本模板辅助器。</param>
        public static void SetTextTemplateHelper(ITextTemplateHelper textTemplateHelper)
        {
            s_TextTemplateHelper = textTemplateHelper;
        }

        /// <summary>
        /// 销毁文本模板辅助器。
        /// </summary>
        public static void DisposeTextTemplateHelper()
        {
            if (s_TextTemplateHelper != null)
            {
                s_TextTemplateHelper.Dispose();
                s_TextTemplateHelper = null;
            }
        }

        /// <summary>
        /// 解析文本模板。
        /// </summary>
        /// <param name="template">文本模板。</param>
        /// <param name="dictionary">参数。</param>
        /// <returns>解析后的文本。</returns>
        public static string ParseTemplate(string template, Dictionary<string, string> dictionary)
        {
            if (s_TextTemplateHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Text template helper is not set.");
            }

            return s_TextTemplateHelper.ParseTemplate(template, dictionary);
        }

        /// <summary>
        /// 添加全局文本模板值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        public static void AddGlobalTemplateValue(string key, string value)
        {
            if (s_TextTemplateHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Text template helper is not set.");
            }

            s_TextTemplateHelper.AddGlobalTemplateValue(key, value);
        }

        /// <summary>
        /// 移除全局文本模板值。
        /// </summary>
        /// <param name="key">键。</param>
        public static void RemoveGlobalTemplateValue(string key)
        {
            if (s_TextTemplateHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Text template helper is not set.");
            }

            s_TextTemplateHelper.RemoveGlobalTemplateValue(key);
        }
    }
}
