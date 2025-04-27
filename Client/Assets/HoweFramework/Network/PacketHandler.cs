using System;

namespace HoweFramework
{
    /// <summary>
    /// 网络消息包处理器基类。
    /// </summary>
    public abstract class PacketHandler<T> : IPacketHandler where T : Packet
    {
        /// <summary>
        /// 网络消息包协议编号。
        /// </summary>
        public abstract int Id { get; }

        public void Handle(object sender, GameEventArgs packet)
        {
            Handle((INetworkChannel)sender, (T)packet);
        }

        /// <summary>
        /// 处理网络消息包。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="packet">网络消息包。</param>
        protected abstract void Handle(INetworkChannel networkChannel, T packet);
    }
}

