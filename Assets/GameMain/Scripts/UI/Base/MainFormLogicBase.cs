using HoweFramework;

namespace GameMain.UI
{
    /// <summary>
    /// 主界面逻辑基类。
    /// </summary>
    public abstract class MainFormLogicBase : FairyGUIFormLogicBase
    {
        /// <summary>
        /// 界面组编号。
        /// </summary>
        public override int FormGroupId => (int)UIGroupId.Background;

        /// <summary>
        /// 界面类型。
        /// </summary>
        public override UIFormType FormType => UIFormType.Main;

        /// <summary>
        /// 是否允许同时打开多个界面实例。
        /// </summary>
        public override bool IsAllowMutiple => false;

        /// <summary>
        /// 屏幕适配器类型。
        /// </summary>
        public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.FullScreen;

        protected override void OnCloseButtonClick()
        {
            // 主界面不允许关闭。
        }

        protected override void OnBackButtonClick()
        {
            // TODO 主界面返回弹出退出游戏提示。
        }
    }
}
