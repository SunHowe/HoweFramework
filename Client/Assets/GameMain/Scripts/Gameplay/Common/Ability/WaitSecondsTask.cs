namespace GameMain
{
    /// <summary>
    /// 累计等待指定秒数后完成。
    /// </summary>
    public sealed class WaitSecondsTask : AbilityTask
    {
        private readonly float m_Seconds;
        private float m_Elapsed;

        public WaitSecondsTask(float seconds)
        {
            m_Seconds = seconds < 0f ? 0f : seconds;
        }

        public override void Tick(in AbilityActivationContext context, float deltaTime)
        {
            if (IsComplete)
            {
                return;
            }

            m_Elapsed += deltaTime;
            if (m_Elapsed >= m_Seconds)
            {
                Complete();
            }
        }
    }
}
