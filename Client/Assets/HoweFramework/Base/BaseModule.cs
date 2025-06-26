namespace HoweFramework
{
    /// <summary>
    /// 基础模块。
    /// </summary>
    public sealed class BaseModule : ModuleBase<BaseModule>
    {
        /// <summary>
        /// 设置 Json 辅助器。
        /// </summary>
        /// <param name="jsonHelper">Json 辅助器。</param>
        public static void SetJsonHelper(IJsonHelper jsonHelper)
        {
            JsonUtility.SetJsonHelper(jsonHelper);
        }

        /// <summary>
        /// 设置文本模板辅助器。
        /// </summary>
        /// <param name="textTemplateHelper">文本模板辅助器。</param>
        public static void SetTextTemplateHelper(ITextTemplateHelper textTemplateHelper)
        {
            TextUtility.SetTextTemplateHelper(textTemplateHelper);
        }

        protected override void OnInit()
        {
            // 注册框架程序集。
            AssemblyUtility.RegisterRuntimeAssembly(GetType().Assembly);
        }

        protected override void OnDestroy()
        {
            JsonUtility.DisposeJsonHelper();
            TextUtility.DisposeTextTemplateHelper();
            AssemblyUtility.Clear();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
