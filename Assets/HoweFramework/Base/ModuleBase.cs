namespace HoweFramework
{
    /// <summary>
    /// 模块基类。
    /// </summary>
    public abstract class ModuleBase
    {
        /// <summary>
        /// 初始化模块。
        /// </summary>
        internal abstract void Init();

        /// <summary>
        /// 销毁模块。
        /// </summary>
        internal abstract void Destroy();

        /// <summary>
        /// 逻辑帧更新。
        /// </summary>
        /// <param name="elapseSeconds">逻辑帧间隔。</param>
        /// <param name="realElapseSeconds">真实间隔。</param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);
    }

    /// <summary>
    /// 模块基类。
    /// </summary>
    public abstract class ModuleBase<T> : ModuleBase where T : ModuleBase<T>
    {
        public static T Instance { get; private set; }

        /// <summary>
        /// 初始化模块。
        /// </summary>
        internal sealed override void Init()
        {
            if (Instance != null)
            {
                throw new ErrorCodeException(ErrorCode.FrameworkException, $"{typeof(T)} 模块已初始化。");
            }

            Instance = (T)this;
            OnInit();
        }

        /// <summary>
        /// 销毁模块。
        /// </summary>
        internal sealed override void Destroy()
        {
            if (Instance != this)
            {
                throw new ErrorCodeException(ErrorCode.FrameworkException, $"{typeof(T)} 模块未初始化。");
            }

            OnDestroy();
            Instance = null;
        }

        /// <summary>
        /// 逻辑帧更新。
        /// </summary>
        /// <param name="elapseSeconds">逻辑帧间隔。</param>
        /// <param name="realElapseSeconds">真实间隔。</param>
        internal sealed override void Update(float elapseSeconds, float realElapseSeconds)
        {
            OnUpdate(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 模块初始化回调。
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// 模块销毁回调。
        /// </summary>
        protected abstract void OnDestroy();

        /// <summary>
        /// 逻辑帧更新回调。
        /// </summary>
        protected abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);
    }
}