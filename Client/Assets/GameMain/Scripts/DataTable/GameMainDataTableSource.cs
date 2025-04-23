using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HoweFramework;
using Luban;

namespace GameMain
{
    /// <summary>
    /// 游戏主数据表数据源。
    /// </summary>
    public partial class GameMainDataTableSource
    {
        private const string DATA_TABLE_ASSET_PATH = "Assets/GameMain/DataTable/{0}.bytes";

        private readonly IResLoader m_ResLoader;

        public GameMainDataTableSource()
        {
            m_ResLoader = ResModule.Instance.CreateResLoader();
        }

        public void Dispose()
        {
            foreach (var dataTable in m_DataTableList)
            {
                dataTable.Dispose();
            }

            m_DataTableList.Clear();
            m_PreloadDataTableList.Clear();

            m_ResLoader.Dispose();
        }
        
        /// <summary>
        /// 异步加载指定配置表数据。
        /// </summary>
        private UniTask<ByteBuf> LoadDataTableAsync(string dataTableName)
        {
            return m_ResLoader.LoadBinaryAsync(string.Format(DATA_TABLE_ASSET_PATH, dataTableName))
                .ContinueWith(bytes => new ByteBuf(bytes));
        }

        /// <summary>
        /// 同步加载指定配置表数据。
        /// </summary>
        private ByteBuf LoadDataTable(string dataTableName)
        {
            return new ByteBuf(m_ResLoader.LoadBinary(string.Format(DATA_TABLE_ASSET_PATH, dataTableName)));
        }

        /// <summary>
        /// 获取需要预加载的配置表。
        /// </summary>
        /// <returns>需要预加载的配置表。</returns>
        private List<IDataTable> GetPreloadDataTables()
        {
            return new List<IDataTable>()
            {
            };
        }
    }
}
