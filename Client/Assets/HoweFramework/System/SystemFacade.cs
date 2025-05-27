namespace HoweFramework
{
    /// <summary>
    /// 系统门面。
    /// </summary>
    public abstract class SystemFacade<T> where T : ISystem
    {
        private static ISystem m_System;
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    Init();
                }

                return m_Instance;
            }
        }

        private static void Init()
        {
            m_Instance = SystemModule.Instance.GetSystem<T>();

            if (m_Instance == null)
            {
                return;
            }
            
            m_System = m_Instance;

            SystemModule.Instance.OnSystemDestroyed += OnSystemDestroyed;
        }

        private static void OnSystemDestroyed(ISystem system)
        {
            if (system != m_System)
            {
                return;
            }

            m_Instance = default;
            m_System = default;

            SystemModule.Instance.OnSystemDestroyed -= OnSystemDestroyed;
        }
    }
}
