# View 与 Transform

## TransformComponent

逻辑坐标：`Position` / `EulerAngles` / `Scale` 与对应 Updated 事件。默认位姿 0,0,0 / 0,0,0 / 1,1,1。不直接驱动 Unity Transform。

## ViewComponent

`ResKey` 变化且可见时，经 `IViewObject` 加载/卸载。`OnViewLoaded` 若已加载会立刻回调。可见性 `IsVisible` 控制是否真正加载。

依赖 `GameViewManager` 提供 ViewObject。

## ViewTransformSyncComponent

把 `TransformComponent` 同步到视图 GameObject，可暂停、可插值。需要实体上同时有 Transform 与 View，并注册 `IGameUpdateManager` 的 LateUpdate。

## 用法

场景摆放用 Converter；运行时设 `view.ResKey`。逻辑移动只改 `TransformComponent`，让 Sync 写到显示。

## 约束与坑

- 没有 ViewManager 时不要加 ViewComponent。
- 卸载后仍订阅 `OnViewLoaded` 要自己配对卸载事件。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Common/Transform/TransformComponent.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Common/View/ViewComponent.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Common/View/ViewTransformSyncComponent.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/View/GameViewManager.cs`
