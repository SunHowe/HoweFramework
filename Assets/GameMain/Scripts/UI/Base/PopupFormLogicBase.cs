using HoweFramework;

namespace GameMain.UI
{
    /// <summary>
    /// 弹窗界面逻辑基类。
    /// </summary>
    public abstract class PopupFormLogicBase : FairyGUIFormLogicBase
    {
        /// <summary>
        /// 界面组编号。
        /// </summary>
        public override int FormGroupId => (int)UIGroupId.Main;

        /// <summary>
        /// 界面类型。
        /// </summary>
        public override UIFormType FormType => UIFormType.Popup;

        /// <summary>
        /// 是否允许同时打开多个界面实例。
        /// </summary>
        public override bool IsAllowMutiple => false;

        /// <summary>
        /// 屏幕适配器类型。
        /// </summary>
        public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.ConstantCenter;
    }
}
