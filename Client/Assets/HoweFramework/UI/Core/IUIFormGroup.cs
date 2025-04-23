namespace HoweFramework
{
    /// <summary>
    /// UI界面组接口。
    /// </summary>
    public interface IUIFormGroup
    {
        /// <summary>
        /// 界面组编号。
        /// </summary>
        int GroupId { get; }

        /// <summary>
        /// 界面组名称。
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// 界面组实例。
        /// </summary>
        object GroupInstance { get; }
    }
}
