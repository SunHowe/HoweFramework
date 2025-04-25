using Protocol;
using ServerProtocol;

namespace GatewayServer;

public static class GatewayServerPackageHelper
{
    /// <summary>
    /// 根据协议头和协议body数据创建传输对象.
    /// </summary>
    public static ServerPackage Pack(in RequestHeader header, byte[] body)
    {
        return new ServerPackage
        {
            ProtocolId = header.ProtocolId,
            ProtocolBody = body,
            RpcId = header.RpcId,
        };
    }

    /// <summary>
    /// 根据传输对象解析出协议头和协议body.
    /// </summary>
    public static void Unpack(ServerPackage package, out ResponseHeader responseHeader, out byte[]? bytes)
    {
        bytes = package.ProtocolBody;

        if (package.RpcId != 0)
        {
            responseHeader = new ResponseHeader
            {
                ProtocolId = package.ProtocolId,
                RpcId = package.RpcId,
                ErrorCode = package.ErrorCode,
                BodyLength = (ushort)(package.ProtocolBody?.Length ?? 0),
            };
        }
        else
        {
            responseHeader = new ResponseHeader
            {
                ProtocolId = package.ProtocolId,
                BodyLength = (ushort)(package.ProtocolBody?.Length ?? 0),
            };
        }
    }
}