using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using ServerProtocol;

namespace GatewayServer;

class Program
{
    static async Task Main(string[] args)
    {
        // 启动Orleans客户端.
        using var host = Host.CreateDefaultBuilder(args)
            .UseOrleansClient((context, client) =>
            {
                client.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "ClusterId";
                        options.ServiceId = "ServiceId";
                    })
                    .UseLocalhostClustering()
                    .AddMemoryStreams(StreamingProviderConst.ProviderName);
            })
            .UseConsoleLifetime()	// 配置主机在控制台应用程序中运行，并在关闭时释放资源。
            .Build();

        await host.StartAsync();

        // 获取Orleans客户端实例.
        var clusterClient = host.Services.GetRequiredService<IClusterClient>();

        var gatewayServer = new GatewayServer(clusterClient, IPAddress.Any, 9000);
        gatewayServer.Start();
        
        Console.WriteLine($"Gateway Server started, listening on port {gatewayServer.Port}");
        Console.ReadLine();
        
        gatewayServer.Stop();
        await host.StopAsync();
    }
}