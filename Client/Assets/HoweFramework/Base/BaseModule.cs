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
        }

        protected override void OnDestroy()
        {
            JsonUtility.DisposeJsonHelper();
            TextUtility.DisposeTextTemplateHelper();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
