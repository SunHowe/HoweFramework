using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 玩家信息更新事件参数。
    /// </summary>
    public sealed class PlayerInfoUpdateEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(PlayerInfoUpdateEventArgs).GetHashCode();

        public override int Id => EventId;

        public override void Clear()
        {
        }

        public static PlayerInfoUpdateEventArgs Create()
        {
            var eventArgs = ReferencePool.Acquire<PlayerInfoUpdateEventArgs>();
            return eventArgs;
        }
    }
}

