using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏停止事件参数。
    /// </summary>
    public sealed class GameStopEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GameStopEventArgs).GetHashCode();

        /// <summary>
        /// 事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
        }

        /// <summary>
        /// 创建游戏停止事件参数。
        /// </summary>
        /// <returns>游戏停止事件参数。</returns>
        public static GameStopEventArgs Create()
        {
            return ReferencePool.Acquire<GameStopEventArgs>();
        }
    }
}