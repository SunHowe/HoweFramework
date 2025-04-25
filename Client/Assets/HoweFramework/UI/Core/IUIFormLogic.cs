namespace HoweFramework
{
    /// <summary>
    /// 界面逻辑接口。
    /// </summary>
    public interface IUIFormLogic
    {
        /// <summary>
        /// 界面接口实例。
        /// </summary>
        IUIForm UIForm { get; }

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
        /// 当界面关闭时，返回的错误码。
        /// </summary>
        int ErrorCodeOnClose { get; }

        /// <summary>
        /// 界面初始化回调。
        /// </summary>
        /// <param name="uiForm">界面接口实例。</param>
        void OnInit(IUIForm uiForm);

        /// <summary>
        /// 界面销毁回调。
        /// </summary>
        void OnDestroy();

        /// <summary>
        /// 界面打开回调。
        /// </summary>
        void OnOpen();

        /// <summary>
        /// 界面更新回调。
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// 界面关闭回调。
        /// </summary>
        void OnClose();

        /// <summary>
        /// 界面显示回调。
        /// </summary>
        void OnVisible();

        /// <summary>
        /// 界面不可见回调。
        /// </summary>
        void OnInvisible();
    }
}

