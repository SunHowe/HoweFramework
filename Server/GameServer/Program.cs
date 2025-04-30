using DataTable;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                siloBuilder.ConfigureLogging(config =>
                {
                    config.SetMinimumLevel(LogLevel.Information);
                    config.ClearProviders();
                    config.AddConsole();
                    config.AddDebug();
                });
                siloBuilder.Services.AddSingleton<IDataTableService, DataTableService>();
                
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

                siloBuilder.AddStartupTask<GameServerStarupTask>();
            })
            .Build();

        var dataTableService = host.Services.GetService<IDataTableService>();

        await host.RunAsync();
    }
}