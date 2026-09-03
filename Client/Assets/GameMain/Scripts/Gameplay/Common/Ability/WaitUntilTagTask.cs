namespace GameMain
{
    /// <summary>
    /// 拥有者持有指定标签后完成。超时则取消（进而取消技能）。
    /// </summary>
    public sealed class WaitUntilTagTask : AbilityTask
    {
        private readonly GameplayTag m_Tag;
        private readonly float m_TimeoutSeconds;
        private float m_Elapsed;

        public WaitUntilTagTask(GameplayTag tag, float timeoutSeconds = -1f)
        {
            m_Tag = tag;
            m_TimeoutSeconds = timeoutSeconds;
        }

        public override void Tick(in AbilityActivationContext context, float deltaTime)
        {
            if (IsComplete)
            {
                return;
            }

            m_Elapsed += deltaTime;
            if (context.Manager.HasTag(context.Owner, m_Tag))
            {
                Complete();
                return;
            }

            if (m_TimeoutSeconds >= 0f && m_Elapsed >= m_TimeoutSeconds)
            {
                Cancel();
            }
        }
    }
}
