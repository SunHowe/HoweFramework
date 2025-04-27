namespace HoweFramework
{
    /// <summary>
    /// 网络消息包基类。
    /// </summary>
    public abstract class Packet : GameEventArgs
    {
        /// <summary>
        /// 在事件处理后是否回收事件实例。
        /// </summary>
        public override bool IsReleaseAfterFire => m_IsReleaseAfterFire;

        private bool m_IsReleaseAfterFire = true;

        /// <summary>
        /// 设置在事件处理后是否回收事件实例。
        /// </summary>
        /// <param name="isReleaseAfterFire">是否回收事件实例。</param>
        public void SetIsReleaseAfterFire(bool isReleaseAfterFire)
        {
            m_IsReleaseAfterFire = isReleaseAfterFire;
        }

        public override void Clear()
        {
            m_IsReleaseAfterFire = true;
        }
    }
}
