using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 数据驱动的技能定义。
    /// </summary>
    public sealed class GameplayAbilityDef
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public IReadOnlyList<GameplayTag> AbilityTags { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> ActivationRequiredTags { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> ActivationBlockedTags { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> CancelAbilitiesWithTag { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> BlockAbilitiesWithTag { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> ActivationOwnedTags { get; set; } = System.Array.Empty<GameplayTag>();

        public AbilityTargetRelation TargetRelation { get; set; } = AbilityTargetRelation.None;

        public AbilityInstancingPolicy InstancingPolicy { get; set; } = AbilityInstancingPolicy.Single;

        public bool Uninterruptible { get; set; }

        public IReadOnlyList<GameplayTag> InterruptTags { get; set; } = System.Array.Empty<GameplayTag>();

        /// <summary>
        /// 共享冷却标签。实体精确拥有此标签时激活失败（OnCooldown）。
        /// </summary>
        public GameplayTag CooldownGroupTag { get; set; }

        public IReadOnlyList<GameplayTag> TargetRequiredTags { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> TargetBlockedTags { get; set; } = System.Array.Empty<GameplayTag>();

        public AbilityTargetFallbackPolicy TargetFallbackPolicy { get; set; } = AbilityTargetFallbackPolicy.None;

        /// <summary>
        /// 提交时施加的 Instant 消耗效果。0 表示无消耗。
        /// </summary>
        public int CostEffectId { get; set; }

        /// <summary>
        /// 提交时施加的冷却效果。0 表示无冷却。
        /// </summary>
        public int CooldownEffectId { get; set; }

        /// <summary>
        /// 实现 <see cref="IGameplayAbility"/> 的逻辑类型。
        /// </summary>
        public System.Type AbilityLogicType { get; set; }
    }
}
