using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 默认文本模板辅助器。
    /// </summary>
    public sealed class DefaultTextTemplateHelper : ITextTemplateHelper
    {
        /// <summary>
        /// 全局文本模板值。
        /// </summary>
        private readonly Dictionary<string, string> m_GlobalTemplateValues = new();

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            m_GlobalTemplateValues.Clear();
        }

        /// <summary>
        /// 添加全局文本模板值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        public void AddGlobalTemplateValue(string key, string value)
        {
            m_GlobalTemplateValues[key] = value;
        }

        /// <summary>
        /// 移除全局文本模板值。
        /// </summary>
        /// <param name="key">键。</param>
        public void RemoveGlobalTemplateValue(string key)
        {
            m_GlobalTemplateValues.Remove(key);
        }

        /// <summary>
        /// 解析文本模板。
        /// </summary>
        /// <param name="template">文本模板。</param>
        /// <param name="dictionary">参数。</param>
        /// <returns>解析后的文本。</returns>
        public string ParseTemplate(string template, Dictionary<string, string> dictionary)
        {
            int pos1 = 0;
            int pos2;
            int pos3;
            string tag;
            string value;
            using var buffer = ReusableStringBuilder.Create();
            var anyMatch = false;
            
            while ((pos2 = template.IndexOf('{', pos1)) != -1)
            {
                if (pos2 > 0 && template[pos2 - 1] == '\\')
                {
                    buffer.Append(template, pos1, pos2 - pos1 - 1);
                    buffer.Append('{');
                    pos1 = pos2 + 1;
                    continue;
                }

                buffer.Append(template, pos1, pos2 - pos1);
                pos1 = pos2;
                pos2 = template.IndexOf('}', pos1);
                if (pos2 == -1)
                {
                    break;
                }

                if (pos2 == pos1 + 1)
                {
                    buffer.Append(template, pos1, 2);
                    pos1 = pos2 + 1;
                    continue;
                }

                anyMatch = true;

                tag = template.Substring(pos1 + 1, pos2 - pos1 - 1);
                pos3 = tag.IndexOf('=');
                if (pos3 != -1)
                {
                    // 文本中设置了默认值的情况。
                    var key = tag.Substring(0, pos3);

                    if (dictionary != null && dictionary.TryGetValue(key, out value))
                    {
                        // 从参数字典中获取值。
                    }
                    else if (m_GlobalTemplateValues.TryGetValue(key, out value))
                    {
                        // 从全局文本模板值中获取值。
                    }
                    else
                    {
                        // 没有获取到值，使用默认值。
                        value = tag.Substring(pos3 + 1);
                    }
                }
                else
                {
                    // 文本中没有设置默认值的情况。
                    if (dictionary != null && dictionary.TryGetValue(tag, out value))
                    {
                        // 从参数字典中获取值。
                    }
                    else if (m_GlobalTemplateValues.TryGetValue(tag, out value))
                    {
                        // 从全局文本模板值中获取值。
                    }
                    else
                    {
                        // 没有获取到值，使用默认值。
                        value = string.Empty;
                    }
                }
                
                buffer.Append(value);
                pos1 = pos2 + 1;
            }

            if (!anyMatch)
            {
                // 文本中没有设置任何模板值的情况。返回原字符串。
                return template;
            }

            if (pos1 < template.Length)
            {
                // 剩余部分文本。
                buffer.Append(template, pos1, template.Length - pos1);
            }

            return buffer.ToString();
        }
    }
}

