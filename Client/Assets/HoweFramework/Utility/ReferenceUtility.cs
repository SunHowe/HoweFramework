namespace HoweFramework
{
    /// <summary>
    /// 实例工具。
    /// </summary>
    public static class ReferenceUtility
    {
        /// <summary>
        /// 获取实例引用。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <param name="reference">实例。</param>
        /// <returns>实例引用。</returns>
        public static ReferenceRef<T> AsRef<T>(this T reference) where T : class, IReferenceWithId
        {
            return new ReferenceRef<T>(reference);
        }
    }
}