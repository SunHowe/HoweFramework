namespace GameMain
{
    /// <summary>
    /// 激活后立即结束的技能模板。
    /// </summary>
    public abstract class InstantGameplayAbility : IGameplayAbility
    {
        public void Activate(in AbilityActivationContext context)
        {
            OnActivate(in context);
            context.EndAbility(false);
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
