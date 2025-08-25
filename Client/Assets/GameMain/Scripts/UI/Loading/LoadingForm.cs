using HoweFramework;

namespace GameMain.UI.Loading
{
    /// <summary>
    /// LoadingForm逻辑实现。
    /// </summary>
    public partial class LoadingForm : FullScreenFormLogicBase
    {
        /// <summary>
        /// 界面组编号。
        /// </summary>
        public override int FormGroupId => (int)UIGroupId.Loading;

        /// <summary>
        /// 界面类型。
        /// </summary>
        public override UIFormType FormType => UIFormType.Fixed;

        /// <summary>
        /// 界面初始化回调。
        /// </summary>
        private void OnInitialize()
        {
        }

        /// <summary>
        /// 界面销毁回调。
        /// </summary>
        private void OnDispose()
        {
        }

        /// <summary>
        /// 界面打开回调。
        /// </summary>
        public override void OnOpen()
        {
            m_Progress.value = 0f;

            EventModule.Instance.Subscribe(LoadingProgressUpdateEventArgs.EventId, OnLoadingProgressUpdate);
        }

        /// <summary>
        /// 界面关闭回调。
        /// </summary>
        public override void OnClose()
        {
            EventModule.Instance.Unsubscribe(LoadingProgressUpdateEventArgs.EventId, OnLoadingProgressUpdate);
        }

        /// <summary>
        /// 界面更新回调(打开时也会触发)。
        /// </summary>
        public override void OnUpdate()
        {
        }

        /// <summary>
        /// 界面显示回调。
        /// </summary>
        public override void OnVisible()
        {
        }

        /// <summary>
        /// 界面隐藏回调。
        /// </summary>
        public override void OnInvisible()
        {
        }

        /// <summary>
        /// 加载进度更新事件。
        /// </summary>
        private void OnLoadingProgressUpdate(object sender, GameEventArgs e)
        {
            if (e is not LoadingProgressUpdateEventArgs eventArgs)
            {
                return;
            }

            m_Progress.value = eventArgs.Progress;
        }
    }
}