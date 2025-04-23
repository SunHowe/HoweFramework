namespace HoweFramework
{
    /// <summary>
    /// 引用接口。
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// 引用实例id。由ReferencePool管理。
        /// </summary>
        int ReferenceId { get; set; }

        /// <summary>
        /// 清理引用。
        /// </summary>
        void Clear();
    }
}
