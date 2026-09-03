using System;
using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 玩法技能系统管理器。
    /// </summary>
    public sealed partial class GameAbilitySystemManager : GameManagerBase, IGameAbilitySystemManager
    {
        public GameplayTagRegistry TagRegistry { get; } = new GameplayTagRegistry();

        private readonly Dictionary<int, GameplayEffectDef> m_Effects = new Dictionary<int, GameplayEffectDef>();
        private readonly Dictionary<int, GameplayAbilityDef> m_Abilities = new Dictionary<int, GameplayAbilityDef>();
        private readonly Dictionary<Type, IGameplayAbility> m_AbilityLogicCache = new Dictionary<Type, IGameplayAbility>();
        private readonly List<AbilitySystemComponent> m_Components = new List<AbilitySystemComponent>();
        private readonly List<GameplayEffectModifier> m_ScratchModifiers = new List<GameplayEffectModifier>(64);
        private readonly Dictionary<int, long> m_ScratchAdd = new Dictionary<int, long>();
        private readonly Dictionary<int, long> m_ScratchMul = new Dictionary<int, long>();
        private readonly Dictionary<int, long> m_ScratchOverride = new Dictionary<int, long>();

        private IGameUpdateManager m_UpdateManager;
        private int m_NextEffectDefId = 1;
        private int m_NextAbilityDefId = 1;

        public GameplayEffectDef RegisterEffect(GameplayEffectDef def)
        {
            if (def == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "GameplayEffectDef is null.");
            }

            if (def.Id == 0)
            {
                def.Id = m_NextEffectDefId++;
            }

            if (!m_Effects.TryAdd(def.Id, def))
            {
                throw new InvalidOperationException(string.Format("Effect def {0} already registered.", def.Id));
            }

            if (def.Id >= m_NextEffectDefId)
            {
                m_NextEffectDefId = def.Id + 1;
            }

            return def;
        }

        public GameplayAbilityDef RegisterAbility(GameplayAbilityDef def)
        {
            if (def == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "GameplayAbilityDef is null.");
            }

            if (def.Id == 0)
            {
                def.Id = m_NextAbilityDefId++;
            }

            if (!m_Abilities.TryAdd(def.Id, def))
            {
                throw new InvalidOperationException(string.Format("Ability def {0} already registered.", def.Id));
            }

            if (def.Id >= m_NextAbilityDefId)
            {
                m_NextAbilityDefId = def.Id + 1;
            }

            return def;
        }

        public bool TryGetEffect(int effectDefId, out GameplayEffectDef def)
        {
            return m_Effects.TryGetValue(effectDefId, out def);
        }

        public bool TryGetAbility(int abilityDefId, out GameplayAbilityDef def)
        {
            return m_Abilities.TryGetValue(abilityDefId, out def);
        }

        public void RegisterComponent(AbilitySystemComponent component)
        {
            if (component == null || m_Components.Contains(component))
            {
                return;
            }

            m_Components.Add(component);
        }

        public void UnregisterComponent(AbilitySystemComponent component)
        {
            m_Components.Remove(component);
        }

        public AbilitySystemComponent EnsureAbilitySystem(IGameEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            var asc = entity.GetComponent<AbilitySystemComponent>();
            if (asc == null)
            {
                asc = entity.AddComponent<AbilitySystemComponent>();
            }

            return asc;
        }

        public bool HasTag(IGameEntity entity, GameplayTag tag)
        {
            var asc = GetAsc(entity);
            return asc != null && asc.OwnedTags.HasTag(tag, TagRegistry);
        }

        protected override void OnAwake()
        {
            m_UpdateManager = Context.GetManager<IGameUpdateManager>();
            m_UpdateManager.RegisterFixedUpdate(this, OnFixedUpdate);
        }

        protected override void OnDispose()
        {
            if (m_UpdateManager != null)
            {
                m_UpdateManager.UnregisterFixedUpdate(this, OnFixedUpdate);
                m_UpdateManager = null;
            }

            using var snapshot = ReusableList<AbilitySystemComponent>.Create();
            snapshot.AddRange(m_Components);
            for (var i = 0; i < snapshot.Count; i++)
            {
                snapshot[i].ShutdownRuntime();
            }

            m_Components.Clear();
            m_Effects.Clear();
            m_Abilities.Clear();
            m_AbilityLogicCache.Clear();
            TagRegistry.Clear();
            m_ScratchModifiers.Clear();
            m_ScratchAdd.Clear();
            m_ScratchMul.Clear();
            m_ScratchOverride.Clear();
            m_NextEffectDefId = 1;
            m_NextAbilityDefId = 1;
        }

        private void OnFixedUpdate(float elapseSeconds)
        {
            TickPeriods(elapseSeconds);
            TickDurations(elapseSeconds);
            TickAbilities(elapseSeconds);
        }

        private AbilitySystemComponent GetAsc(IGameEntity entity)
        {
            return entity?.GetComponent<AbilitySystemComponent>();
        }

        private static bool IsAlive(AbilitySystemComponent asc)
        {
            return asc != null && asc.Entity != null;
        }

        private static GameplayModifierChannel ResolveChannel(GameplayEffectDef def, in GameplayEffectModifier modifier)
        {
            if (modifier.Channel != GameplayModifierChannel.Auto)
            {
                return modifier.Channel;
            }

            if (def.DurationPolicy == DurationPolicy.Instant || def.Period > 0f)
            {
                return GameplayModifierChannel.ExecuteResource;
            }

            return GameplayModifierChannel.Overlay;
        }

        private static NumericSubType ResolveOverlaySubType(in GameplayEffectModifier modifier)
        {
            if (modifier.Op == ModifierOp.Override)
            {
                return NumericSubType.Override;
            }

            switch (modifier.NumericSubType)
            {
                case NumericSubType.BasicPercent:
                case NumericSubType.BasicConstAdd:
                case NumericSubType.FinalPercent:
                case NumericSubType.FinalConstAdd:
                    return modifier.NumericSubType;
                default:
                    return modifier.Op == ModifierOp.Multiply
                        ? NumericSubType.BasicPercent
                        : NumericSubType.BasicConstAdd;
            }
        }

        private static List<GameplayEffectModifier> SnapshotModifiers(
            IReadOnlyList<GameplayEffectModifier> modifiers,
            IReadOnlyDictionary<string, long> paramBag)
        {
            var list = new List<GameplayEffectModifier>(modifiers == null ? 0 : modifiers.Count);
            if (modifiers == null)
            {
                return list;
            }

            for (var i = 0; i < modifiers.Count; i++)
            {
                list.Add(modifiers[i].Snapshot(paramBag));
            }

            return list;
        }

        private void AddGrantedTags(AbilitySystemComponent asc, IReadOnlyList<GameplayTag> tags, object provider)
        {
            if (tags == null || tags.Count == 0)
            {
                return;
            }

            asc.OwnedTags.AddTags(tags);
            var state = asc.GetComponent<StateComponent>();
            if (state == null)
            {
                return;
            }

            for (var i = 0; i < tags.Count; i++)
            {
                if (tags[i].IsValid)
                {
                    state.AddState(tags[i].Id, provider);
                }
            }
        }

        private void RemoveGrantedTags(AbilitySystemComponent asc, IReadOnlyList<GameplayTag> tags, object provider)
        {
            if (tags == null || tags.Count == 0)
            {
                return;
            }

            asc.OwnedTags.RemoveTags(tags);
            var state = asc.GetComponent<StateComponent>();
            if (state == null)
            {
                return;
            }

            for (var i = 0; i < tags.Count; i++)
            {
                if (tags[i].IsValid)
                {
                    state.RemoveState(tags[i].Id, provider);
                }
            }
        }

        private void EmitCues(IGameEntity target, GameplayEffectDef def, bool isAdded)
        {
            if (def.AssetTags == null || def.AssetTags.Count == 0)
            {
                return;
            }

            for (var i = 0; i < def.AssetTags.Count; i++)
            {
                Context.EventDispatcher.Dispatch(Context, GameplayCueEventArgs.Create(target, def.AssetTags[i], isAdded, def.Id));
            }
        }

        private void EmitLifecycle(
            IGameEntity target,
            GameplayEffectDef def,
            int activeHandle,
            IGameEntity source,
            GameplayEffectLifecycleKind kind,
            bool wasInstant)
        {
            Context.EventDispatcher.Dispatch(
                Context,
                GameplayEffectLifecycleEventArgs.Create(target, def.Id, activeHandle, source, kind, wasInstant));
        }

        private NumericComponent EnsureNumeric(IGameEntity entity)
        {
            var numeric = entity.GetComponent<NumericComponent>();
            return numeric ?? entity.AddComponent<NumericComponent>();
        }

        private ResourceComponent EnsureResource(IGameEntity entity)
        {
            var resource = entity.GetComponent<ResourceComponent>();
            return resource ?? entity.AddComponent<ResourceComponent>();
        }
    }
}
