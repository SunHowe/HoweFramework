using Cysharp.Threading.Tasks;
using GameMain.UI;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 闪屏流程。
    /// </summary>
    public sealed class ProcedureSplash : ProcedureBase
    {
        public override int Id => (int)ProcedureId.Splash;

        private bool m_IsInited = false;

        public override void OnEnter()
        {
            // 在这里进行一些必要的初始化流程。
            m_IsInited = false;
            InitAsync().Forget();
        }

        public override void OnLeave()
        {
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_IsInited)
            {
                ChangeNextProcedure();
            }
        }

        private async UniTask InitAsync()
        {
            await ResModule.Instance.InitYooAssetEditorSimulateMode();
            await ResModule.Instance.RequestUpdatePackageManifest();

            SoundUtility.InitSoundGroup();
            NetworkModule.Instance.CreateDefaultNetworkChannel(NetworkConst.GatewayChannelName, ServiceType.Tcp, new OrleansNetworkChannelHelper());

            await UIModule.Instance.UseFairyGUI(new FairyGUISettings());
            await UIModule.Instance.LoadFairyGUIPackagesAsync(UIConst.UIPackageMappingAssetPath);
            UIModule.Instance.AddFairyGUIFormBindings(UIFormBindings.Bindings);
            UIModule.Instance.AddFairyGUIComponentBindings(UIComponentBindings.Bindings);

            for (UIGroupId i = 0; i < UIGroupId.Count; i++)
            {
                UIModule.Instance.CreateUIFormGroup((int)i, i.ToString());
            }

            m_IsInited = true;
        }
    }
}
