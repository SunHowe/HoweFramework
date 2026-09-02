# 实体与组件

## 职责

`GameEntityManager` 分配实体与组件 id，实体挂唯一类型的组件。组件从 `ReferencePool` 取。

## 关键类型

- `IGameEntity` / `GameEntity`（`GameEntityManager` 嵌套实现）
- `IGameEntityManager`：`CreateEntity`、`GetEntity`、`DestroyEntity`、`SpawnComponentId`
- `IGameComponent` / `GameComponentBase`：`ComponentType` 为 `TypeId.GetIdByType` 分配的运行时 id（具体组件类）
- `GameEntityHelper`：`AddComponent<T>`、`GetComponent<T>`、`RemoveComponent<T>`

`GetComponentType` 只接受 `GameComponentBase` 的非抽象派生类。id 随进程分配，不要当协议号或表字段持久化。

## 用法

```csharp
var entity = context.GetManager<IGameEntityManager>().CreateEntity();
var numeric = entity.AddComponent<NumericComponent>();
entity.GetComponent<NumericComponent>();
entity.Dispose(); // 或 DestroyEntity
```

`GameComponentBase.Dispose` 若仍挂在实体上会先 `RemoveComponent`。实体销毁时 `DisposeFromEntity` 再还池。

## 扩展点

业务组件：继承 `GameComponentBase`，不必再标 Attribute 或占枚举。需要场景摆放再写 Converter。

## 约束与坑

- 同一 `ComponentType` 在一个实体上只有一份（以 Entity 实现为准）。
- 组件 `Clear` 目前是空实现，状态应在 `OnDispose` 清掉。
- 不要跳过 Manager `new` 实体当长期对象。
- `GameEntityHelper.GetGameEntity(GameObject)` 找父级 `BlackboardComponent` 的 `"GameEntity"`。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameEntityManager.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameComponentBase.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Helper/GameEntityHelper.cs`
- `Client/Assets/HoweFramework/Base/TypeId.cs`
