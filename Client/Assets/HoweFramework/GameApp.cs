using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace HoweFramework
{
    /// <summary>
    /// 应用基类。
    /// </summary>
    public sealed class GameApp
    {
        /// <summary>
        /// 应用实例。
        /// </summary>
        public static GameApp Instance { get; private set; }

        /// <summary>
        /// 游戏应用销毁事件。
        /// </summary>
        public event Action OnGameAppDestroyed;

        /// <summary>
        /// 模块列表。
        /// </summary>
        private readonly List<ModuleBase> m_ModuleList = new List<ModuleBase>();

        public GameApp()
        {
            if (Instance != null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameApp already exists.");
            }

            Instance = this;

            AddModule<BaseModule>()  // 基础模块。
                .UseUnityJsonHelper()
                .UseDefaultTextTemplateHelper();
            AddModule<EventModule>(); // 事件模块。
            AddModule<RemoteRequestModule>(); // 远程请求模块。
            AddModule<NetworkModule>(); // 网络模块。
            AddModule<WebRequestModule>().UseUnityWebRequest(); // Web请求模块。
            AddModule<TimerModule>(); // 计时器模块。
            AddModule<SettingModule>().UsePlayerPrefsSetting(); // 设置模块。
            AddModule<SafeAreaModule>(); // 安全区域模块。
            AddModule<ResModule>().UseYooAsset(); // 资源模块。
            AddModule<SceneModule>(); // 场景模块。
            AddModule<SoundModule>().UseAudioClipSound(); // 声音模块。
            AddModule<GameObjectPoolModule>(); // 游戏对象池模块。
            AddModule<DataTableModule>(); // 配置表模块。
            AddModule<LocalizationModule>(); // 本地化模块。
            AddModule<UIModule>(); // UI模块。
            AddModule<ProcedureModule>(); // 流程模块。
        }

        /// <summary>
        /// 销毁应用。
        /// </summary>
        public void Destroy()
        {
            // 销毁模块。
            for (int i = m_ModuleList.Count - 1; i >= 0; i--)
            {
                ModuleBase module = m_ModuleList[i];
                module.Destroy();
            }

            m_ModuleList.Clear();

            Instance = null;
            OnGameAppDestroyed?.Invoke();
        }

        /// <summary>
        /// 逻辑帧更新。
        /// </summary>
        /// <param name="elapseSeconds">逻辑帧间隔。</param>
        /// <param name="realElapseSeconds">真实间隔。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            var count = m_ModuleList.Count;
            for (int i = 0; i < count; i++)
            {
                ModuleBase module = m_ModuleList[i];
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 重启游戏。
        /// </summary>
        public void RestartGame()
        {
            // 重新加载主场景。
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// 添加模块。
        /// </summary>
        public T AddModule<T>() where T : ModuleBase<T>
        {
            var module = Activator.CreateInstance<T>();
            AddModule(module);
            return module;
        }

        /// <summary>
        /// 添加模块。
        /// </summary>
        /// <param name="module">模块。</param>
        public void AddModule(ModuleBase module)
        {
            module.Init();
            m_ModuleList.Add(module);
        }
    }
}

