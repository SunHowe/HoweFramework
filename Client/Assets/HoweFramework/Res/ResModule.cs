namespace HoweFramework
{
    /// <summary>
    /// 资源模块。
    /// </summary>
    public sealed class ResModule : ModuleBase<ResModule>
    {
        /// <summary>
        /// 资源加载器。
        /// </summary>
        private IResLoader m_ResLoader;

        /// <summary>
        /// 设置核心资源加载器。
        /// </summary>
        /// <param name="resLoader">核心资源加载器。</param>
        public void SetResCoreLoader(IResLoader resLoader)
        {
            m_ResLoader = resLoader;
        }

        /// <summary>
        /// 创建资源加载器。
        /// </summary>
        /// <returns>资源加载器。</returns>
        public IResLoader CreateResLoader()
        {
            if (m_ResLoader == null)
            {
                throw new ErrorCodeException(ErrorCode.ResCoreLoaderNotSet);
            }

            return ResProxyLoader.Create(m_ResLoader);
        }

        /// <summary>
        /// 获取核心资源加载器。
        /// </summary>
        /// <returns>核心资源加载器。</returns>
        internal IResLoader GetResCoreLoader()
        {
            return m_ResLoader;
        }

        /// <summary>
        /// 卸载未使用的资源。
        /// </summary>
        public void UnloadUnusedAsset()
        {
            m_ResLoader?.UnloadUnusedAsset();
        }

        protected override void OnDestroy()
        {
            m_ResLoader?.Dispose();
            m_ResLoader = null;
        }

        protected override void OnInit()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
