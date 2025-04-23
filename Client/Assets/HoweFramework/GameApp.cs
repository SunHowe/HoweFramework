using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 应用基类。
    /// </summary>
    public sealed class GameApp
    {
        /// <summary>
        /// 游戏应用初始化完成事件。
        /// </summary>
        public static event Action OnGameAppInited;

        /// <summary>
        /// 游戏应用销毁事件。
        /// </summary>
        public static event Action OnGameAppDestroyed;

        /// <summary>
        /// 应用实例。
        /// </summary>
        public static GameApp Instance { get; private set; }

        /// <summary>
        /// 模块列表。
        /// </summary>
        private readonly List<ModuleBase> m_ModuleList = new List<ModuleBase>();

        /// <summary>
        /// 创建应用实例。
        /// </summary>
        public static GameApp NewApp()
        {
            if (Instance != null)
            {
                Instance.Destroy();
            }

            Instance = new GameApp();
            OnGameAppInited?.Invoke();
            return Instance;
        }

        private GameApp()
        {
            AddModule<EventModule>(); // 事件模块。
            AddModule<TimerModule>(); // 计时器模块。
            AddModule<SafeAreaModule>(); // 安全区域模块。
            AddModule<ResModule>(); // 资源模块。
            AddModule<DataTableModule>(); // 配置表模块。
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
            foreach (var module in m_ModuleList)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 添加模块。
        /// </summary>
        private void AddModule<T>() where T : ModuleBase<T>
        {
            var module = Activator.CreateInstance<T>();
            module.Init();

            m_ModuleList.Add(module);
        }
    }
}

