using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        private CancellationTokenSource m_CancellationTokenSource;

        public override void OnEnter()
        {
            m_CancellationTokenSource = new CancellationTokenSource();

            // 打开登录界面。
            UIModule.Instance.OpenUIForm(UIFormId.LoginForm, m_CancellationTokenSource.Token).Forget();
        }

        public override void OnLeave()
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (LoginModule.Instance.LoginState == LoginStateType.OnGame)
            {
                ChangeNextProcedure();
            }
        }
    }
}

