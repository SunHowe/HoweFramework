namespace GameMain
{
    /// <summary>
    /// 激活期间传给技能逻辑的上下文。
    /// </summary>
    public readonly struct AbilityActivationContext
    {
        public IGameEntity Owner { get; }

        public IGameEntity Target { get; }

        public int AbilityDefId { get; }

        public int ActiveHandle { get; }

        public int Level { get; }

        public IGameAbilitySystemManager Manager { get; }

        public AbilityActivationContext(
            IGameEntity owner,
            int abilityDefId,
            int activeHandle,
            IGameAbilitySystemManager manager,
            IGameEntity target = null,
            int level = 1)
        {
            Owner = owner;
            AbilityDefId = abilityDefId;
            ActiveHandle = activeHandle;
            Manager = manager;
            Target = target;
            Level = level < 1 ? 1 : level;
        }

        /// <summary>
        /// 显式目标存在时用目标，否则用拥有者。
        /// </summary>
        public IGameEntity ResolvedTarget => Target ?? Owner;

        public void EndAbility(bool wasCancelled = false)
        {
            Manager.EndAbility(Owner, ActiveHandle, wasCancelled);
        }

        public void CancelAbility()
        {
            Manager.CancelAbility(Owner, ActiveHandle);
        }
    }
}
