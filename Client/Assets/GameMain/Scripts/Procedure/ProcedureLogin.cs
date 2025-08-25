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

        protected override void OnEnter()
        {
            // 打开登录场景。
            SceneModule.Instance.LoadSceneAsync(LoginSceneAssetName).Forget();
        }

        protected override void OnLeave()
        {
            // 卸载登录场景。
            SceneModule.Instance.UnloadSceneAsync(LoginSceneAssetName).Forget();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}

