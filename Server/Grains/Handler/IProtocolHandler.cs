using IGrains;
using Protocol;

namespace Grains;

/// <summary>
/// 协议处理器接口.
/// </summary>
public interface IProtocolHandler
{
    Task<IResponse> Handle(IUserSessionGrain sessionGrain, IProtocol request);
}