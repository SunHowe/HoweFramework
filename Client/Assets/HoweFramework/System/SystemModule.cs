using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 系统模块。用于管理游戏系统。
    /// </summary>
    public sealed class SystemModule : ModuleBase<SystemModule>
    {
        /// <summary>
        /// 系统销毁事件。
        /// </summary>
        public event Action<ISystem> OnSystemDestroyed;

        /// <summary>
        /// 系统列表。
        /// </summary>
        private readonly List<ISystem> m_Systems = new List<ISystem>();

        /// <summary>
        /// 系统字典。
        /// </summary>
        private readonly Dictionary<Type, ISystem> m_SystemCache = new Dictionary<Type, ISystem>();

        /// <summary>
        /// 获取系统。
        /// </summary>
        /// <typeparam name="T">系统类型。</typeparam>
        /// <returns>系统。</returns>
        public T GetSystem<T>() where T : ISystem
        {
            if (m_SystemCache.TryGetValue(typeof(T), out var system))
            {
                return (T)system;
            }

            return default;
        }

        /// <summary>
        /// 注册系统。
        /// </summary>
        /// <typeparam name="T">系统类型。</typeparam>
        public T RegisterSystem<T>() where T : ISystem, new()
        {
            var system = new T();
            RegisterSystem(system);
            return system;
        }

        /// <summary>
        /// 注册系统。
        /// </summary>
        /// <typeparam name="TInterface">系统接口类型。</typeparam>
        /// <typeparam name="TSystem">系统类型。</typeparam>
        /// <returns>系统。</returns>
        public TInterface RegisterSystem<TInterface, TSystem>() where TInterface : ISystem where TSystem : TInterface, new()
        {
            var system = new TSystem();
            RegisterSystem<TInterface>(system);
            return system;
        }

        /// <summary>
        /// 注册系统。
        /// </summary>
        /// <typeparam name="T">系统类型。</typeparam>
        /// <param name="system">系统。</param>
        public void RegisterSystem<T>(T system) where T : ISystem
        {
            var systemType = typeof(T);
            if (m_SystemCache.ContainsKey(systemType))
            {
                throw new ErrorCodeException(FrameworkErrorCode.FrameworkException, $"系统 {systemType.Name} 已注册。");
            }

            system.Init();

            m_SystemCache.Add(systemType, system);
            m_Systems.Add(system);
        }

        /// <summary>
        /// 销毁系统。
        /// </summary>
        /// <typeparam name="T">系统类型。</typeparam>
        public void DestroySystem<T>() where T : ISystem
        {
            var systemType = typeof(T);
            if (!m_SystemCache.TryGetValue(systemType, out var system))
            {
                throw new ErrorCodeException(FrameworkErrorCode.FrameworkException, $"系统 {systemType.Name} 未注册。");
            }

            m_SystemCache.Remove(systemType);
            m_Systems.Remove(system);

            OnSystemDestroyed?.Invoke(system);

            system.Destroy();
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            // 逆序销毁：后注册的系统通常依赖先注册的系统。单个系统销毁异常不中断销毁链。
            for (int i = m_Systems.Count - 1; i >= 0; i--)
            {
                var system = m_Systems[i];
                try
                {
                    OnSystemDestroyed?.Invoke(system);
                    system.Destroy();
                }
                catch (Exception e)
                {
                    Log.Error($"销毁系统 {system.GetType().Name} 时发生异常：{e.Message}\n{e.StackTrace}");
                }
            }

            m_Systems.Clear();
            m_SystemCache.Clear();
            OnSystemDestroyed = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
