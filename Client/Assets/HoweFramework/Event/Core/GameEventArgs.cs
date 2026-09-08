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
    /// 游戏事件委托。
    /// </summary>
    /// <param name="sender">事件发送者。</param>
    /// <param name="e">事件参数。</param>
    /// <returns>是否已处理。</returns>
    public delegate bool GameEventHandlerFunc(object sender, GameEventArgs e);

    /// <summary>
    /// 游戏事件参数。
    /// </summary>
    public abstract class GameEventArgs : EventArgs, IReference
    {
        /// <summary>
        /// 事件类型（具体类型的运行时 TypeId）。
        /// </summary>
        public int Id { get; }

        protected GameEventArgs()
        {
            Id = TypeId.GetIdByType(GetType());
        }

        /// <summary>
        /// 在事件处理后是否回收事件实例。
        /// </summary>
        public bool IsReleaseAfterFire => m_IsReleaseAfterFire;

        private bool m_IsReleaseAfterFire = true;

        /// <summary>
        /// 设置在事件处理后是否回收事件实例。
        /// </summary>
        /// <param name="isReleaseAfterFire">是否回收事件实例。</param>
        public void SetIsReleaseAfterFire(bool isReleaseAfterFire)
        {
            m_IsReleaseAfterFire = isReleaseAfterFire;
        }

        /// <summary>
        /// 清理引用。
        /// </summary>
        public void Clear()
        {
            m_IsReleaseAfterFire = true;
            OnClear();
        }

        /// <summary>
        /// 清理派生类字段。
        /// </summary>
        protected abstract void OnClear();
    }
}
