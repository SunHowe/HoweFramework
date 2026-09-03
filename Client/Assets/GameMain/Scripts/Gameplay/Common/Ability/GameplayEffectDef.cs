using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 数据驱动的游戏效果定义。
    /// </summary>
    public sealed class GameplayEffectDef
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DurationPolicy DurationPolicy { get; set; } = DurationPolicy.Instant;

        /// <summary>
        /// <see cref="DurationPolicy.Duration"/> 时的持续秒数。
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// 周期秒数。大于 0 时按通道 Execute；为 0 时 Duration/Infinite 的修正进入 Numeric 叠加。
        /// </summary>
        public float Period { get; set; }

        /// <summary>
        /// Period 大于 0 时，施加当下是否立刻 Execute 一次。
        /// </summary>
        public bool ExecutePeriodicOnApplication { get; set; }

        public IReadOnlyList<GameplayEffectModifier> Modifiers { get; set; } = System.Array.Empty<GameplayEffectModifier>();

        public IReadOnlyList<GameplayTag> GrantedTags { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> AssetTags { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> ApplicationRequiredTags { get; set; } = System.Array.Empty<GameplayTag>();

        public IReadOnlyList<GameplayTag> ApplicationImmunityTags { get; set; } = System.Array.Empty<GameplayTag>();

        public GameplayEffectStackingType StackingType { get; set; } = GameplayEffectStackingType.RefreshDuration;

        public int MaxStackCount { get; set; } = 1;

        public IReadOnlyList<int> GrantedAbilityDefIds { get; set; } = System.Array.Empty<int>();

        /// <summary>
        /// 兼容开关：StackingType 为 RefreshDuration 且此项为 false 时，退化为 None。
        /// </summary>
        public bool RefreshDurationOnReapply { get; set; } = true;
    }
}
