using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏开始事件参数。
    /// </summary>
    public sealed class GameStartEventArgs : GameEventArgs
    {
        public static readonly int EventId = TypeId<GameStartEventArgs>.Id;

        /// <summary>
        /// 清理。
        /// </summary>
        protected override void OnClear()
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