using static FairyGUI.UIObjectFactory;

namespace HoweFramework
{
    /// <summary>
    /// FairyGUI组件绑定信息。
    /// </summary>
    public sealed class FairyGUIComponentBinding
    {
        /// <summary>
        /// 组件URL。
        /// </summary>
        public string ComponentURL { get; }

        /// <summary>
        /// 组件建造者。
        /// </summary>
        public GComponentCreator Creator { get; }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="componentURL">组件URL。</param>
        /// <param name="creator">组件建造者。</param>
        public FairyGUIComponentBinding(string componentURL, GComponentCreator creator)
        {
            ComponentURL = componentURL;
            Creator = creator;
        }
    }
}
