using System.Collections.Generic;
using Luban;
using NLog;

namespace Geek.Server.Config
{
    /// <summary>
    /// 游戏主数据表数据源。
    /// </summary>
    public partial class GameDataManager
    {
        public static GameDataManager Instance { get; private set; }

        private GameDataManager()
        {
            Init(DataTableLoadMode.SyncLoad);
        }
        
        public void Dispose()
        {
            UnloadDataTable();
        }

        public static (bool, string) ReloadAll()
        {
            Instance ??= new GameDataManager();
            try
            {
                Instance.UnloadDataTable();
            }
            catch
            {
                // ignored
            }

            try
            {
                Instance.Preload();
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
            
            return (true, string.Empty);
        }

        private void UnloadDataTable()
        {
            foreach (var dataTable in m_DataTableList)
            {
                dataTable.Dispose();
            }

            m_DataTableList.Clear();
            m_PreloadDataTableList.Clear();
        }

        private string GetDataTableByteFilePath(string dataTableName)
        {
            var folder = Environment.CurrentDirectory;
            return $"{folder}/Bytes/{dataTableName}.bytes";
        }

        private byte[] LoadDataTableBytes(string dataTableName)
        {
            return File.ReadAllBytes(GetDataTableByteFilePath(dataTableName));
        }

        /// <summary>
        /// 异步加载指定配置表数据。
        /// </summary>
        private Task<ByteBuf> LoadDataTableAsync(string dataTableName)
        {
            throw new Exception("Server not support async load data table");
        }

        /// <summary>
        /// 同步加载指定配置表数据。
        /// </summary>
        private ByteBuf LoadDataTable(string dataTableName)
        {
            return new ByteBuf(LoadDataTableBytes(dataTableName));
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