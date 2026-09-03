# 技能系统（GAS）

## 职责

为一段玩法会话提供技能授予/激活、游戏效果施加与持续，以及层次化 Tag 门闩。会话级规则在 Manager，实体侧状态在组件。数值仍走 `NumericComponent` / `ResourceComponent`，不另做 AttributeSet，也不引入第二套 EC。

## 关键类型

- `IGameAbilitySystemManager` / `GameAbilitySystemManager`：Tag 与 Def 注册表，Grant / Activate / Apply，FixedUpdate 驱动 duration、period、ability lifetime
- `AbilitySystemComponent`：OwnedTags、BlockedAbilityTags、已授予/激活技能、生效中效果；Awake 时向 Manager 自注册
- `GameplayTag` / `GameplayTagRegistry` / `GameplayTagContainer`：点分名层次标签与引用计数
- `GameplayAbilityDef` / `GameplayEffectDef`：数据模板（Cost/Cooldown GE、Stacking、Immunity、Interrupt 等）
- `IGameplayAbility`：`InstantGameplayAbility`、`DurationGameplayAbility`、`TaskGameplayAbility`
- `AbilityTask`：`WaitSecondsTask`、`WaitUntilTagTask`、`ApplyEffectTask`
- `GameplayCueEventArgs` / `GameplayEffectLifecycleEventArgs`：表现 Cue 与逻辑层生命周期，走 `Context.EventDispatcher`

修正通道 `GameplayModifierChannel`：

- Instant 或 `Period > 0`（Auto）→ Execute，默认改 `ResourceComponent`
- Duration/Infinite 且无 Period（Auto）→ Overlay，以 ASC 为 source 写入 Numeric 现有子项（不另开 Gas 层）
- Overlay 默认：Add → `BasicConstAdd`，Multiply → `BasicPercent`，Override → `Override`；可用 `GameplayEffectModifier.NumericSubType` 改到 Final 层
- 可显式指定 `ExecuteNumeric`（改 Basic）或 `ExecuteResource`

Magnitude 为 `long`，百分比与 Numeric 一致（20 = +20%）。

## 用法

Context `OnAwake` 里在 `GameUpdateManager` 之后 `AddManager<GameAbilitySystemManager>()`。实体 `AddComponent<AbilitySystemComponent>()`。

```csharp
var gas = context.GetManager<IGameAbilitySystemManager>();
var stun = gas.Tags.Request("State.Debuff.Stun");
gas.RegisterEffect(new GameplayEffectDef { /* Duration + GrantedTags + Overlay modifiers */ });
gas.RegisterAbility(new GameplayAbilityDef { AbilityLogicType = typeof(MyInstantAbility) });

var entity = context.GetManager<IGameEntityManager>().CreateEntity();
entity.AddComponent<AbilitySystemComponent>();
gas.GrantAbility(entity, abilityId);
gas.TryActivateAbility(entity, abilityId, target);
gas.ApplyEffect(target, new EffectSpec(effectId, entity));
```

`TargetRelation` 只是 Def 数据，空间选敌由调用方完成后再传入 target。

## 扩展点

- 新技能：实现 `IGameplayAbility`（或三个模板之一），在 Def 上填 `AbilityLogicType`
- 新 Task：继承 `AbilityTask`，在 `TaskGameplayAbility.BuildTasks` 里组装
- Cue：订阅 `GameplayCueEventArgs`，不要在技能逻辑里播 VFX
- 表现 Converter 与 Luban 表不在本模块范围内，Def 目前代码注册

## 约束与坑

- 必须同时挂 `IGameUpdateManager`，否则 Manager `Awake` 无法注册 FixedUpdate。Pause 时 UpdateManager 不推进，效果计时自动停。
- 不要用 Duration 技能模拟眩晕/沉默/中毒，那些是 Duration 效果。
- Tag 与 `StateComponent` 不合并。效果授予 Tag 时，若实体上有 State，会用效果/技能实例当 provider 做 `AddState(tag.Id, provider)`。Tag 注册表 id 与业务自己的 stateId 可能冲突，桥接时共用 Tag.Id。
- Overlay 以 ASC 实例为 Numeric 来源；实体销毁会 `RemoveFromSource`。
- TypeId / Tag id / Def id 都不要当跨进程协议号，除非 Def id 由你自己稳定分配。
- 效果时长走 GAS Tick，不要再挂一份 `IGameTimerManager`，避免双通道。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Ability/`
- `Client/Assets/GameMain/Scripts/Gameplay/Common/Ability/`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Event/GameplayCueEventArgs.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Event/GameplayEffectLifecycleEventArgs.cs`
