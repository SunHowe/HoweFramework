namespace ServerProtocol;

/// <summary>
/// 供网关服务器与游戏服务器交互的协议包.作为IGrains接口的参数进行传递.
/// </summary>
[GenerateSerializer]
public class ServerPackage
{
    [Id(0)]
    public ushort ProtocolId { get; set; }
    
    [Id(1)]
    public byte[]? ProtocolBody { get; set; }
    
    [Id(2)]
    public int RpcId { get; set; }
    
    [Id(3)]
    public int ErrorCode { get; set; }
}