using HoweFramework;
using Protocol;

namespace HoweFramework
{
    /// <summary>
    /// Orleans 网络消息包处理器基类。
    /// </summary>
    /// <typeparam name="T">网络消息包类型。</typeparam>
    public abstract class OrleansPacketHandlerBase<T> : IPacketHandler where T : ProtocolBase
    {
        /// <summary>
        /// 网络消息包协议编号。
        /// </summary>
        public int Id => ProtocolBinder.GetProtocolId<T>() ?? 0;

        /// <summary>
        /// 处理网络消息包。
        /// </summary>
        /// <param name="sender">消息包源。</param>
        /// <param name="packet">网络消息包。</param>
        public void Handle(object sender, GameEventArgs packet)
        {
            OrleansPacket orleansPacket = packet as OrleansPacket;
            if (orleansPacket == null)
            {
                return;
            }

            T protocol = orleansPacket.Protocol as T;
            if (protocol == null)
            {
                return;
            }

            Handle(sender, protocol);
        }

        /// <summary>
        /// 处理网络消息包。
        /// </summary>
        /// <param name="sender">消息包源。</param>
        /// <param name="protocol">网络消息包。</param>
        protected abstract void Handle(object sender, T protocol);
    }
}
