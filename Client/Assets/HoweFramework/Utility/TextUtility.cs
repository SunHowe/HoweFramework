using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 获取模板值委托。
    /// </summary>
    /// <param name="key">键。</param>
    /// <param name="value">值。</param>
    /// <returns>是否获取到值。</returns>
    public delegate bool GetTemplateValue(string key, out string value);

    /// <summary>
    /// 文本模板辅助器。
    /// </summary>
    public interface ITextTemplateHelper : IDisposable
    {
        /// <summary>
        /// 解析文本模板。
        /// </summary>
        /// <param name="template">文本模板。</param>
        /// <param name="getTemplateValue">获取模板值委托实例。</param>
        /// <returns>解析后的文本。</returns>
        string ParseTemplate(string template, GetTemplateValue getTemplateValue);

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
    /// 文本模板格式化器。
    /// </summary>
    public interface ITextTemplateFormatter : IDisposable
    {
        /// <summary>
        /// 设置文本模板变量。
        /// </summary>
        ITextTemplateFormatter SetVar(string key, string value);

        /// <summary>
        /// 获取格式化后的文本。
        /// </summary>
        /// <param name="autoDispose">是否自动释放。</param>
        /// <returns>格式化后的文本。</returns>
        string GetText(bool autoDispose = true);
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
        /// <param name="getTemplateValue">获取模板值委托实例。</param>
        /// <returns>解析后的文本。</returns>
        public static string ParseTemplate(string template, GetTemplateValue getTemplateValue)
        {
            if (s_TextTemplateHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Text template helper is not set.");
            }

            return s_TextTemplateHelper.ParseTemplate(template, getTemplateValue);
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

        /// <summary>
        /// 设置文本模板变量。
        /// </summary>
        /// <param name="text">文本。</param>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <returns>文本模板格式化器。</returns>
        public static ITextTemplateFormatter SetVar(this string text, string key, string value)
        {
            return TextTemplateFormatter.Create(text).SetVar(key, value);
        }
    }

    internal sealed class TextTemplateFormatter : ITextTemplateFormatter, IReference
    {
        private string m_Text;
        private readonly Dictionary<string, string> m_Dictionary = new ();

        public void Clear()
        {
            m_Text = null;
            m_Dictionary.Clear();
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public string GetText(bool autoDispose = true)
        {
            var text = TextUtility.ParseTemplate(m_Text, m_Dictionary.TryGetValue);

            if (autoDispose)
            {
                Dispose();
            }

            return text;
        }

        public ITextTemplateFormatter SetVar(string key, string value)
        {
            m_Dictionary[key] = value;
            return this;
        }

        public static TextTemplateFormatter Create(string text)
        {
            var formatter = ReferencePool.Acquire<TextTemplateFormatter>();
            formatter.m_Text = text;
            return formatter;
        }
    }
}
