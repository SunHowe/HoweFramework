using HoweFramework;

namespace GameMain.UI
{
    /// <summary>
    /// 固定界面逻辑基类。
    /// </summary>
    public abstract class FixedFormLogicBase : FairyGUIFormLogicBase
    {
        /// <summary>
        /// 界面组编号。
        /// </summary>
        public override int FormGroupId => (int)UIGroupId.Tips;

        /// <summary>
        /// 界面类型。
        /// </summary>
        public override UIFormType FormType => UIFormType.Fixed;

        /// <summary>
        /// 是否允许同时打开多个界面实例。
        /// </summary>
        public override bool IsAllowMutiple => true;

        /// <summary>
        /// 屏幕适配器类型。
        /// </summary>
        public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.None;
    }
}
