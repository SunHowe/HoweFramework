
using Geek.Server.App.Common;
using Geek.Server.Core.Net;

namespace Server.Logic.Common.Handler;

public static class NetChannelExtensions
{
    public static void Write(this NetChannel channel, Message msg, int uniId, ServerErrorCode code = ServerErrorCode.Success)
    {
        if (msg == null)
        {
            throw new ArgumentNullException(nameof(msg));
        }
        
        msg.UniId = uniId;
        
        if (msg is ResponseMessage resp)
        {
            resp.ErrorCode = (int)code;
        }

        channel.Write(msg);
    }
}