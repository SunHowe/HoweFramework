using FairyGUI;
using HoweFramework;

namespace GameMain.UI.Login
{
    /// <summary>
    /// LoginAccountForm逻辑实现。
    /// </summary>
    public partial class LoginAccountForm : PopupFormLogicBase
    {
        /// <summary>
        /// 界面初始化回调。
        /// </summary>
        private void OnInitialize()
        {
            this.m_BtnLogin.onClick.Add(OnLoginButtonClick);
            this.m_BtnCancel.onClick.Add(OnCancelButtonClick);
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

        private void OnLoginButtonClick(EventContext context)
        {
            var account = this.m_InputAccount.text;
            var password = this.m_InputPassword.text;

            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                Log.Error("账号或密码不能为空");
                return;
            }

            Request?.SetResponse(LoginAccountResponse.Create(account, password));
        }

        private void OnCancelButtonClick(EventContext context)
        {
            Request?.SetResponse(ErrorCode.RequestCanceled);
        }
    }
}