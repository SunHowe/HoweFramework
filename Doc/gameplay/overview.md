# Gameplay 概览

源码全部在 `Client/Assets/GameMain/Scripts/Gameplay/`，命名空间 `GameMain`。框架只提供事件、资源、对象池等服务，Context 在 `Awake` 时按需创建托管的 `IEventDispatcher` / `IResLoader` / `IGameObjectPool`。

```
Gameplay/
├── Framework/     Context、Entity、Managers、Event、Expression、Helper
├── Common/        Transform、View、Numeric、State、Resource
├── Mono/          GameEntityConverter 与各 ComponentConverter
└── Doc/           仅指针，正文在仓库根 Doc/
```

业务玩法组件继续放在 `GameMain/Scripts/` 下自建目录，不要塞进 `HoweFramework`。

## 组合方式

一个玩法会话 = 一个 `GameContextBase` 子类：`AddManager` 挂 Manager，用 `IGameEntityManager.CreateEntity()` 建实体，再 `entity.AddComponent<T>()`。

生命周期：`None → Initialize(Awake) → Running(StartGame) → Pause/Resume → Stopped(StopGame) → Dispose`。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Framework/GameContextBase.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/IGameContext.cs`
