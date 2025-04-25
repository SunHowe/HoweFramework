using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 协议处理器抽象类.
/// </summary>
public abstract class ProtocolHandler<T, TResponse> : IProtocolHandler
    where T : IProtocol
    where TResponse : IProtocolResponse, new()
{
    public async Task<IProtocolResponse> Handle(IUserSessionGrain sessionGrain, IProtocol request)
    {
        var response = new TResponse();

        try
        {
            await OnHandle(sessionGrain, (T)request, response);
        }
        catch (GameException e)
        {
            Console.WriteLine(e);
            response.ErrorCode = e.ErrorCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            response.ErrorCode = GameErrorCode.Internal;
        }

        return response;
    }

    protected abstract Task OnHandle(IUserSessionGrain sessionGrain, T request, TResponse response);
}

/// <summary>
/// 使用通用应答包返回的协议处理器.
/// </summary>
public abstract class ProtocolHandler<T> : ProtocolHandler<T, CommonResp>
    where T : IProtocol
{
}