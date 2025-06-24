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
        private const string LoginSceneAssetName = "Assets/GameMain/Scene/Login.unity";
        public override int Id => (int)ProcedureId.Login;

        private CancellationTokenSource m_CancellationTokenSource;

        protected override void OnEnter()
        {
            m_CancellationTokenSource = new CancellationTokenSource();
            
            // 打开登录场景。
            SceneModule.Instance.LoadSceneAsync(LoginSceneAssetName).Forget();

            // 打开登录界面。
            UIModule.Instance.OpenUIForm(UIFormId.LoginForm, m_CancellationTokenSource.Token).Forget();
        }

        protected override void OnLeave()
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();

            // 卸载登录场景。
            SceneModule.Instance.UnloadSceneAsync(LoginSceneAssetName).Forget();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (LoginSystem.Instance.LoginState.Value == LoginStateType.OnGame)
            {
                ChangeNextProcedure();
            }
        }
    }
}

