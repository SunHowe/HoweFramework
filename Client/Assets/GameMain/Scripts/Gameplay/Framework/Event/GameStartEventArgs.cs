using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏开始事件参数。
    /// </summary>
    public sealed class GameStartEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GameStartEventArgs).GetHashCode();

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
        /// 创建游戏开始事件参数。
        /// </summary>
        /// <returns>游戏开始事件参数。</returns>
        public static GameStartEventArgs Create()
        {
            return ReferencePool.Acquire<GameStartEventArgs>();
        }
    }
}