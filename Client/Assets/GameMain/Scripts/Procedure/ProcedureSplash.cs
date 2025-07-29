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
            var enablePreloadPackageMode = GameConfig.Instance.EnablePreloadPackageMode;
            var dataTableLoadMode = GameConfig.Instance.DataTableLoadMode;
            
#if UNITY_WEBGL
            // WEBGL强制指定使用异步加载配置表模式。
            dataTableLoadMode = DataTableLoadMode.AsyncLoad;
            // WEBGL强制开启预加载包模式。
            enablePreloadPackageMode = true;
#endif
            // 设置配置表加载模式。
            DataTableModule.Instance.LoadMode = dataTableLoadMode;

            // 注册逻辑程序集。
            AssemblyUtility.RegisterRuntimeAssembly(GetType().Assembly);

            await InitYooAssetAsync();
            await ResModule.Instance.RequestUpdatePackageManifest();

            SoundUtility.InitSoundGroup();
            // NetworkModule.Instance.CreateDefaultNetworkChannel(NetworkConst.GatewayChannelName, ServiceType.Tcp, new OrleansNetworkChannelHelper());

            await UIModule.Instance.UseFairyGUI(new FairyGUISettings());
            UIModule.Instance.SetPreloadPackageMode(enablePreloadPackageMode);
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
            if (GameConfig.Instance.EnableEditorSimulateMode)
            {
                return ResModule.Instance.InitYooAssetEditorSimulateMode();
            }
#endif
            var mainCDNUrl = GameConfig.Instance.CDNUrl;
            var fallbackCDNUrl = GameConfig.Instance.CDNFallbackUrl;

            if (string.IsNullOrEmpty(fallbackCDNUrl))
            {
                fallbackCDNUrl = mainCDNUrl;
            }

            if (GameConfig.Instance.EnableCDNVersionPath)
            {
                mainCDNUrl = $"{mainCDNUrl}/{Application.version}";
                fallbackCDNUrl = $"{fallbackCDNUrl}/{Application.version}";
            }

#if UNITY_WEBGL
            return ResModule.Instance.InitYooAssetWebGLMode(mainCDNUrl, fallbackCDNUrl);
#else
            return ResModule.Instance.InitYooAssetOfflineMode();
#endif
        }
    }
}
