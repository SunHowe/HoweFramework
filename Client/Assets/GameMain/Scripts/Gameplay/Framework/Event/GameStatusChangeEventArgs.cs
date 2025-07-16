using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏状态变更事件参数。
    /// </summary>
    public sealed class GameStatusChangeEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GameStatusChangeEventArgs).GetHashCode();

        /// <summary>
        /// 事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        /// 游戏状态。
        /// </summary>
        public GameStatus GameStatus { get; private set; }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            GameStatus = GameStatus.None;
        }

        /// <summary>
        /// 创建游戏状态变更事件参数。
        /// </summary>
        /// <param name="gameStatus">游戏状态。</param>
        /// <returns>游戏状态变更事件参数。</returns>
        public static GameStatusChangeEventArgs Create(GameStatus gameStatus)
        {
            var eventArgs = ReferencePool.Acquire<GameStatusChangeEventArgs>();
            eventArgs.GameStatus = gameStatus;
            return eventArgs;
        }
    }
}