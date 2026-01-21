using HoweFramework;
using PolymorphicMessagePack;

namespace GameMain
{
    /// <summary>
    /// 对接GeekServer的服务器消息包处理器基类。
    /// </summary>
    public abstract class MessageHandler<T> : IPacketHandler where T: Message
    {
        public int Id => PolymorphicTypeMapper.TryGet(typeof(T), out var id) ? id : -1;
        public void Handle(object sender, GameEventArgs packet)
        {
            var msg = packet as GeekServerPacket;
            if (msg?.Message is not T message)
            {
                throw new ErrorCodeException(ErrorCode.NetworkDeserializePacketError, $"Invalid message type: {typeof(T).FullName}");
            }

            Handle((INetworkChannel)sender, message);
        }
        
        protected abstract void Handle(INetworkChannel networkChannel, T message);
    }
}