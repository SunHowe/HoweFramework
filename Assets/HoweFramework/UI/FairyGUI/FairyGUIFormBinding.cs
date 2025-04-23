using System;

namespace HoweFramework
{
    /// <summary>
    /// 界面逻辑实例建造者方法。
    /// </summary>
    public delegate IUIFormLogic FormLogicCreator();

    /// <summary>
    /// FairyGUI界面绑定信息。
    /// </summary>
    public sealed class FairyGUIFormBinding
    {
        /// <summary>
        /// 界面Id。
        /// </summary>
        public int FormId { get; set; }

        /// <summary>
        /// 界面URL。
        /// </summary>
        public string FormURL { get; set; }

        /// <summary>
        /// 界面逻辑实例建造者方法。
        /// </summary>
        public FormLogicCreator Creator { get; set; }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="formId">界面Id。</param>
        /// <param name="formURL">界面URL。</param>
        /// <param name="creator">界面逻辑实例建造者方法。</param>
        public FairyGUIFormBinding(int formId, string formURL, FormLogicCreator creator)
        {
            FormId = formId;
            FormURL = formURL;
            Creator = creator;
        }
    }
}

