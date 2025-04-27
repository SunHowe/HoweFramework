namespace ProtocolGenerator
{
    public static class TextUtility
    {
        /// <summary>
        /// 将字符串转换为驼峰命名法
        /// </summary>
        /// <param name="str">需要转换的字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string CamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
