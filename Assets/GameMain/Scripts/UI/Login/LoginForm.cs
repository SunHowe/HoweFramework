using System;
using FairyGUI;
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
            m_LoginComp.LoginButton.onClick.Add(OnLoginButtonClick);
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
        /// 登录按钮点击回调。
        /// </summary>
        private void OnLoginButtonClick(EventContext context)
        {
            Request?.SetResponse(CommonResponse.Create(ErrorCode.Success, "Test"));
        }
    }
}