# Mono 桥接

## 职责

把场景上的 MonoBehaviour 配置转成实体组件。不把 MonoBehaviour 当玩法组件。

## 关键类型

- `GameEntityConverter.Convert(IGameEntity)`：收集 `IGameComponentConverter`，按 `SortingOrder` 排序后依次 `Convert`
- 已有：`TransformComponentConverter`、`ViewComponentConverter`、`NumericComponentConverter`、`StateComponentConverter`
- `GameEntityConverterExtensions`

从场景找实体：`GameObject.GetGameEntity()` → 父级 `BlackboardComponent["GameEntity"]`。射线：`GameEntityHelper.RaycastGameEntity`。

## 用法

Prefab 挂 Converter + 各 ComponentConverter。运行时 `CreateEntity` 后对根物体 `GetComponent<GameEntityConverter>().Convert(entity)`（以调用方扩展方法为准）。

## 扩展点

新组件若要可摆进场景：实现 `IGameComponentConverter`，控制 `SortingOrder`（依赖 Transform 的应更早）。

## 约束与坑

- Converter 只在转换那一刻跑，之后改 Inspector 不会自动写回实体。
- 不要在 Converter 里 new 不还池的组件。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Mono/GameEntityConverter.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Mono/IGameComponentConverter.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Helper/GameEntityHelper.cs`
