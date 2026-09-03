using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 技能系统组件。持有标签、已授予/激活的技能与生效中的效果。不含 AttributeSet。
    /// </summary>
    public sealed class AbilitySystemComponent : GameComponentBase
    {
        public GameplayTagContainer OwnedTags { get; } = new GameplayTagContainer();

        public GameplayTagContainer BlockedAbilityTags { get; } = new GameplayTagContainer();

        public List<ActiveGameplayEffect> ActiveEffects { get; } = new List<ActiveGameplayEffect>();

        public List<GrantedAbility> GrantedAbilities { get; } = new List<GrantedAbility>();

        public List<ActiveAbility> ActiveAbilities { get; } = new List<ActiveAbility>();

        public int NextEffectHandle { get; set; } = 1;

        public int NextAbilityHandle { get; set; } = 1;

        internal IGameAbilitySystemManager Manager { get; private set; }

        public ApplyEffectResult ApplyEffect(in EffectSpec spec)
        {
            if (Manager == null)
            {
                return ApplyEffectResult.Failed(ApplyEffectFailReason.MissingAsc);
            }

            return Manager.ApplyEffect(Entity, in spec);
        }

        public AbilityActivationResult TryActivateAbility(int abilityDefId, IGameEntity target = null)
        {
            if (Manager == null)
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.MissingAsc);
            }

            return Manager.TryActivateAbility(Entity, abilityDefId, target);
        }

        public void GrantAbility(int abilityDefId, int level = 1)
        {
            Manager?.GrantAbility(Entity, abilityDefId, level);
        }

        public bool HasTag(GameplayTag tag)
        {
            return Manager != null && Manager.HasTag(Entity, tag);
        }

        internal void ShutdownRuntime()
        {
            if (Manager != null)
            {
                for (var i = ActiveAbilities.Count - 1; i >= 0; i--)
                {
                    Manager.EndAbility(Entity, ActiveAbilities[i].Handle, true);
                }

                for (var i = ActiveEffects.Count - 1; i >= 0; i--)
                {
                    Manager.RemoveEffect(Entity, ActiveEffects[i].Handle);
                }
            }

            ReleaseRemainingRuntime();
        }

        internal void ReleaseRemainingRuntime()
        {
            for (var i = ActiveAbilities.Count - 1; i >= 0; i--)
            {
                ReferencePool.Release(ActiveAbilities[i]);
            }

            ActiveAbilities.Clear();

            for (var i = ActiveEffects.Count - 1; i >= 0; i--)
            {
                ReferencePool.Release(ActiveEffects[i]);
            }

            ActiveEffects.Clear();
            GrantedAbilities.Clear();
            OwnedTags.Clear();
            BlockedAbilityTags.Clear();
            NextEffectHandle = 1;
            NextAbilityHandle = 1;
        }

        protected override void OnAwake()
        {
            NextEffectHandle = 1;
            NextAbilityHandle = 1;
            Manager = Context.GetManager<IGameAbilitySystemManager>();
            Manager?.RegisterComponent(this);
        }

        protected override void OnDispose()
        {
            ShutdownRuntime();
            Manager?.UnregisterComponent(this);
            Manager = null;

            var numeric = this.GetComponent<NumericComponent>();
            numeric?.RemoveFromSource(this);
        }
    }
}
