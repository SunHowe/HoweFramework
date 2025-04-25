using MemoryPack;
using Protocol;

namespace ServerProtocol;

public static class ServerPackageHelper
{
    /// <summary>
    /// 根据协议包创建传输对象.
    /// </summary>
    public static ServerPackage Pack(IProtocol message)
    {
        var messageType = message.GetType();

        var id = ProtocolBinder.GetProtocolId(messageType);
        if (id == null)
        {
            throw new Exception($"未找到协议ID: {messageType.Name}");
        }
        
        return new ServerPackage
        {
            ProtocolId = id.Value,
            ProtocolBody = MemoryPackSerializer.Serialize(messageType, message)
        };
    }

    /// <summary>
    /// 解析协议包.
    /// </summary>
    public static IProtocol? Unpack(this ServerPackage package)
    {
        var messageType = ProtocolBinder.GetProtocolType(package.ProtocolId);
        if (messageType == null)
        {
            return null;
        }

        return MemoryPackSerializer.Deserialize(messageType, package.ProtocolBody!) as IProtocol;
    }

    /// <summary>
    /// 解析协议包.
    /// </summary>
    public static T? Unpack<T>(this ServerPackage package) where T : class, IProtocol
    {
        return MemoryPackSerializer.Deserialize(typeof(T), package.ProtocolBody!) as T;
    }
}