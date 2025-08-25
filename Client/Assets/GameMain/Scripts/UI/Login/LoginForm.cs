using HoweFramework;

namespace GameMain.UI.Login
{
    /// <summary>
    /// LoginForm逻辑实现。
    /// </summary>
    public partial class LoginForm : FullScreenFormLogicBase
    {
        /// <summary>
        /// 界面初始化回调。
        /// </summary>
        private void OnInitialize()
        {
            this.m_BtnNewGame.onClick.Add(OnBtnNewGameClick);
        }

        /// <summary>
        /// 界面销毁回调。
        /// </summary>
        private void OnDispose()
        {
            this.m_BtnNewGame.onClick.Remove(OnBtnNewGameClick);
        }

        /// <summary>
        /// 界面打开回调。
        /// </summary>
        public override void OnOpen()
        {
        }

        /// <summary>
        /// 界面关闭回调。
        /// </summary>
        public override void OnClose()
        {
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
        /// 新游戏按钮点击回调。
        /// </summary>
        private void OnBtnNewGameClick()
        {
            EventModule.Instance.Dispatch(this, NewGameRequestEventArgs.Create());
        }
    }
}