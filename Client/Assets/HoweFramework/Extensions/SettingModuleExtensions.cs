namespace HoweFramework
{
    /// <summary>
    /// 设置模块扩展。
    /// </summary>
    public static class SettingModuleExtensions
    {
        /// <summary>
        /// 使用基于文件流的设置。
        /// </summary>
        /// <param name="settingModule">设置模块。</param>
        /// <returns>设置模块。</returns>
        public static SettingModule UseFileSetting(this SettingModule settingModule)
        {
            settingModule.SetSettingHelper(new FileSettingHelper());
            return settingModule;
        }

        /// <summary>
        /// 使用基于 PlayerPrefs 的设置。
        /// </summary>
        /// <param name="settingModule">设置模块。</param>
        /// <returns>设置模块。</returns>
        public static SettingModule UsePlayerPrefsSetting(this SettingModule settingModule)
        {
            settingModule.SetSettingHelper(new PlayerPrefsSettingHelper());
            return settingModule;
        }
    }
}
