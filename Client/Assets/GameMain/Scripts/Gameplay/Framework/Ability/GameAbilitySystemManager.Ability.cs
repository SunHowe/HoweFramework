using System;
using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    public sealed partial class GameAbilitySystemManager
    {
        public void GrantAbility(IGameEntity entity, int abilityDefId, int level = 1)
        {
            if (!m_Abilities.ContainsKey(abilityDefId))
            {
                throw new InvalidOperationException(string.Format("Unknown ability def {0}.", abilityDefId));
            }

            var asc = EnsureAbilitySystem(entity);
            if (asc == null)
            {
                return;
            }

            for (var i = 0; i < asc.GrantedAbilities.Count; i++)
            {
                if (asc.GrantedAbilities[i].AbilityDefId == abilityDefId)
                {
                    return;
                }
            }

            asc.GrantedAbilities.Add(new GrantedAbility(abilityDefId, level));
        }

        public bool SetAbilityLevel(IGameEntity entity, int abilityDefId, int level)
        {
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return false;
            }

            var clamped = level < 1 ? 1 : level;
            for (var i = 0; i < asc.GrantedAbilities.Count; i++)
            {
                var granted = asc.GrantedAbilities[i];
                if (granted.AbilityDefId != abilityDefId)
                {
                    continue;
                }

                granted.Level = clamped;
                asc.GrantedAbilities[i] = granted;
                return true;
            }

            return false;
        }

        public bool TryGetAbilityLevel(IGameEntity entity, int abilityDefId, out int level)
        {
            level = 1;
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return false;
            }

            for (var i = 0; i < asc.GrantedAbilities.Count; i++)
            {
                if (asc.GrantedAbilities[i].AbilityDefId != abilityDefId)
                {
                    continue;
                }

                level = asc.GrantedAbilities[i].Level;
                return true;
            }

            return false;
        }

        public bool ClearAbility(IGameEntity entity, int abilityDefId)
        {
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return false;
            }

            CancelAbilitiesByDef(entity, asc, abilityDefId);

            for (var i = asc.GrantedAbilities.Count - 1; i >= 0; i--)
            {
                if (asc.GrantedAbilities[i].AbilityDefId != abilityDefId)
                {
                    continue;
                }

                asc.GrantedAbilities.RemoveAt(i);
                return true;
            }

            return false;
        }

        public AbilityActivationResult TryActivateAbility(IGameEntity entity, int abilityDefId, IGameEntity target = null)
        {
            if (!m_Abilities.TryGetValue(abilityDefId, out var def))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.UnknownAbility);
            }

            var asc = GetAsc(entity);
            if (asc == null)
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.MissingAsc);
            }

            var abilityLevel = 1;
            var granted = false;
            for (var i = 0; i < asc.GrantedAbilities.Count; i++)
            {
                if (asc.GrantedAbilities[i].AbilityDefId != abilityDefId)
                {
                    continue;
                }

                granted = true;
                abilityLevel = asc.GrantedAbilities[i].Level < 1 ? 1 : asc.GrantedAbilities[i].Level;
                break;
            }

            if (!granted)
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.NotGranted);
            }

            if (def.InstancingPolicy == AbilityInstancingPolicy.Single)
            {
                for (var i = 0; i < asc.ActiveAbilities.Count; i++)
                {
                    if (asc.ActiveAbilities[i].AbilityDefId == abilityDefId)
                    {
                        return AbilityActivationResult.Fail(AbilityActivationFailReason.AlreadyActive);
                    }
                }
            }

            if (def.ActivationRequiredTags != null &&
                def.ActivationRequiredTags.Count > 0 &&
                !asc.OwnedTags.HasAll(def.ActivationRequiredTags, TagRegistry))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.MissingRequiredTags);
            }

            if (def.ActivationBlockedTags != null &&
                def.ActivationBlockedTags.Count > 0 &&
                asc.OwnedTags.HasAny(def.ActivationBlockedTags, TagRegistry))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.BlockedByTags);
            }

            if (def.AbilityTags != null &&
                def.AbilityTags.Count > 0 &&
                asc.BlockedAbilityTags.HasAny(def.AbilityTags, TagRegistry))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.BlockedByTags);
            }

            if (def.CooldownEffectId != 0 && HasActiveEffectDef(asc, def.CooldownEffectId))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.OnCooldown);
            }

            if (def.CooldownGroupTag.IsValid && asc.OwnedTags.HasExact(def.CooldownGroupTag))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.OnCooldown);
            }

            if (def.CostEffectId != 0 && !CanAffordCost(entity, def.CostEffectId))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.CannotAffordCost);
            }

            if (target != null && !PassesTargetTagFilter(target, def))
            {
                return AbilityActivationResult.Fail(AbilityActivationFailReason.TargetRejected);
            }

            CancelAbilitiesWithTags(entity, asc, def.CancelAbilitiesWithTag);

            if (def.CostEffectId != 0)
            {
                ApplyEffect(entity, new EffectSpec(def.CostEffectId, entity, abilityLevel));
            }

            if (def.CooldownEffectId != 0)
            {
                ApplyEffect(entity, new EffectSpec(def.CooldownEffectId, entity, abilityLevel));
            }

            var active = ActiveAbility.Create();
            active.Handle = asc.NextAbilityHandle++;
            active.AbilityDefId = abilityDefId;
            active.Level = abilityLevel;
            active.TargetEntity = new GameEntityRef(target);
            active.Logic = ResolveAbilityLogic(def);
            if (def.ActivationOwnedTags != null)
            {
                active.OwnedTags.AddRange(def.ActivationOwnedTags);
            }

            if (def.BlockAbilitiesWithTag != null)
            {
                active.BlockedAbilityTags.AddRange(def.BlockAbilitiesWithTag);
            }

            asc.ActiveAbilities.Add(active);
            AddGrantedTags(asc, active.OwnedTags, active);
            asc.BlockedAbilityTags.AddTags(active.BlockedAbilityTags);

            var context = new AbilityActivationContext(entity, abilityDefId, active.Handle, this, target, abilityLevel);
            active.Logic?.Activate(in context);

            return AbilityActivationResult.Ok(active.Handle);
        }

        public bool CancelAbility(IGameEntity entity, int activeHandle)
        {
            return EndAbility(entity, activeHandle, true);
        }

        public bool EndAbility(IGameEntity entity, int activeHandle, bool wasCancelled = false)
        {
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return false;
            }

            for (var i = 0; i < asc.ActiveAbilities.Count; i++)
            {
                var active = asc.ActiveAbilities[i];
                if (active.Handle != activeHandle || active.IsEnding)
                {
                    continue;
                }

                active.IsEnding = true;
                var context = new AbilityActivationContext(
                    entity,
                    active.AbilityDefId,
                    active.Handle,
                    this,
                    active.TargetEntity.GameEntity,
                    active.Level);

                for (var t = 0; t < active.Tasks.Count; t++)
                {
                    active.Tasks[t].OnEnd(in context, wasCancelled);
                }

                active.Logic?.OnEnd(in context, wasCancelled);

                RemoveGrantedTags(asc, active.OwnedTags, active);
                asc.BlockedAbilityTags.RemoveTags(active.BlockedAbilityTags);
                asc.ActiveAbilities.RemoveAt(i);
                ReferencePool.Release(active);
                return true;
            }

            return false;
        }

        public bool TryInterruptAbility(IGameEntity entity, int activeHandle, GameplayTag interruptTag = default)
        {
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return false;
            }

            for (var i = 0; i < asc.ActiveAbilities.Count; i++)
            {
                var active = asc.ActiveAbilities[i];
                if (active.Handle != activeHandle || active.IsEnding)
                {
                    continue;
                }

                if (!m_Abilities.TryGetValue(active.AbilityDefId, out var def) || def.Uninterruptible)
                {
                    return false;
                }

                if (def.InterruptTags != null && def.InterruptTags.Count > 0)
                {
                    if (!interruptTag.IsValid)
                    {
                        return false;
                    }

                    var matches = false;
                    for (var t = 0; t < def.InterruptTags.Count && !matches; t++)
                    {
                        matches = TagRegistry.Matches(interruptTag, def.InterruptTags[t])
                                  || TagRegistry.Matches(def.InterruptTags[t], interruptTag);
                    }

                    if (!matches)
                    {
                        return false;
                    }
                }

                return EndAbility(entity, activeHandle, true);
            }

            return false;
        }

        public void SetAbilityDuration(IGameEntity entity, int activeHandle, float duration)
        {
            if (!TryGetActiveAbility(entity, activeHandle, out var active))
            {
                return;
            }

            active.RemainingDuration = duration < 0f ? 0f : duration;
        }

        public void AddAbilityTasks(IGameEntity entity, int activeHandle, IReadOnlyList<AbilityTask> tasks)
        {
            if (!TryGetActiveAbility(entity, activeHandle, out var active) || tasks == null)
            {
                return;
            }

            active.Tasks.AddRange(tasks);
        }

        private void TickAbilities(float deltaTime)
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

                for (var i = asc.ActiveAbilities.Count - 1; i >= 0; i--)
                {
                    var active = asc.ActiveAbilities[i];
                    if (active.IsEnding)
                    {
                        continue;
                    }

                    var autoEnd = false;
                    var cancelled = false;

                    if (active.RemainingDuration >= 0f)
                    {
                        active.RemainingDuration -= deltaTime;
                        if (active.RemainingDuration <= 0f)
                        {
                            autoEnd = true;
                        }
                    }

                    if (!autoEnd && active.Tasks.Count > 0)
                    {
                        var context = new AbilityActivationContext(
                            asc.Entity,
                            active.AbilityDefId,
                            active.Handle,
                            this,
                            active.TargetEntity.GameEntity,
                            active.Level);

                        var allComplete = true;
                        for (var t = 0; t < active.Tasks.Count; t++)
                        {
                            var task = active.Tasks[t];
                            if (task.IsComplete)
                            {
                                if (task.IsCancelled)
                                {
                                    cancelled = true;
                                }

                                continue;
                            }

                            task.StartOnce(in context);
                            if (task.IsComplete)
                            {
                                if (task.IsCancelled)
                                {
                                    cancelled = true;
                                }

                                continue;
                            }

                            task.Tick(in context, deltaTime);
                            if (!task.IsComplete)
                            {
                                allComplete = false;
                                break;
                            }

                            if (task.IsCancelled)
                            {
                                cancelled = true;
                            }
                        }

                        if (allComplete)
                        {
                            autoEnd = true;
                        }
                    }

                    if (autoEnd)
                    {
                        EndAbility(asc.Entity, active.Handle, cancelled);
                    }
                }
            }
        }

        private bool TryGetActiveAbility(IGameEntity entity, int activeHandle, out ActiveAbility active)
        {
            active = null;
            var asc = GetAsc(entity);
            if (asc == null)
            {
                return false;
            }

            for (var i = 0; i < asc.ActiveAbilities.Count; i++)
            {
                if (asc.ActiveAbilities[i].Handle != activeHandle)
                {
                    continue;
                }

                active = asc.ActiveAbilities[i];
                return true;
            }

            return false;
        }

        private bool PassesTargetTagFilter(IGameEntity target, GameplayAbilityDef def)
        {
            var targetAsc = GetAsc(target);
            if (def.TargetRequiredTags != null && def.TargetRequiredTags.Count > 0)
            {
                if (targetAsc == null || !targetAsc.OwnedTags.HasAll(def.TargetRequiredTags, TagRegistry))
                {
                    return false;
                }
            }

            if (def.TargetBlockedTags != null && def.TargetBlockedTags.Count > 0)
            {
                if (targetAsc != null && targetAsc.OwnedTags.HasAny(def.TargetBlockedTags, TagRegistry))
                {
                    return false;
                }
            }

            return true;
        }

        private void GrantAbilitiesFromEffect(IGameEntity entity, GameplayEffectDef def, int level)
        {
            if (def.GrantedAbilityDefIds == null || def.GrantedAbilityDefIds.Count == 0)
            {
                return;
            }

            var abilityLevel = level < 1 ? 1 : level;
            for (var i = 0; i < def.GrantedAbilityDefIds.Count; i++)
            {
                GrantAbility(entity, def.GrantedAbilityDefIds[i], abilityLevel);
            }
        }

        private void RevokeAbilitiesFromEffect(IGameEntity entity, AbilitySystemComponent asc, GameplayEffectDef def)
        {
            if (def.GrantedAbilityDefIds == null || def.GrantedAbilityDefIds.Count == 0)
            {
                return;
            }

            for (var i = 0; i < def.GrantedAbilityDefIds.Count; i++)
            {
                var abilityDefId = def.GrantedAbilityDefIds[i];
                if (AnyActiveEffectGrantsAbility(asc, abilityDefId))
                {
                    continue;
                }

                ClearAbility(entity, abilityDefId);
            }
        }

        private bool AnyActiveEffectGrantsAbility(AbilitySystemComponent asc, int abilityDefId)
        {
            for (var e = 0; e < asc.ActiveEffects.Count; e++)
            {
                var effect = asc.ActiveEffects[e];
                if (!m_Effects.TryGetValue(effect.EffectDefId, out var def) || def.GrantedAbilityDefIds == null)
                {
                    continue;
                }

                for (var g = 0; g < def.GrantedAbilityDefIds.Count; g++)
                {
                    if (def.GrantedAbilityDefIds[g] == abilityDefId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CancelAbilitiesWithTags(IGameEntity entity, AbilitySystemComponent asc, IReadOnlyList<GameplayTag> tags)
        {
            if (tags == null || tags.Count == 0)
            {
                return;
            }

            for (var i = asc.ActiveAbilities.Count - 1; i >= 0; i--)
            {
                var active = asc.ActiveAbilities[i];
                if (!m_Abilities.TryGetValue(active.AbilityDefId, out var def) || def.AbilityTags == null)
                {
                    continue;
                }

                var shouldCancel = false;
                for (var t = 0; t < tags.Count && !shouldCancel; t++)
                {
                    for (var a = 0; a < def.AbilityTags.Count; a++)
                    {
                        if (TagRegistry.Matches(def.AbilityTags[a], tags[t]) || def.AbilityTags[a] == tags[t])
                        {
                            shouldCancel = true;
                            break;
                        }
                    }
                }

                if (shouldCancel)
                {
                    EndAbility(entity, active.Handle, true);
                }
            }
        }

        private void CancelAbilitiesByDef(IGameEntity entity, AbilitySystemComponent asc, int abilityDefId)
        {
            for (var i = asc.ActiveAbilities.Count - 1; i >= 0; i--)
            {
                if (asc.ActiveAbilities[i].AbilityDefId == abilityDefId)
                {
                    EndAbility(entity, asc.ActiveAbilities[i].Handle, true);
                }
            }
        }

        private IGameplayAbility ResolveAbilityLogic(GameplayAbilityDef def)
        {
            if (def.AbilityLogicType == null)
            {
                return null;
            }

            if (m_AbilityLogicCache.TryGetValue(def.AbilityLogicType, out var cached))
            {
                return cached;
            }

            if (!typeof(IGameplayAbility).IsAssignableFrom(def.AbilityLogicType))
            {
                throw new InvalidOperationException(
                    string.Format("Ability logic type {0} does not implement {1}.", def.AbilityLogicType, nameof(IGameplayAbility)));
            }

            var instance = (IGameplayAbility)Activator.CreateInstance(def.AbilityLogicType);
            m_AbilityLogicCache[def.AbilityLogicType] = instance;
            return instance;
        }
    }
}
