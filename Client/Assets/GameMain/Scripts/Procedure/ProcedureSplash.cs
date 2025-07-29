using Cysharp.Threading.Tasks;
using GameMain.UI;
using HoweFramework;
using UnityEngine;

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

            await InitYooAssetAsync();
            await ResModule.Instance.RequestUpdatePackageManifest();

            SoundUtility.InitSoundGroup();
            // NetworkModule.Instance.CreateDefaultNetworkChannel(NetworkConst.GatewayChannelName, ServiceType.Tcp, new OrleansNetworkChannelHelper());

            await UIModule.Instance.UseFairyGUI(new FairyGUISettings());
            
#if UNITY_WEBGL
            // WEBGL强制指定使用异步加载配置表模式。
            DataTableModule.Instance.LoadMode = DataTableLoadMode.AsyncLoad;
            // WEBGL强制开启预加载包模式。
            UIModule.Instance.SetPreloadPackageMode(true);
#endif

            await UIModule.Instance.LoadFairyGUIPackagesAsync(UIConst.UIPackageMappingAssetPath);
            UIModule.Instance.AddFairyGUIFormBindings(UIFormBindings.Bindings);
            UIModule.Instance.AddFairyGUIComponentBindings(UIComponentBindings.Bindings);

            for (UIGroupId i = 0; i < UIGroupId.Count; i++)
            {
                UIModule.Instance.CreateUIFormGroup((int)i, i.ToString());
            }

            m_IsInited = true;
        }

        /// <summary>
        /// 初始化YooAsset资源管线。
        /// </summary>
        private UniTask InitYooAssetAsync()
        {
#if UNITY_EDITOR
            if (PlayerPrefs.GetInt("SimulateAssetBundle", 0) == 0)
            {
                return ResModule.Instance.InitYooAssetEditorSimulateMode();
            }
#endif

#if UNITY_WEBGL
            return ResModule.Instance.InitYooAssetWebGLMode("http://localhost:8080/CDN", "http://localhost:8080/CDN");
#else
            return ResModule.Instance.InitYooAssetOfflineMode();
#endif
        }
    }
}
