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
        /// 固定界面不受栈显隐控制。
        /// </summary>
        public override bool IsAllowControlVisibleByFramework => false;

        /// <summary>
        /// 固定界面不受主界面打开时的批量关闭影响。
        /// </summary>
        public override bool IsAllowControlCloseByFramework => false;

        /// <summary>
        /// 屏幕适配器类型。
        /// </summary>
        public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.None;
    }
}
