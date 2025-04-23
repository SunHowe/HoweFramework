namespace HoweFramework
{
    /// <summary>
    /// UI界面接口。
    /// </summary>
    public interface IUIForm
    {
        /// <summary>
        /// UI界面编号。
        /// </summary>
        int FormId { get; }

        /// <summary>
        /// UI界面所属的界面组编号。
        /// </summary>
        int FormGroupId { get; }

        /// <summary>
        /// 界面类型。
        /// </summary>
        UIFormType FormType { get; }

        /// <summary>
        /// 是否允许同时打开多个界面实例。
        /// </summary>
        bool IsAllowMutiple { get; }

        /// <summary>
        /// 是否允许由框架控制界面显示与隐藏。
        /// </summary>
        bool IsAllowControlVisibleByFramework { get; }

        /// <summary>
        /// 是否允许由框架控制界面关闭。
        /// </summary>
        bool IsAllowControlCloseByFramework { get; }

        /// <summary>
        /// UI界面序列编号，每次打开界面时，序列编号会自动增加。
        /// </summary>
        int FormSerialId { get; }

        /// <summary>
        /// UI界面所属的界面组。
        /// </summary>
        IUIFormGroup FormGroup { get; }

        /// <summary>
        /// 界面实例。
        /// </summary>
        object FormInstance { get; }

        /// <summary>
        /// 是否已加载。
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// 是否已打开。
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// 是否可见。
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// 当前的打开界面请求。
        /// </summary>
        OpenFormRequest Request { get; }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        void CloseForm();
    }
}
