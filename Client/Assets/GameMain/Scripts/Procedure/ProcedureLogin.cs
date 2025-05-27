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

        public override void OnEnter()
        {
            // 打开登录场景。
            SceneModule.Instance.LoadSceneAsync(LoginSceneAssetName).Forget();

            // 打开登录界面。
            UIModule.Instance.OpenUIForm(UIFormId.LoginForm).Forget();
        }

        public override void OnLeave()
        {
            // 关闭登录界面。
            UIModule.Instance.CloseUIForm(UIFormId.LoginForm).Forget();

            // 卸载登录场景。
            SceneModule.Instance.UnloadSceneAsync(LoginSceneAssetName).Forget();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}

