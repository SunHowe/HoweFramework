namespace Grains;

/// <summary>
/// 协议处理器绑定attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ProtocolHandlerAttribute : Attribute
{
    public ushort ProtocolId { get; }

    public ProtocolHandlerAttribute(ushort protocolId)
    {
        ProtocolId = protocolId;
    }
}