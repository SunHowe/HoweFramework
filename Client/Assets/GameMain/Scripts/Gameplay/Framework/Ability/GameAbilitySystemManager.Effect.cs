using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    public sealed partial class GameAbilitySystemManager
    {
        public ApplyEffectResult ApplyEffect(IGameEntity target, in EffectSpec spec)
        {
            if (target == null || !m_Effects.TryGetValue(spec.EffectDefId, out var def))
            {
                return ApplyEffectResult.Failed(ApplyEffectFailReason.UnknownEffect);
            }

            var asc = EnsureAbilitySystem(target);
            if (asc == null)
            {
                return ApplyEffectResult.Failed(ApplyEffectFailReason.MissingAsc);
            }

            if (def.ApplicationRequiredTags != null &&
                def.ApplicationRequiredTags.Count > 0 &&
                !asc.OwnedTags.HasAll(def.ApplicationRequiredTags, TagRegistry))
            {
                return ApplyEffectResult.Failed(ApplyEffectFailReason.MissingApplicationRequiredTags);
            }

            if (def.ApplicationImmunityTags != null &&
                def.ApplicationImmunityTags.Count > 0 &&
                asc.OwnedTags.HasAny(def.ApplicationImmunityTags, TagRegistry))
            {
                return ApplyEffectResult.Failed(ApplyEffectFailReason.BlockedByImmunityTags);
            }

            var snapshotted = SnapshotModifiers(def.Modifiers, spec.Params);

            if (def.DurationPolicy == DurationPolicy.Instant)
            {
                ExecuteModifiers(target, def, snapshotted);
                EmitCues(target, def, true);
                EmitLifecycle(target, def, 0, spec.Source, GameplayEffectLifecycleKind.Applied, true);
                return ApplyEffectResult.Instant();
            }

            var stacking = def.StackingType;
            if (stacking == GameplayEffectStackingType.RefreshDuration && !def.RefreshDurationOnReapply)
            {
                stacking = GameplayEffectStackingType.None;
            }

            var existingIndex = FindActiveEffect(asc, def.Id);
            if (existingIndex >= 0)
            {
                switch (stacking)
                {
                    case GameplayEffectStackingType.None:
                        return ApplyEffectResult.Applied(asc.ActiveEffects[existingIndex].Handle);
                    case GameplayEffectStackingType.RefreshDuration:
                        RefreshActiveEffect(asc, existingIndex, def);
                        EmitCues(target, def, true);
                        EmitLifecycle(target, def, asc.ActiveEffects[existingIndex].Handle, spec.Source, GameplayEffectLifecycleKind.Applied, false);
                        return ApplyEffectResult.Refreshed(asc.ActiveEffects[existingIndex].Handle);
                    case GameplayEffectStackingType.StackCount:
                        var cap = def.MaxStackCount < 1 ? 1 : def.MaxStackCount;
                        if (CountActiveEffects(asc, def.Id) >= cap)
                        {
                            var oldest = FindSoonestExpiring(asc, def.Id);
                            RefreshActiveEffect(asc, oldest, def);
                            EmitCues(target, def, true);
                            EmitLifecycle(target, def, asc.ActiveEffects[oldest].Handle, spec.Source, GameplayEffectLifecycleKind.Applied, false);
                            return ApplyEffectResult.Refreshed(asc.ActiveEffects[oldest].Handle);
                        }

                        break;
                }
            }

            var active = ActiveGameplayEffect.Create();
            active.Handle = asc.NextEffectHandle++;
            active.EffectDefId = def.Id;
            active.SourceEntity = new GameEntityRef(spec.Source);
            active.TimeRemaining = def.DurationPolicy == DurationPolicy.Duration ? def.Duration : 0f;
            active.PeriodTimeRemaining = def.Period > 0f ? def.Period : 0f;
            active.UsesPeriod = def.Period > 0f;
            active.AggregatesModifiers = def.Period <= 0f;
            active.Modifiers.AddRange(snapshotted);
            if (def.GrantedTags != null)
            {
                active.GrantedTags.AddRange(def.GrantedTags);
            }

            asc.ActiveEffects.Add(active);
            AddGrantedTags(asc, active.GrantedTags, active);
            GrantAbilitiesFromEffect(target, def, spec.Level);

            if (active.AggregatesModifiers)
            {
                RecalculateAttributes(asc);
            }

            if (active.UsesPeriod && def.ExecutePeriodicOnApplication)
            {
                ExecuteModifiers(target, def, active.Modifiers);
            }

            EmitCues(target, def, true);
            EmitLifecycle(target, def, active.Handle, spec.Source, GameplayEffectLifecycleKind.Applied, false);
            return ApplyEffectResult.Applied(active.Handle);
        }

        public int ApplyEffects(IReadOnlyList<IGameEntity> targets, in EffectSpec spec)
        {
            if (targets == null || targets.Count == 0)
            {
                return 0;
            }

            var applied = 0;
            for (var i = 0; i < targets.Count; i++)
            {
                if (ApplyEffect(targets[i], in spec).Success)
                {
                    applied++;
                }
            }

            return applied;
        }

        public bool RemoveEffect(IGameEntity entity, int activeHandle)
        {
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return false;
            }

            for (var i = 0; i < asc.ActiveEffects.Count; i++)
            {
                if (asc.ActiveEffects[i].Handle != activeHandle)
                {
                    continue;
                }

                RemoveActiveEffectAt(entity, asc, i, false);
                return true;
            }

            return false;
        }

        public int RemoveEffectsByDef(IGameEntity entity, int effectDefId)
        {
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return 0;
            }

            var removed = 0;
            for (var i = asc.ActiveEffects.Count - 1; i >= 0; i--)
            {
                if (asc.ActiveEffects[i].EffectDefId != effectDefId)
                {
                    continue;
                }

                RemoveActiveEffectAt(entity, asc, i, false);
                removed++;
            }

            return removed;
        }

        public int RemoveEffectsByTag(IGameEntity entity, GameplayTag grantedTag)
        {
            var asc = GetAsc(entity);
            if (asc == null || !grantedTag.IsValid)
            {
                return 0;
            }

            var removed = 0;
            for (var i = asc.ActiveEffects.Count - 1; i >= 0; i--)
            {
                if (!EffectGrantsMatchingTag(asc.ActiveEffects[i], grantedTag))
                {
                    continue;
                }

                RemoveActiveEffectAt(entity, asc, i, false);
                removed++;
            }

            return removed;
        }

        public bool CanAffordCost(IGameEntity entity, int costEffectId)
        {
            if (entity == null || !m_Effects.TryGetValue(costEffectId, out var def))
            {
                return false;
            }

            var runningResource = new Dictionary<int, long>();
            var runningNumeric = new Dictionary<int, long>();

            var modifiers = def.Modifiers;
            if (modifiers == null)
            {
                return true;
            }

            for (var i = 0; i < modifiers.Count; i++)
            {
                var mod = modifiers[i];
                if (mod.AttributeId == 0)
                {
                    continue;
                }

                var channel = ResolveChannel(def, in mod);
                if (channel == GameplayModifierChannel.Overlay)
                {
                    continue;
                }

                long current;
                if (channel == GameplayModifierChannel.ExecuteNumeric)
                {
                    if (!runningNumeric.TryGetValue(mod.AttributeId, out current))
                    {
                        var numeric = entity.GetComponent<NumericComponent>();
                        current = numeric != null ? numeric.Get(mod.AttributeId, NumericSubType.Basic) : 0;
                    }
                }
                else
                {
                    if (!runningResource.TryGetValue(mod.AttributeId, out current))
                    {
                        var resource = entity.GetComponent<ResourceComponent>();
                        current = resource != null ? resource.Get(mod.AttributeId) : 0;
                    }
                }

                current = ApplyOp(current, mod.Op, mod.Magnitude);
                if (current < 0)
                {
                    return false;
                }

                if (channel == GameplayModifierChannel.ExecuteNumeric)
                {
                    runningNumeric[mod.AttributeId] = current;
                }
                else
                {
                    runningResource[mod.AttributeId] = current;
                }
            }

            return true;
        }

        private void TickDurations(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            using var snapshot = ReusableList<AbilitySystemComponent>.Create();
            snapshot.AddRange(m_Components);

            for (var c = 0; c < snapshot.Count; c++)
            {
                var asc = snapshot[c];
                if (!IsAlive(asc))
                {
                    continue;
                }

                for (var i = asc.ActiveEffects.Count - 1; i >= 0; i--)
                {
                    var effect = asc.ActiveEffects[i];
                    if (!m_Effects.TryGetValue(effect.EffectDefId, out var def) ||
                        def.DurationPolicy != DurationPolicy.Duration)
                    {
                        continue;
                    }

                    effect.TimeRemaining -= deltaTime;
                    if (effect.TimeRemaining <= 0f)
                    {
                        RemoveActiveEffectAt(asc.Entity, asc, i, true);
                    }
                }
            }
        }

        private void TickPeriods(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            using var snapshot = ReusableList<AbilitySystemComponent>.Create();
            snapshot.AddRange(m_Components);

            for (var c = 0; c < snapshot.Count; c++)
            {
                var asc = snapshot[c];
                if (!IsAlive(asc))
                {
                    continue;
                }

                for (var i = 0; i < asc.ActiveEffects.Count; i++)
                {
                    var effect = asc.ActiveEffects[i];
                    if (!effect.UsesPeriod || !m_Effects.TryGetValue(effect.EffectDefId, out var def))
                    {
                        continue;
                    }

                    effect.PeriodTimeRemaining -= deltaTime;
                    while (effect.PeriodTimeRemaining <= 0f)
                    {
                        ExecuteModifiers(asc.Entity, def, effect.Modifiers);
                        effect.PeriodTimeRemaining += def.Period > 0f ? def.Period : 1f;
                    }
                }
            }
        }

        private void RemoveActiveEffectAt(IGameEntity entity, AbilitySystemComponent asc, int index, bool expired)
        {
            var effect = asc.ActiveEffects[index];
            RemoveGrantedTags(asc, effect.GrantedTags, effect);
            asc.ActiveEffects.RemoveAt(index);

            if (m_Effects.TryGetValue(effect.EffectDefId, out var def))
            {
                RevokeAbilitiesFromEffect(entity, asc, def);
                EmitCues(entity, def, false);
                EmitLifecycle(
                    entity,
                    def,
                    effect.Handle,
                    effect.SourceEntity.GameEntity,
                    expired ? GameplayEffectLifecycleKind.Expired : GameplayEffectLifecycleKind.Removed,
                    false);
            }

            if (effect.AggregatesModifiers)
            {
                RecalculateAttributes(asc);
            }

            ReferencePool.Release(effect);
        }

        private void RecalculateAttributes(AbilitySystemComponent asc)
        {
            if (!IsAlive(asc))
            {
                return;
            }

            var hasOverlay = false;
            for (var i = 0; i < asc.ActiveEffects.Count; i++)
            {
                if (asc.ActiveEffects[i].AggregatesModifiers)
                {
                    hasOverlay = true;
                    break;
                }
            }

            var numeric = asc.GetComponent<NumericComponent>();
            if (numeric == null)
            {
                if (!hasOverlay)
                {
                    return;
                }

                numeric = EnsureNumeric(asc.Entity);
            }

            numeric.RemoveFromSource(asc);
            m_ScratchAdd.Clear();
            m_ScratchMul.Clear();
            m_ScratchOverride.Clear();

            for (var i = 0; i < asc.ActiveEffects.Count; i++)
            {
                var effect = asc.ActiveEffects[i];
                if (!effect.AggregatesModifiers || !m_Effects.TryGetValue(effect.EffectDefId, out var def))
                {
                    continue;
                }

                for (var m = 0; m < effect.Modifiers.Count; m++)
                {
                    var mod = effect.Modifiers[m];
                    if (mod.AttributeId == 0 || ResolveChannel(def, in mod) != GameplayModifierChannel.Overlay)
                    {
                        continue;
                    }

                    var subType = ResolveOverlaySubType(in mod);
                    if (mod.Op == ModifierOp.Override)
                    {
                        m_ScratchOverride[mod.AttributeId] = mod.Magnitude;
                        continue;
                    }

                    var key = NumericHelper.EncodeNumericKey(mod.AttributeId, subType);
                    if (mod.Op == ModifierOp.Multiply)
                    {
                        if (!m_ScratchMul.TryGetValue(key, out var product))
                        {
                            product = 100;
                        }

                        m_ScratchMul[key] = product * (100 + mod.Magnitude) / 100;
                    }
                    else
                    {
                        m_ScratchAdd.TryGetValue(key, out var add);
                        m_ScratchAdd[key] = add + mod.Magnitude;
                    }
                }
            }

            foreach (var pair in m_ScratchAdd)
            {
                var (id, subType) = NumericHelper.DecodeNumericKey(pair.Key);
                numeric.Set(id, subType, pair.Value, asc);
            }

            foreach (var pair in m_ScratchMul)
            {
                var (id, subType) = NumericHelper.DecodeNumericKey(pair.Key);
                numeric.Set(id, subType, pair.Value - 100, asc);
            }

            foreach (var pair in m_ScratchOverride)
            {
                numeric.Set(pair.Key, NumericSubType.Override, pair.Value, asc);
            }

            m_ScratchAdd.Clear();
            m_ScratchMul.Clear();
            m_ScratchOverride.Clear();
        }

        private void ExecuteModifiers(IGameEntity entity, GameplayEffectDef def, IReadOnlyList<GameplayEffectModifier> modifiers)
        {
            if (entity == null || modifiers == null || modifiers.Count == 0)
            {
                return;
            }

            for (var i = 0; i < modifiers.Count; i++)
            {
                var mod = modifiers[i];
                if (mod.AttributeId == 0)
                {
                    continue;
                }

                var channel = ResolveChannel(def, in mod);
                if (channel == GameplayModifierChannel.Overlay)
                {
                    continue;
                }

                if (channel == GameplayModifierChannel.ExecuteNumeric)
                {
                    var numeric = EnsureNumeric(entity);
                    var current = numeric.Get(mod.AttributeId, NumericSubType.Basic);
                    var next = ApplyOp(current, mod.Op, mod.Magnitude);
                    numeric.Modify(mod.AttributeId, NumericSubType.Basic, next - current, this);
                }
                else
                {
                    var resource = EnsureResource(entity);
                    var current = resource.Get(mod.AttributeId);
                    resource.Set(mod.AttributeId, ApplyOp(current, mod.Op, mod.Magnitude));
                }
            }
        }

        private static long ApplyOp(long current, ModifierOp op, long magnitude)
        {
            switch (op)
            {
                case ModifierOp.Add:
                    return current + magnitude;
                case ModifierOp.Multiply:
                    return current * (100 + magnitude) / 100;
                case ModifierOp.Override:
                    return magnitude;
                default:
                    return current;
            }
        }

        private static int FindActiveEffect(AbilitySystemComponent asc, int effectDefId)
        {
            for (var i = 0; i < asc.ActiveEffects.Count; i++)
            {
                if (asc.ActiveEffects[i].EffectDefId == effectDefId)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int CountActiveEffects(AbilitySystemComponent asc, int effectDefId)
        {
            var count = 0;
            for (var i = 0; i < asc.ActiveEffects.Count; i++)
            {
                if (asc.ActiveEffects[i].EffectDefId == effectDefId)
                {
                    count++;
                }
            }

            return count;
        }

        private static int FindSoonestExpiring(AbilitySystemComponent asc, int effectDefId)
        {
            var best = -1;
            var bestRemaining = float.PositiveInfinity;
            for (var i = 0; i < asc.ActiveEffects.Count; i++)
            {
                var effect = asc.ActiveEffects[i];
                if (effect.EffectDefId != effectDefId)
                {
                    continue;
                }

                if (effect.TimeRemaining < bestRemaining)
                {
                    bestRemaining = effect.TimeRemaining;
                    best = i;
                }
            }

            return best >= 0 ? best : 0;
        }

        private static void RefreshActiveEffect(AbilitySystemComponent asc, int index, GameplayEffectDef def)
        {
            var existing = asc.ActiveEffects[index];
            if (def.DurationPolicy == DurationPolicy.Duration)
            {
                existing.TimeRemaining = def.Duration;
            }

            if (existing.UsesPeriod)
            {
                existing.PeriodTimeRemaining = def.Period;
            }
        }

        private bool EffectGrantsMatchingTag(ActiveGameplayEffect effect, GameplayTag tag)
        {
            for (var g = 0; g < effect.GrantedTags.Count; g++)
            {
                if (TagRegistry.Matches(effect.GrantedTags[g], tag))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasActiveEffectDef(AbilitySystemComponent asc, int effectDefId)
        {
            return FindActiveEffect(asc, effectDefId) >= 0;
        }
    }
}
