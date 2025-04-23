using System;

namespace HoweFramework
{
    /// <summary>
    /// 游戏事件委托。
    /// </summary>
    /// <param name="sender">事件发送者。</param>
    /// <param name="e">事件参数。</param>
    public delegate void GameEventHandler(object sender, GameEventArgs e);

    /// <summary>
    /// 游戏事件参数。
    /// </summary>
    public abstract class GameEventArgs : EventArgs, IReference
    {
        /// <summary>
        /// 事件类型。
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// 引用id。
        /// </summary>
        public int ReferenceId { get; set; }

        /// <summary>
        /// 在事件处理后是否回收事件实例。
        /// </summary>
        public virtual bool IsReleaseAfterFire => true;

        public abstract void Clear();
    }
}
