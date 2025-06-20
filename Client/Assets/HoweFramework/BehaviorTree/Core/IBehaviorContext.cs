namespace HoweFramework
{
    /// <summary>
    /// 行为树上下文接口。
    /// </summary>
    public interface IBehaviorContext
    {
        /// <summary>
        /// 将值写入行为树上下文。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// 从行为树上下文获取值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回值。</returns>
        T GetValue<T>(string key, T defaultValue = default);

        /// <summary>
        /// 从行为树上下文获取值的字符串。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回值。</returns>
        string GetValueString(string key);

        /// <summary>
        /// 移除值。
        /// </summary>
        /// <param name="key">键。</param>
        void RemoveValue(string key);
    }
}