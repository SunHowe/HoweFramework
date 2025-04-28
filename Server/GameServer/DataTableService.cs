using DataTable;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServerProtocol;

namespace GameServer
{
    /// <summary>
    /// 配置表服务实现。
    /// </summary>
    public sealed class DataTableService : IDataTableService
    {
        private readonly IClusterClient m_ClusterClient;
        private readonly ILogger<DataTableService> m_Logger;
        private DataTableList m_DataTableList;

        public DataTableService(IClusterClient clusterClient, ILogger<DataTableService> logger)
        {
            m_ClusterClient = clusterClient;
            m_Logger = logger;

            LoadConfig(0);
        }

        public T Get<T>() where T : IDataTable
        {
            return (T)m_DataTableList.DataTableDict[typeof(T)];
        }

        public void Load(int version)
        {
            LoadConfig(version);

            // 通知配置表更新。
            var streamProvider = m_ClusterClient.GetStreamProvider(StreamingProviderConst.ProviderName);
            var streamId = StreamId.Create(StreamingProviderConst.StreamConfigUpdateEvent, 0);
            var stream = streamProvider.GetStream<int>(streamId);
            stream.OnNextAsync(version);
        }

        private void LoadConfig(int version)
        {
            var dataTableRoot = version > 0 ? $"DataTable_{version}" : "DataTable";
            m_DataTableList = new DataTableList(LoadFromFile);
            return;

            JArray LoadFromFile(string name)
            {
                var path = Path.Combine(dataTableRoot, name + ".json");
                try
                {
                    var json = File.ReadAllText(path);
                    return JArray.Parse(json);
                }
                catch (Exception e)
                {
                    m_Logger.LogError(e, "Load config file failed: {0}", path);
                    throw;
                }
            }
        }
    }
}