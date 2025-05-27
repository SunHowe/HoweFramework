using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 在线玩家系统。
    /// </summary>
    public sealed class OnlinePlayerSystem : SystemBase, IPlayerSystem
    {
        /// <summary>
        /// 玩家信息。
        /// </summary>
        public Bindable<PlayerInfo> PlayerInfo { get; } = new Bindable<PlayerInfo>(new PlayerInfo());

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
        }
    }
}
