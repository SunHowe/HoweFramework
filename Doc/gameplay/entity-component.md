# 实体与组件

## 职责

`GameEntityManager` 分配实体与组件 id，实体挂唯一类型的组件。组件从 `ReferencePool` 取。

## 关键类型

- `IGameEntity` / `GameEntity`（`GameEntityManager` 嵌套实现）
- `IGameEntityManager`：`CreateEntity`、`GetEntity`、`DestroyEntity`、`SpawnComponentId`
- `IGameComponent` / `GameComponentBase`：`ComponentType` 来自类上 `GameComponentAttribute`
- `GameEntityHelper`：`AddComponent<T>`、`GetComponent<T>`、`RemoveComponent<T>`
- `GameComponentType` 通用区注释为 **1–1000**：Transform=1、View=2、ViewTransformSync=3、Numeric=4、State=5、Resource=6

组件类必须 `[GameComponent(GameComponentType.Xxx)]`，否则 `GetComponentType` 抛异常。

## 用法

```csharp
var entity = context.GetManager<IGameEntityManager>().CreateEntity();
var numeric = entity.AddComponent<NumericComponent>();
entity.GetComponent<NumericComponent>();
entity.Dispose(); // 或 DestroyEntity
```

`GameComponentBase.Dispose` 若仍挂在实体上会先 `RemoveComponent`。实体销毁时 `DisposeFromEntity` 再还池。

## 扩展点

业务组件：枚举从 **1001** 起（避开通用 1–1000），新类 + Attribute，需要场景摆放再写 Converter。

## 约束与坑

- 同一 `ComponentType` 在一个实体上只有一份（以 Entity 实现为准）。
- 组件 `Clear` 目前是空实现，状态应在 `OnDispose` 清掉。
- 不要跳过 Manager `new` 实体当长期对象。
- `GameEntityHelper.GetGameEntity(GameObject)` 找父级 `BlackboardComponent` 的 `"GameEntity"`。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameEntityManager.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameComponentBase.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameComponentAttribute.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/GameComponentType.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Helper/GameEntityHelper.cs`
