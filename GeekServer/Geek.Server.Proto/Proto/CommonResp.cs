using MessagePack;

namespace Geek.Server.Proto
{
    /// <summary>
    /// 通用响应包。
    /// </summary>
    [MessagePackObject(true)]
    public class CommonResp : ResponseMessage
    {
    }
}