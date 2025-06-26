namespace HoweFramework
{
    /// <summary>
    /// 行为树模块。
    /// </summary>
    public sealed class BehaviorModule : ModuleBase<BehaviorModule>
    {
        /// <summary>
        /// 全局行为树加载器。
        /// </summary>
        public IBehaviorLoader GlobalBehaviorLoader { get; private set; }

        /// <summary>
        /// 创建行为树加载器。
        /// </summary>
        /// <param name="resLoader">资源加载器。若为null，则会分配一个新的资源加载器。</param>
        /// <returns>行为树加载器。</returns>
        public IBehaviorLoader CreateBehaviorLoader(IResLoader resLoader = null)
        {
            return BehaviorLoader.Create(resLoader);
        }

        protected override void OnDestroy()
        {
            GlobalBehaviorLoader.Dispose();
            GlobalBehaviorLoader = null;
        }

        protected override void OnInit()
        {
            GlobalBehaviorLoader = CreateBehaviorLoader();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}