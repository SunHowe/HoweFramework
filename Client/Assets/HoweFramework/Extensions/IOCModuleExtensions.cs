namespace HoweFramework
{
    /// <summary>
    /// IOC模块扩展。
    /// </summary>
    public static class IOCModuleExtensions
    {
        /// <summary>
        /// 注入对象。
        /// </summary>
        /// <param name="obj">要注入的对象。</param>
        public static void InjectThis(this object obj)
        {
            IOCModule.Instance.Inject(obj);
        }
    }
}