namespace HoweFramework
{
    /// <summary>
    /// 基础模块扩展。
    /// </summary>
    public static class BaseModuleExtensions
    {
        /// <summary>
        /// 使用基于Unity默认实现的Json辅助器。
        /// </summary>
        /// <param name="baseModule">基础模块。</param>
        /// <returns>基础模块。</returns>
        public static BaseModule UseUnityJsonHelper(this BaseModule baseModule)
        {
            BaseModule.SetJsonHelper(new UnityJsonHelper());
            return baseModule;
        }

        /// <summary>
        /// 使用基于默认实现的文本模板辅助器。
        /// </summary>
        /// <param name="baseModule">基础模块。</param>
        /// <returns>基础模块。</returns>
        public static BaseModule UseDefaultTextTemplateHelper(this BaseModule baseModule)
        {
            BaseModule.SetTextTemplateHelper(new DefaultTextTemplateHelper());
            return baseModule;
        }
    }
}
