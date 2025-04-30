
using IGrains;

namespace Grains;

/// <summary>
/// 游戏服务器启动任务。
/// </summary>
public class GameServerStarupTask : IStartupTask
{
    private readonly IClusterClient m_ClusterClient;

    public GameServerStarupTask(IClusterClient clusterClient)
    {
        m_ClusterClient = clusterClient;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        ProtocolHandlerManager.Init();
    }
}

