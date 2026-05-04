# 框架架构

## 概述

HoweFramework 是一个基于 Unity 的模块化游戏框架，采用**单例模式 + IOC 注入**的混合架构。框架核心提供 20+ 功能模块，游戏业务构建于框架之上。

## 模块依赖关系

```
GameApp (入口)
    │
    ├── IOCModule (依赖注入容器)
    │
    ├── EventModule (事件系统)
    │
    ├── ProcedureModule (流程管理)
    │       └──依赖 StateMachine
    │
    ├── UIModule (UI 界面)
    │       └──依赖 EventModule, ResModule
    │
    ├── ResModule (资源管理)
    │
    ├── NetworkModule (网络)
    │
    └── [其他 15+ 模块]
```

## 模块初始化顺序

`GameApp` 构造函数按以下顺序初始化所有模块：

| 顺序 | 模块 | 说明 |
|------|------|------|
| 1 | IOCModule | IOC 容器的初始化 |
| 2 | BaseModule | 基础工具（Json、文本模板） |
| 3 | EventModule | 事件系统 |
| 4 | RemoteRequestModule | 远程请求调度器 |
| 5 | NetworkModule | TCP 网络 |
| 6 | WebRequestModule | HTTP 请求 |
| 7 | TimerModule | 计时器 |
| 8 | SettingModule | 设置（PlayerPrefs） |
| 9 | SafeAreaModule | 安全区域 |
| 10 | ResModule | 资源加载 |
| 11 | SceneModule | 场景管理 |
| 12 | CameraModule | 相机管理 |
| 13 | SoundModule | 声音播放 |
| 14 | GameObjectPoolModule | 对象池 |
| 15 | DataTableModule | 配置表 |
| 16 | LocalizationModule | 本地化 |
| 17 | BehaviorModule | 行为树 |
| 18 | SystemModule | 系统管理 |
| 19 | UIModule | UI 界面 |
| 20 | ProcedureModule | 流程管理 |

**注意**：后续模块可以依赖前面已初始化的模块。例如 UIModule 依赖 EventModule 和 ResModule。

## 核心设计模式

### 1. 模块单例模式

所有模块继承 `ModuleBase<T>`，自动实现单例：

```csharp
public sealed class UIModule : ModuleBase<UIModule>
{
    // Instance 属性由基类自动提供
}

// 访问方式
UIModule.Instance.OpenForm(formId);
```

基类 `ModuleBase<T>` 提供以下功能：
- 自动单例实例化
- IOC 自动注册（默认启用，可通过 `RegisterIOC = false` 禁用）
- 生命周期管理（Init/Update/Destroy）
- 模块间依赖

### 2. IOC 依赖注入

框架使用 IOC（控制反转）实现模块间解耦：

```csharp
public class MyService
{
    [Inject]  // 自动注入
    private UIModule _uiModule;

    public void Init()
    {
        this.InjectThis(); // 触发注入
    }
}
```

### 3. 事件驱动通信

模块间通过事件系统通信，避免直接耦合：

```csharp
// 订阅
EventModule.Instance.Subscribe(MyEventArgs.EventId, OnHandler);

// 发布
EventModule.Instance.Dispatch(sender, MyEventArgs.Create(params));
```

### 4. 流程状态机

游戏流程通过 ProcedureModule 管理：

```csharp
ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures);
```

## GameApp 生命周期

```csharp
public sealed class GameApp
{
    public static GameApp Instance { get; }

    // 初始化（GameEntry.Awake 调用）
    internal void Initialize();

    // 每帧更新（GameEntry.Update 调用）
    public void Update(float elapseSeconds, float realElapseSeconds);

    // 销毁（GameEntry.OnDestroy 调用）
    public void Destroy();

    // 重启
    public void RestartGame();
}
```

## 游戏入口流程

```
GameEntry.Awake()
    │
    └── GameApp.Initialize()
            │
            └── GameApp 构造函数
                    │
                    └── 按顺序初始化 20 个模块

GameEntry.Start()
    │
    └── ProcedureModule.Launch(procedureId, procedures)
            │
            └── ProcedureSplash.OnEnter() → ... → ProcedureLogin.OnEnter()

GameEntry.Update()
    │
    └── GameApp.Update(deltaTime, unscaledDeltaTime)
            │
            └── 所有模块的 Update()
```

## 目录结构

```
Assets/HoweFramework/
├── GameApp.cs              # 框架入口
├── Base/
│   └── ModuleBase.cs       # 模块基类
├── IOC/
│   ├── IOCModule.cs        # IOC 容器
│   └── InjectAttribute.cs  # 注入标记
├── Event/
│   └── EventModule.cs      # 事件系统
├── UI/
│   └── UIModule.cs         # UI 管理
├── Procedure/
│   └── ProcedureModule.cs  # 流程管理
├── Res/
│   └── ResModule.cs        # 资源管理
├── Network/
│   └── NetworkModule.cs    # 网络管理
└── [其他模块...]

Assets/GameMain/
├── Scripts/
│   ├── GameEntry.cs        # Unity 入口
│   ├── GameConfig.cs       # 游戏配置
│   ├── Procedure/          # 游戏流程
│   ├── UI/                 # 游戏 UI
│   └── Gameplay/           # 游戏玩法框架
│       ├── Framework/
│       │   ├── Entity/      # 实体组件
│       │   ├── View/        # 视图
│       │   └── ...
│       └── Common/
│           ├── Numeric/     # 数值
│           ├── State/       # 状态
│           └── Resource/    # 资源
```

## 关键类图

```
ModuleBase<T>
    ├── IOCModule
    ├── EventModule
    ├── UIModule
    ├── ProcedureModule
    ├── ResModule
    ├── NetworkModule
    └── ... (其他模块)

GameEventArgs (抽象基类)
    └── Packet (网络包)
    └── RequestBase (请求)
    └── MyCustomEventArgs

IUIFormLogic
    └── FairyGUIFormLogicBase

IGameEntity
    └── GameEntity (由 GameEntityManager 创建)

GameComponentBase
    └── TransformComponent
    └── ViewComponent
    └── NumericComponent
    └── StateComponent
    └── ResourceComponent
```