namespace HoweFramework
{
    /// <summary>
    /// 配置表加载模式。
    /// </summary>
    public enum DataTableLoadMode
    {
        /// <summary>
        /// 启动时异步加载配置表。
        /// </summary>
        AsyncLoad,
        
        /// <summary>
        /// 启动时同步加载配置表。
        /// </summary>
        SyncLoad,
        
        /// <summary>
        /// 懒加载模式。使用时同步加载配置表。
        /// </summary>
        LazyLoad,
        
        /// <summary>
        /// 在懒加载模式的基础上，使用异步的方式异步加载特定的配置表。
        /// </summary>
        LazyLoadAndPreloadAsync,
        
        /// <summary>
        /// 在懒加载模式的基础上，使用同步的方式同步加载特定的配置表。
        /// </summary>
        LazyLoadAndPreloadSync,
    }
}