using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 玩法技能系统管理器。会话级：标签与定义注册、施加/激活、以及 duration/period/lifetime 驱动。
    /// </summary>
    public interface IGameAbilitySystemManager : IGameManager
    {
        /// <summary>
        /// 层次化标签注册表（名字实习与父子关系），不是实体拥有的标签。
        /// </summary>
        GameplayTagRegistry TagRegistry { get; }

        GameplayEffectDef RegisterEffect(GameplayEffectDef def);

        GameplayAbilityDef RegisterAbility(GameplayAbilityDef def);

        bool TryGetEffect(int effectDefId, out GameplayEffectDef def);

        bool TryGetAbility(int abilityDefId, out GameplayAbilityDef def);

        void RegisterComponent(AbilitySystemComponent component);

        void UnregisterComponent(AbilitySystemComponent component);

        AbilitySystemComponent EnsureAbilitySystem(IGameEntity entity);

        void GrantAbility(IGameEntity entity, int abilityDefId, int level = 1);

        bool SetAbilityLevel(IGameEntity entity, int abilityDefId, int level);

        bool TryGetAbilityLevel(IGameEntity entity, int abilityDefId, out int level);

        bool ClearAbility(IGameEntity entity, int abilityDefId);

        ApplyEffectResult ApplyEffect(IGameEntity target, in EffectSpec spec);

        int ApplyEffects(IReadOnlyList<IGameEntity> targets, in EffectSpec spec);

        bool RemoveEffect(IGameEntity entity, int activeHandle);

        int RemoveEffectsByDef(IGameEntity entity, int effectDefId);

        int RemoveEffectsByTag(IGameEntity entity, GameplayTag grantedTag);

        AbilityActivationResult TryActivateAbility(IGameEntity entity, int abilityDefId, IGameEntity target = null);

        bool CancelAbility(IGameEntity entity, int activeHandle);

        bool EndAbility(IGameEntity entity, int activeHandle, bool wasCancelled = false);

        bool TryInterruptAbility(IGameEntity entity, int activeHandle, GameplayTag interruptTag = default);

        void SetAbilityDuration(IGameEntity entity, int activeHandle, float duration);

        void AddAbilityTasks(IGameEntity entity, int activeHandle, IReadOnlyList<AbilityTask> tasks);

        /// <summary>
        /// 实体是否拥有该标签（层次匹配，与 <see cref="AbilitySystemComponent.HasTag"/> 一致）。
        /// </summary>
        bool HasTag(IGameEntity entity, GameplayTag tag);

        bool CanAffordCost(IGameEntity entity, int costEffectId);
    }
}
