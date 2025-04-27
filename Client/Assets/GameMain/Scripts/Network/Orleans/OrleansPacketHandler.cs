using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// Orleans 网络消息包处理器基类。
    /// </summary>
    /// <typeparam name="T">网络消息包类型。</typeparam>
    public abstract class OrleansPacketHandler<T> : PacketHandler<T> where T : ProtocolBase
    {
        /// <summary>
        /// 网络消息包协议编号。
        /// </summary>
        public override int Id => ProtocolBinder.GetProtocolId<T>() ?? 0;
    }
}
