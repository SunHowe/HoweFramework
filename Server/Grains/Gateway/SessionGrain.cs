using IGrains;
using Orleans.Streams;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 会话Grain, 用于游戏服务器发送消息给客户端.
/// </summary>
public class SessionGrain : Grain, ISessionGrain
{
    private IAsyncStream<ServerPackage> m_Stream;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        base.OnActivateAsync(cancellationToken);
        
        var streamProvider = this.GetStreamProvider(StreamingProviderConst.ProviderName);
        var streamId = StreamId.Create(StreamingProviderConst.StreamId, this.GetPrimaryKey());
        m_Stream = streamProvider.GetStream<ServerPackage>(streamId);
        
        return Task.CompletedTask;
    }
    
    public async Task Send(ServerPackage package)
    {
        try
        {
            await m_Stream.OnNextAsync(package);
        }
        catch (Exception e)
        {
            Console.WriteLine("SessionGrain Send Error: " + e.Message);
        }
    }
}