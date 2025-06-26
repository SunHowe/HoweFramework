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

        protected override void OnEnter()
        {
            // 在这里进行一些必要的初始化流程。
            m_IsInited = false;
            InitAsync().Forget();
        }

        protected override void OnLeave()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_IsInited)
            {
                ChangeNextProcedure();
            }
        }

        private async UniTask InitAsync()
        {
            // 注册逻辑程序集。
            AssemblyUtility.RegisterRuntimeAssembly(GetType().Assembly);

#if UNITY_EDITOR
            await ResModule.Instance.InitYooAssetEditorSimulateMode();
#else
            await ResModule.Instance.InitYooAssetOfflineMode();
#endif
            await ResModule.Instance.RequestUpdatePackageManifest();

            SoundUtility.InitSoundGroup();
            NetworkModule.Instance.CreateDefaultNetworkChannel(NetworkConst.GatewayChannelName, ServiceType.Tcp, new OrleansNetworkChannelHelper())
                .MappingPacketHandler(typeof(OrleansNetworkChannelHelper).Assembly);

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
