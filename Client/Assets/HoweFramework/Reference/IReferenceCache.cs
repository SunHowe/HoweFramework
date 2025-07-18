namespace HoweFramework
{
    /// <summary>
    /// 引用缓存接口。
    /// </summary>
    internal interface IReferenceCache
    {
        /// <summary>
        /// 缓存的引用数量。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 出队。若缓存为空，则创建一个新实例。
        /// </summary>
        /// <returns>引用。</returns>
        IReference Dequeue();

        /// <summary>
        /// 入队。
        /// </summary>
        /// <param name="reference">引用。</param>
        void Enqueue(IReference reference);

        /// <summary>
        /// 清空。
        /// </summary>
        void Clear();
    }
}