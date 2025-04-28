namespace DataTable
{
    /// <summary>
    /// 配置表服务接口。
    /// </summary>
    public interface IDataTableService
    {
        /// <summary>
        /// 获取指定的配置包。
        /// </summary>
        T Get<T>() where T : IDataTable;

        /// <summary>
        /// 加载配置表。
        /// </summary>
        void Load(int version);
    }
}