using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 玩家系统接口。
    /// </summary>
    public interface IPlayerSystem : ISystem
    {
        /// <summary>
        /// 玩家信息。
        /// </summary>
        Bindable<PlayerInfo> PlayerInfo { get; }
    }
}
