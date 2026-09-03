using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 施加在 ASC 上的 Duration / Infinite 效果实例。
    /// </summary>
    public sealed class ActiveGameplayEffect : IReference
    {
        public int Handle;
        public int EffectDefId;
        public GameEntityRef SourceEntity;
        public float TimeRemaining;
        public float PeriodTimeRemaining;
        public bool UsesPeriod;
        public bool AggregatesModifiers;
        public readonly List<GameplayEffectModifier> Modifiers = new List<GameplayEffectModifier>();
        public readonly List<GameplayTag> GrantedTags = new List<GameplayTag>();

        public void Clear()
        {
            Handle = 0;
            EffectDefId = 0;
            SourceEntity = default;
            TimeRemaining = 0f;
            PeriodTimeRemaining = 0f;
            UsesPeriod = false;
            AggregatesModifiers = false;
            Modifiers.Clear();
            GrantedTags.Clear();
        }

        public static ActiveGameplayEffect Create()
        {
            return ReferencePool.Acquire<ActiveGameplayEffect>();
        }
    }
}
