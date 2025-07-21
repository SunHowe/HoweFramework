namespace HoweFramework
{
    /// <summary>
    /// 提供由框架自动分配的实例唯一标识的引用接口。
    /// </summary>
    public interface IReferenceWithId : IReference
    {
        /// <summary>
        /// 实例唯一标识。由框架自动分配。
        /// </summary>
        int InstanceId { get; set; }
    }
}