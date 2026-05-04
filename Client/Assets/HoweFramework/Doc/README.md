# HoweFramework 技术文档

本目录包含 HoweFramework Unity 游戏框架的完整技术文档，专为 AI Agent 提供深入的代码理解参考。

## 文档索引

### 框架架构
| 文档 | 说明 |
|------|------|
| [architecture.md](architecture.md) | 框架整体架构、模块依赖、初始化顺序 |

### 核心模块
| 文档 | 说明 |
|------|------|
| [modules/README.md](modules/README.md) | 模块系统概览 |
| [modules/ioc.md](modules/ioc.md) | IOC 依赖注入 |
| [modules/event.md](modules/event.md) | 事件系统 |
| [modules/ui.md](modules/ui.md) | UI 系统 |
| [modules/network.md](modules/network.md) | 网络系统 |
| [modules/resource.md](modules/resource.md) | 资源系统 |
| [modules/procedure.md](modules/procedure.md) | 流程系统 |
| [modules/timer.md](modules/timer.md) | 计时器系统 |
| [modules/reference-pool.md](modules/reference-pool.md) | 引用池 |
| [modules/fsm.md](modules/fsm.md) | 状态机 |
| [modules/behavior-tree.md](modules/behavior-tree.md) | 行为树 |
| [modules/object-pool.md](modules/object-pool.md) | 对象池 |
| [modules/scene.md](modules/scene.md) | 场景管理 |
| [modules/camera.md](modules/camera.md) | 相机管理 |
| [modules/sound.md](modules/sound.md) | 声音系统 |
| [modules/setting.md](modules/setting.md) | 设置系统 |
| [modules/data-table.md](modules/data-table.md) | 配置表系统 |
| [modules/localization.md](modules/localization.md) | 本地化 |
| [modules/safe-area.md](modules/safe-area.md) | 安全区域 |
| [modules/web-request.md](modules/web-request.md) | HTTP 请求 |
| [modules/remote-request.md](modules/remote-request.md) | 远程请求调度器 |
| [modules/system.md](modules/system.md) | 系统管理 |
| [modules/base.md](modules/base.md) | 基础工具 |

### Gameplay 框架
| 文档 | 说明 |
|------|------|
| [gameplay/README.md](gameplay/README.md) | Gameplay 框架概览 |
| [gameplay/entity-component.md](gameplay/entity-component.md) | 实体-组件系统 |
| [gameplay/view-system.md](gameplay/view-system.md) | 视图系统 |
| [gameplay/numeric-system.md](gameplay/numeric-system.md) | 数值系统 |
| [gameplay/state-component.md](gameplay/state-component.md) | 状态组件 |
| [gameplay/resource-component.md](gameplay/resource-component.md) | 资源组件 |
| [gameplay/game-context.md](gameplay/game-context.md) | 游戏上下文 |
| [gameplay/managers.md](gameplay/managers.md) | 游戏管理器 |

## 快速参考

### 模块访问
```csharp
// 通过单例访问模块
UIModule.Instance.OpenForm(formId);
ResModule.Instance.LoadAssetAsync<T>(path);

// 通过 IOC 注入（推荐）
[Inject]
private UIModule _uiModule;

this.InjectThis(); // 在 Init 时调用
```

### 事件订阅
```csharp
// 订阅事件
EventModule.Instance.Subscribe(MyEventArgs.EventId, OnHandler);

// 发布事件
EventModule.Instance.Dispatch(sender, MyEventArgs.Create(params));

// 取消订阅
EventModule.Instance.Unsubscribe(MyEventArgs.EventId, OnHandler);
```

### 创建 UI Form
```csharp
public sealed class MyForm : FullScreenFormLogicBase
{
    public override int FormId => (int)UIFormId.MyForm;

    protected override void OnInit() { }
    public override void OnOpen() { }
    public override void OnClose() { }
}

// 打开 Form
await UIModule.Instance.OpenUIForm(UIFormId.MyForm, userData);
```

### 创建实体
```csharp
var entity = context.GetManager<GameEntityManager>().CreateEntity();
entity.AddComponent<TransformComponent>();
entity.AddComponent<NumericComponent>();
```

## 关键文件路径

| 功能 | 文件路径 |
|------|----------|
| 框架入口 | `Assets/HoweFramework/GameApp.cs` |
| 模块基类 | `Assets/HoweFramework/Base/ModuleBase.cs` |
| IOC 容器 | `Assets/HoweFramework/IOC/IOCModule.cs` |
| 事件系统 | `Assets/HoweFramework/Event/EventModule.cs` |
| UI 模块 | `Assets/HoweFramework/UI/UIModule.cs` |
| 实体管理器 | `Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameEntityManager.cs` |
| 游戏上下文 | `Assets/GameMain/Scripts/Gameplay/Framework/GameContextBase.cs` |

## 文档贡献

本文档由 AI Agent 根据源代码自动生成。如发现文档与实际代码不符，请提交 Issue。