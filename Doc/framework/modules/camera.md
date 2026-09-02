# CameraModule

## 职责

跟踪 `GameCamera` 列表，维护 `MainCamera`。初始化时记下当前 `Camera.main`。

## 关键类型

- `CameraModule`：`MainCamera`；内部 `RegisterCamera` / `UnregisterCamera`
- `GameCamera`：`Priority`（默认 0）；OnEnable 注册、OnDisable 注销

列表变化后在 Update 里按 `Priority` 选出启用的主相机；没有控制器时回退构造时记下的 `Camera.main`。

## 用法

在相机物体上挂 `GameCamera` 并设 `Priority`。不要绕过模块长期改 `Camera.main`。

## 扩展点

多相机并存时只靠 `Priority` 竞选，不要再写一套主相机管理器。

## 约束与坑

- `RegisterCamera` 是 internal，从 `GameCamera` 生命周期调用。
- 与玩法 `GameViewManager` 不是同一层。

## 相关源码

- `Client/Assets/HoweFramework/Camera/CameraModule.cs`
- `Client/Assets/HoweFramework/Camera/GameCamera.cs`
