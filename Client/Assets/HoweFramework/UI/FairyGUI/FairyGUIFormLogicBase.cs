using FairyGUI;

namespace HoweFramework
{
    /// <summary>
    /// FairyGUI界面逻辑基类。
    /// </summary>
    public abstract class FairyGUIFormLogicBase : IUIFormLogic
    {
        /// <summary>
        /// 界面接口实例。
        /// </summary>
        public IUIForm UIForm { get; private set; }

        /// <summary>
        /// UI界面内容根节点。
        /// </summary>
        public GComponent ContentPane { get; private set; }

        /// <summary>
        /// UI界面编号。
        /// </summary>
        public abstract int FormId { get; }

        /// <summary>
        /// UI界面所属的界面组编号。
        /// </summary>
        public abstract int FormGroupId { get; }

        /// <summary>
        /// UI界面类型。
        /// </summary>
        public abstract UIFormType FormType { get; }

        /// <summary>
        /// 是否允许同时打开多个界面实例。
        /// </summary>
        public abstract bool IsAllowMutiple { get; }

        /// <summary>
        /// 是否允许由框架控制界面显示与隐藏。
        /// </summary>
        public virtual bool IsAllowControlVisibleByFramework => true;

        /// <summary>
        /// 是否允许由框架控制界面关闭。
        /// </summary>
        public virtual bool IsAllowControlCloseByFramework => true;

        /// <summary>
        /// 当界面关闭时，返回的错误码。
        /// </summary>
        public virtual int ErrorCodeOnClose => ErrorCode.UIFormClosed;

        /// <summary>
        /// 屏幕适配器类型。
        /// </summary>
        public abstract FairyGUIScreenAdaptorType ScreenAdaptorType { get; }

        /// <summary>
        /// 可释放对象组。
        /// </summary>
        public DisposableGroup DisposableGroup { get; } = new DisposableGroup();

        /// <summary>
        /// 当前处理的打开请求。
        /// </summary>
        public OpenFormRequest Request => UIForm.Request;

        /// <summary>
        /// 当前处理的打开请求的业务透传数据。
        /// </summary>
        public object RequestUserData => Request?.UserData;

        /// <summary>
        /// 界面框架组件。
        /// </summary>
        public GComponent Frame
        {
            get => m_Frame;
            protected set
            {
                m_Frame = value;

                if (m_Frame != null)
                {
                    CloseButton = m_Frame.GetChild("closeButton");
                    BackButton = m_Frame.GetChild("backButton");
                }
                else
                {
                    CloseButton = null;
                    BackButton = null;
                }
            }
        }

        /// <summary>
        /// 界面的关闭按钮 当它被点击时会调用OnCloseButtonClick方法 约定框架组件下名为closeButton的组件为默认的关闭按钮
        /// </summary>
        public GObject CloseButton
        {
            get => m_CloseButton;
            set
            {
                if (m_CloseButton == value)
                    return;
                
                if (m_CloseButton != null)
                    m_CloseButton.onClick.Remove(OnCloseButtonClick);

                m_CloseButton = value;
                
                if (m_CloseButton != null)
                    m_CloseButton.onClick.Add(OnCloseButtonClick);
            }
        }

        /// <summary>
        /// 界面的返回按钮 当它被点击时会调用OnBackButtonClick方法 约定框架组件下名为backButton的组件为默认的返回按钮
        /// </summary>
        public GObject BackButton
        {
            get => m_BackButton;
            set
            {
                if (m_BackButton == value)
                    return;
                
                if (m_BackButton != null)
                    m_BackButton.onClick.Remove(OnBackButtonClick);

                m_BackButton = value;
                
                if (m_BackButton != null)
                    m_BackButton.onClick.Add(OnBackButtonClick);
            }
        }

        private GComponent m_Frame;
        private GObject m_CloseButton;
        private GObject m_BackButton;

        /// <summary>
        /// 界面初始化回调。
        /// </summary>
        /// <param name="uiForm">界面接口实例。</param>
        public void OnInit(IUIForm uiForm)
        {
            UIForm = uiForm;
            ContentPane = (GComponent)uiForm.FormInstance;
            Frame = ContentPane.GetChild("frame") as GComponent;

            OnInit();
            OnInitScreenAdpater();
        }

        /// <summary>
        /// 界面销毁回调。
        /// </summary>
        public virtual void OnDestroy()
        {
            DisposableGroup.Dispose();
            Frame = null;
        }

        /// <summary>
        /// 界面初始化回调。
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// 界面打开回调。
        /// </summary>
        public abstract void OnOpen();

        /// <summary>
        /// 界面关闭回调。
        /// </summary>
        public abstract void OnClose();

        /// <summary>
        /// 界面更新回调。
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// 界面显示回调。
        /// </summary>
        public abstract void OnVisible();

        /// <summary>
        /// 界面隐藏回调。
        /// </summary>
        public abstract void OnInvisible();

        /// <summary>
        /// 关闭按钮点击回调。
        /// </summary>
        protected virtual void OnCloseButtonClick()
        {
            UIForm.CloseForm();
        }

        /// <summary>
        /// 返回按钮点击回调。默认逻辑与关闭按钮相同。
        /// </summary>
        protected virtual void OnBackButtonClick()
        {
            OnCloseButtonClick();
        }

        /// <summary>
        /// 初始化屏幕适配器。
        /// </summary>
        private void OnInitScreenAdpater()
        {
            switch (ScreenAdaptorType)
            {
                case FairyGUIScreenAdaptorType.ConstantHorizontalCenter:
                    DisposableGroup.Add(FairyGUIConstantAdaptor.Create(ContentPane, isHorizontalCenter: true));
                    break;
                case FairyGUIScreenAdaptorType.ConstantVerticalCenter:
                    DisposableGroup.Add(FairyGUIConstantAdaptor.Create(ContentPane, isVerticalCenter: true));
                    break;
                case FairyGUIScreenAdaptorType.ConstantCenter:
                    DisposableGroup.Add(FairyGUIConstantAdaptor.Create(ContentPane, true, true));
                    break;
                case FairyGUIScreenAdaptorType.FullScreen:
                    DisposableGroup.Add(FairyGUIFullScreenAdaptor.Create(ContentPane));
                    break;
                case FairyGUIScreenAdaptorType.SafeAreaFullScreen:
                    DisposableGroup.Add(FairyGUISafeAreaAdaptor.Create(ContentPane));
                    break;
            }
        }
    }
}

