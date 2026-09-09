# 启动链

## 入口

`GameEntry`（`Client/Assets/GameMain/Scripts/GameEntry.cs`）挂在启动场景：

1. `Awake`：把序列化的 `GameConfig` 赋给 `GameConfig.Instance`，`new GameApp()`。
2. `Start`：组装 `ProcedureBase[]`，调用 `ProcedureModule.Instance.Launch<ProcedureSplash>(procedures)`。
3. `Update`：`m_GameApp.Update(Time.deltaTime, Time.unscaledDeltaTime)`，驱动所有模块 `OnUpdate`。
4. `OnDestroy`：`m_GameApp.Destroy()`，模块按注册**逆序**销毁。

`GameApp` 是单例；重复构造会抛 `FrameworkErrorCode.InvalidOperationException`。构造中途某模块 `Init` 失败会销毁已注册模块并清空单例。`RestartGame()` 加载场景 0。

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

`GameEntry` 里 Launch 数组的顺序就是启动链（id 为各具体类型的运行时 `TypeId`）：

1. `ProcedureSplash`
2. `ProcedureLoadDataTable`
3. `ProcedureLoadLocalization`
4. `ProcedureInitSystem`
5. `ProcedureLogin`

`ProcedureBase.ChangeNextProcedure()` 按 **Launch 数组顺序** 前进，与 TypeId 数值无关。跳到指定流程用 `ChangeProcedure<T>()`。新增流程只改 `GameEntry` 数组（以及需要时的 `ChangeProcedure<T>`），不要再写流程枚举。

流程只能 `Launch` 一次；重复启动抛 `ProcedureAlreadyLaunch`。切换必须从流程实例走 `ChangeProcedure`。

## 相关源码

- `Client/Assets/GameMain/Scripts/GameEntry.cs`
- `Client/Assets/HoweFramework/GameApp.cs`
- `Client/Assets/HoweFramework/Base/ModuleBase.cs`
- `Client/Assets/HoweFramework/Procedure/ProcedureModule.cs`
- `Client/Assets/GameMain/Scripts/Procedure/`
