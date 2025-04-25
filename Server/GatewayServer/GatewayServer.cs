using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace GatewayServer;

public class GatewayServer : TcpServer
{
    private readonly IClusterClient m_ClusterClient;
    
    public GatewayServer(IClusterClient clusterClient, IPAddress address, int port) : base(address, port)
    {
        m_ClusterClient = clusterClient;
        // OptionNoDelay = true; // 启用Nagle算法
        // OptionReuseAddress = true;
        // OptionSendBufferSize = 8192; // 8K发送缓冲区
        // OptionReceiveBufferSize = 8192; // 8K接收缓冲区
    }

    protected override TcpSession CreateSession()
    {
        return new GatewaySession(this, m_ClusterClient);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Gateway Server caught an error with code {error}");
    }
}