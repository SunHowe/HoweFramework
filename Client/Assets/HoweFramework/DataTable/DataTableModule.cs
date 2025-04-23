using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 配置表模块。
    /// </summary>
    public sealed class DataTableModule : ModuleBase<DataTableModule>
    {
        /// <summary>
        /// 配置表加载模式。
        /// </summary>
        public DataTableLoadMode LoadMode { get; set; } = DataTableLoadMode.LazyLoadAndPreloadAsync;

        /// <summary>
        /// 配置表数据源列表。
        /// </summary>
        private readonly List<IDataTableSource> m_DataTableSourceList = new();

        /// <summary>
        /// 添加配置表数据源。
        /// </summary>
        /// <param name="dataTableSource">配置表数据源。</param>
        public void AddDataTableSource(IDataTableSource dataTableSource)
        {
            m_DataTableSourceList.Add(dataTableSource);

            dataTableSource.Init(LoadMode);
        }

        /// <summary>
        /// 移除配置表数据源。
        /// </summary>
        /// <param name="dataTableSource">配置表数据源。</param>
        public void RemoveDataTableSource(IDataTableSource dataTableSource)
        {
            if (!m_DataTableSourceList.Remove(dataTableSource))
            {
                return;
            }

            dataTableSource.Dispose();
        }

        /// <summary>
        /// 使用同步的方式预加载配置表。
        /// </summary>
        public void PreLoad()
        {
            foreach (var dataTableSource in m_DataTableSourceList)
            {
                dataTableSource.PreLoad();
            }
        }

        /// <summary>
        /// 使用异步的方式预加载配置表。
        /// </summary>
        public async UniTask PreLoadAsync()
        {
            using var uniTaskList = ReusableList<UniTask>.Create();

            foreach (var dataTableSource in m_DataTableSourceList)
            {
                uniTaskList.Add(dataTableSource.PreLoadAsync());
            }
            
            await UniTask.WhenAll(uniTaskList);
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            foreach (var dataTableSource in m_DataTableSourceList)
            {
                dataTableSource.Dispose();
            }

            m_DataTableSourceList.Clear();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
