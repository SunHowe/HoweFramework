using System.Reflection;

namespace Grains;

/// <summary>
/// 协议处理器管理器.
/// </summary>
public static class ProtocolHandlerManager
{
    private static readonly Dictionary<ushort, IProtocolHandler> s_HandlerDict = new();
    
    /// <summary>
    /// 初始化.
    /// </summary>
    public static void Init()
    {
        s_HandlerDict.Clear();

        var assembly = typeof(ProtocolHandlerManager).Assembly;
        var baseType = typeof(IProtocolHandler);
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || !baseType.IsAssignableFrom(type))
            {
                continue;
            }

            var attribute = type.GetCustomAttribute<ProtocolHandlerAttribute>();
            if (attribute == null)
            {
                continue;
            }
            
            var handler = (IProtocolHandler) Activator.CreateInstance(type)!;
            s_HandlerDict.Add(attribute.ProtocolId, handler);
        }
    }
    
    /// <summary>
    /// 获取协议处理器.
    /// </summary>
    public static IProtocolHandler? Get(ushort protocolId)
    {
        return s_HandlerDict.GetValueOrDefault(protocolId);
    }
}