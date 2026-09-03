using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 正在执行的技能实例。
    /// </summary>
    public sealed class ActiveAbility : IReference
    {
        public int Handle;
        public int AbilityDefId;
        public int Level = 1;
        public GameEntityRef TargetEntity;
        public float RemainingDuration = -1f;
        public readonly List<GameplayTag> OwnedTags = new List<GameplayTag>();
        public readonly List<GameplayTag> BlockedAbilityTags = new List<GameplayTag>();
        public readonly List<AbilityTask> Tasks = new List<AbilityTask>();
        public IGameplayAbility Logic;
        public bool IsEnding;

        public void Clear()
        {
            Handle = 0;
            AbilityDefId = 0;
            Level = 1;
            TargetEntity = default;
            RemainingDuration = -1f;
            OwnedTags.Clear();
            BlockedAbilityTags.Clear();
            Tasks.Clear();
            Logic = null;
            IsEnding = false;
        }

        public static ActiveAbility Create()
        {
            return ReferencePool.Acquire<ActiveAbility>();
        }
    }
}
