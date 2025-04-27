using Grains;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using ServerProtocol;

namespace GameServer;

internal static class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseOrleans((context, siloBuilder) =>
            {
                siloBuilder.UseLocalhostClustering(); // 本地开发集群
                siloBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "ClusterId";
                    options.ServiceId = "ServiceId";
                });
                
                siloBuilder.UseMongoDBClient("mongodb://localhost");
                siloBuilder.AddMongoDBGrainStorageAsDefault(options =>
                {
                    options.DatabaseName  = "OrleansDB";
                    options.CollectionPrefix = "Grains";
                });

                siloBuilder.AddMemoryStreams(StreamingProviderConst.ProviderName);
                siloBuilder.AddMemoryGrainStorage("PubSubStore");
            })
            .Build();

        await host.RunAsync();
    }
}