using IGrains;
using Protocol;

namespace Grains;

/// <summary>
/// 协议处理器接口.
/// </summary>
public interface IProtocolHandler
{
    Task<IProtocolResponse> Handle(IPlayerSessionGrain sessionGrain, IProtocol request);
}