namespace GameMain
{
    /// <summary>
    /// 持续一段时间后自动结束的技能模板。眩晕/沉默/中毒应使用 Duration 效果，而不是此类技能。
    /// </summary>
    public abstract class DurationGameplayAbility : IGameplayAbility
    {
        /// <summary>
        /// 激活后保持的秒数。
        /// </summary>
        public abstract float DurationSeconds { get; }

        public void Activate(in AbilityActivationContext context)
        {
            context.Manager.SetAbilityDuration(context.Owner, context.ActiveHandle, DurationSeconds);
            OnActivate(in context);
        }

        public void OnEnd(in AbilityActivationContext context, bool wasCancelled)
        {
            OnAbilityEnded(in context, wasCancelled);
        }

        protected abstract void OnActivate(in AbilityActivationContext context);

        protected virtual void OnAbilityEnded(in AbilityActivationContext context, bool wasCancelled)
        {
        }
    }
}
