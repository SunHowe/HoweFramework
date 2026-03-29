# Howe Framework - Claude Code 项目指南

本项目是 **HoweFramework** Unity 游戏框架及基于其上的游戏业务实现。

## 项目结构

```
howe-framework/Client/
├── Assets/
│   ├── HoweFramework/           # 框架核心 (Assets/HoweFramework/README.md)
│   ├── HoweFramework.Editor/   # 编辑器工具
│   └── GameMain/               # 游戏业务
│       └── Scripts/
│           ├── Gameplay/       # 游戏玩法框架 (Doc/README.md)
│           ├── UI/             # UI界面
│           ├── Procedure/       # 游戏流程
│           ├── Network/        # 网络协议
│           └── ...
├── Packages/
└── ProjectSettings/
```

## 文档索引

| 模块 | 文档路径 | 说明 |
|------|----------|------|
| **框架核心** | `Assets/HoweFramework/README.md` | 模块系统、IOC、UI、事件、引用池、20+模块 |
| **Gameplay 框架** | `Assets/GameMain/Scripts/Gameplay/Doc/README.md` | 实体-组件架构、View系统、数值系统、状态组件、资源组件 |
| **新手入门** | `Assets/HoweFramework/Doc/Tutorial/getting-started.md` | 环境配置、创建入口、实体、UI、依赖注入 |
| **示例项目** | `Assets/HoweFramework/Doc/Tutorial/demo-project.md` | 模块配合示例代码 |
| **故障排查** | `Assets/HoweFramework/Doc/troubleshooting.md` | 常见问题及解决方法 |
| **性能优化** | `Assets/HoweFramework/Doc/performance.md` | 性能最佳实践 |

## Claude Code Skills

使用 `/skill-name` 调用以下 Skills：

| Skill | 用途 | 触发命令 |
|-------|------|----------|
| `create-entity-component` | 快速创建实体-组件 | `/create-entity-component` |
| `create-ui-form` | 快速创建 UI 界面 | `/create-ui-form` |
| `create-game-context` | 创建游戏上下文和管理器 | `/create-game-context` |
| `create-procedure` | 创建游戏流程状态 | `/create-procedure` |
| `setup-network` | 设置网络通道和协议 | `/setup-network` |
| `implement-event` | 实现事件系统 | `/implement-event` |

## 框架模块（共20个）

> 详细API见 [`Assets/HoweFramework/README.md`](Assets/HoweFramework/README.md)

| 模块 | 说明 | 详细章节 |
|------|------|----------|
| IOCModule | 依赖注入 | [IOC 依赖注入](Assets/HoweFramework/README.md#ioc-依赖注入) |
| BaseModule | 基础工具（Json、文本模板） | [基础系统](Assets/HoweFramework/README.md#基础系统) |
| EventModule | 事件系统 | [事件系统](Assets/HoweFramework/README.md#事件系统) |
| RemoteRequestModule | 远程请求调度器 | [远程请求调度器](Assets/HoweFramework/README.md#远程请求调度器) |
| NetworkModule | 网络（TCP） | [网络系统](Assets/HoweFramework/README.md#网络系统) |
| WebRequestModule | HTTP请求 | [Web请求系统](Assets/HoweFramework/README.md#web请求系统) |
| TimerModule | 计时器 | [计时器系统](Assets/HoweFramework/README.md#计时器系统) |
| SettingModule | 设置（PlayerPrefs） | [设置系统](Assets/HoweFramework/README.md#设置系统) |
| SafeAreaModule | 安全区域 | [安全区域系统](Assets/HoweFramework/README.md#安全区域系统) |
| ResModule | 资源加载 | [资源系统](Assets/HoweFramework/README.md#资源系统) |
| SceneModule | 场景管理 | [场景系统](Assets/HoweFramework/README.md#场景系统) |
| CameraModule | 相机管理 | [相机系统](Assets/HoweFramework/README.md#相机系统) |
| SoundModule | 声音播放 | [声音系统](Assets/HoweFramework/README.md#声音系统) |
| GameObjectPoolModule | 对象池 | [对象池系统](Assets/HoweFramework/README.md#对象池系统) |
| DataTableModule | 配置表 | [配置表系统](Assets/HoweFramework/README.md#配置表系统) |
| LocalizationModule | 本地化 | [本地化系统](Assets/HoweFramework/README.md#本地化系统) |
| BehaviorModule | 行为树 | [行为树系统](Assets/HoweFramework/README.md#行为树系统) |
| SystemModule | 系统管理 | [系统模块](Assets/HoweFramework/README.md#系统模块) |
| UIModule | UI界面 | [UI 系统](Assets/HoweFramework/README.md#ui-系统) |
| ProcedureModule | 流程管理 | [流程系统](Assets/HoweFramework/README.md#流程系统) |

## 快速参考

> 完整API文档见各模块详细章节（见上表"详细章节"列）

### 框架核心

```csharp
// 模块访问
UIModule.Instance.OpenForm(formId);
ResModule.Instance.LoadAssetAsync<T>(path);
EventModule.Instance.Subscribe(eventId, handler);

// IOC 注入
[Inject]
private UIModule _uiModule;

// 引用池
var obj = ReferencePool.Acquire<MyClass>();
ReferencePool.Release(obj);
```

### Gameplay 框架

> Gameplay框架详细文档：[`Assets/GameMain/Scripts/Gameplay/Doc/README.md`](Assets/GameMain/Scripts/Gameplay/Doc/README.md)

```csharp
// 创建实体
var entity = context.GetManager<GameEntityManager>().CreateEntity();

// 添加组件
entity.AddComponent<TransformComponent>();
entity.AddComponent<NumericComponent>();

// 获取组件
var transform = entity.GetComponent<TransformComponent>();

// 销毁实体
context.GetManager<GameEntityManager>().DestroyEntity(entity.EntityId);
```

## 关键文件

> 完整文件列表及详细说明见 [`Assets/HoweFramework/README.md`](Assets/HoweFramework/README.md) 和 [`Gameplay/Doc/README.md`](Assets/GameMain/Scripts/Gameplay/Doc/README.md)

### 框架核心

| 文件 | 说明 |
|------|------|
| `Assets/HoweFramework/GameApp.cs` | 框架入口，模块初始化 |
| `Assets/HoweFramework/Base/ModuleBase.cs` | 模块基类 |
| `Assets/HoweFramework/IOC/IOCModule.cs` | IOC容器 |

### 游戏业务

| 文件 | 说明 |
|------|------|
| `Assets/GameMain/Scripts/GameEntry.cs` | Unity入口脚本 |
| `Assets/GameMain/Scripts/Gameplay/Framework/GameContextBase.cs` | 游戏上下文基类 |
| `Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameEntityManager.cs` | 实体管理器 |
| `Assets/GameMain/Scripts/Gameplay/Framework/Update/GameUpdateManager.cs` | 更新管理器 |
| `Assets/GameMain/Scripts/Gameplay/Framework/Timer/GameTimerManager.cs` | 游戏定时器 |
| `Assets/GameMain/Scripts/Gameplay/Framework/Random/GameRandomManager.cs` | 随机数管理器 |
| `Assets/GameMain/Scripts/Gameplay/Common/State/StateComponent.cs` | 状态组件 |
| `Assets/GameMain/Scripts/Gameplay/Common/Resource/ResourceComponent.cs` | 资源组件 |

## 命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| 模块类 | `XXXModule` | `UIModule`, `ResModule` |
| 界面逻辑类 | `XXXFormLogic` | `MainMenuFormLogic` |
| 游戏组件 | `XXXComponent` | `TransformComponent` |
| 游戏管理器 | `XXXManager` | `GameEntityManager` |
| 游戏实体 | `GameEntity` | - |
| 组件类型枚举 | `GameComponentType` | `Transform = 1` |
| 管理器类型枚举 | `GameManagerType` | `Entity = 100` |

## 错误码

> 详细错误码范围定义见 [`Assets/HoweFramework/README.md#错误码规范`](Assets/HoweFramework/README.md#错误码规范)

错误码定义在两个位置：

- **框架错误码**: `Assets/HoweFramework/FrameworkErrorCode.cs`
- **游戏错误码**: `Assets/GameMain/Scripts/ErrorCode.cs`

## 依赖关系

```
GameMain (游戏业务)
    ├── UI (界面)
    ├── Procedure (流程)
    ├── Network (网络)
    ├── Gameplay (玩法)
    └── ...
        └── HoweFramework (框架核心)
            └── Unity (引擎)
```

**注意**: 游戏代码可以引用框架，框架不应引用游戏代码。
