# 启动链

## 入口

`GameEntry`（`Client/Assets/GameMain/Scripts/GameEntry.cs`）挂在启动场景：

1. `Awake`：把序列化的 `GameConfig` 赋给 `GameConfig.Instance`，`new GameApp()`。
2. `Start`：组装 `ProcedureBase[]`，调用 `ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures)`。
3. `Update`：`m_GameApp.Update(Time.deltaTime, Time.unscaledDeltaTime)`，驱动所有模块 `OnUpdate`。
4. `OnDestroy`：`m_GameApp.Destroy()`，模块按注册**逆序**销毁。

`GameApp` 是单例；重复构造会抛 `FrameworkErrorCode.InvalidOperationException`。`RestartGame()` 加载场景 0。

## 模块注册顺序

`GameApp` 构造函数里 `AddModule` 会立刻 `Init()`。顺序（后者可依赖前者）：

1. `IOCModule`
2. `BaseModule`（`UseUnityJsonHelper`、`UseDefaultTextTemplateHelper`）
3. `EventModule`
4. `RemoteRequestModule`
5. `NetworkModule`
6. `WebRequestModule`（`UseUnityWebRequest`）
7. `TimerModule`
8. `SettingModule`（`UsePlayerPrefsSetting`）
9. `SafeAreaModule`
10. `ResModule`（`UseYooAsset`）
11. `SceneModule`
12. `CameraModule`
13. `SoundModule`（`UseAudioClipSound`）
14. `GameObjectPoolModule`
15. `DataTableModule`
16. `LocalizationModule`
17. `BehaviorModule`
18. `SystemModule`
19. `UIModule`
20. `ProcedureModule`

辅助器（Json、YooAsset、WebRequest 等）在 `AddModule` 的链式扩展上设置。未设置核心加载器时，`ResModule.CreateResLoader` 会抛 `ResCoreLoaderNotSet`。

## 业务流程

`ProcedureId`（`Client/Assets/GameMain/Scripts/Procedure/ProcedureId.cs`）：

1. `Splash`
2. `LoadDataTable`
3. `LoadLocalization`
4. `InitSystem`
5. `Login`

`ProcedureBase.ChangeNextProcedure()` 依赖 **Id 连续 +1**。新增流程要同时改枚举、`GameEntry` 数组和切换逻辑。

流程只能 `Launch` 一次；重复启动抛 `ProcedureAlreadyLaunch`。切换必须从流程实例走 `ChangeProcedure`。

## 相关源码

- `Client/Assets/GameMain/Scripts/GameEntry.cs`
- `Client/Assets/HoweFramework/GameApp.cs`
- `Client/Assets/HoweFramework/Base/ModuleBase.cs`
- `Client/Assets/HoweFramework/Procedure/ProcedureModule.cs`
- `Client/Assets/GameMain/Scripts/Procedure/`
