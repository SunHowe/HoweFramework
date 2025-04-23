using GameMain.UI;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 登录流程。
    /// </summary>
    public sealed class ProcedureLogin : ProcedureBase
    {
        public override int Id => (int)ProcedureId.Login;

        public override void OnEnter()
        {
            // 打开登录界面。
            UIModule.Instance.OpenUIForm(UIFormId.LoginForm);
        }

        public override void OnLeave()
        {
            // 关闭登录界面。
            UIModule.Instance.CloseUIForm(UIFormId.LoginForm);
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}

