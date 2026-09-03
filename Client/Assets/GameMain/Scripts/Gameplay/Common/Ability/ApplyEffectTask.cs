namespace GameMain
{
    /// <summary>
    /// 对解析后的目标施加一次效果后立即完成。
    /// </summary>
    public sealed class ApplyEffectTask : AbilityTask
    {
        private readonly EffectSpec m_Spec;
        private bool m_Applied;

        public ApplyEffectTask(in EffectSpec spec)
        {
            m_Spec = spec;
        }

        public override void OnStart(in AbilityActivationContext context)
        {
            if (m_Applied || IsComplete)
            {
                return;
            }

            m_Applied = true;
            var target = context.ResolvedTarget;
            if (target != null)
            {
                context.Manager.ApplyEffect(target, in m_Spec);
            }

            Complete();
        }
    }
}
