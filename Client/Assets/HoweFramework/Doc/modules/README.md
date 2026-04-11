# 模块系统

## 概述

HoweFramework 的核心由 20+ 模块组成，每个模块负责特定功能。所有模块继承自 `ModuleBase<T>`，采用单例模式访问。

## 模块列表

| 模块 | 说明 | 详细文档 |
|------|------|----------|
| IOCModule | 依赖注入容器 | [ioc.md](ioc.md) |
| BaseModule | 基础工具（Json、文本模板） | [base.md](base.md) |
| EventModule | 事件系统 | [event.md](event.md) |
| RemoteRequestModule | 远程请求调度器 | [remote-request.md](remote-request.md) |
| NetworkModule | TCP 网络 | [network.md](network.md) |
| WebRequestModule | HTTP 请求 | [web-request.md](web-request.md) |
| TimerModule | 计时器 | [timer.md](timer.md) |
| SettingModule | 设置（PlayerPrefs） | [setting.md](setting.md) |
| SafeAreaModule | 安全区域 | [safe-area.md](safe-area.md) |
| ResModule | 资源加载 | [resource.md](resource.md) |
| SceneModule | 场景管理 | [scene.md](scene.md) |
| CameraModule | 相机管理 | [camera.md](camera.md) |
| SoundModule | 声音播放 | [sound.md](sound.md) |
| GameObjectPoolModule | 对象池 | [object-pool.md](object-pool.md) |
| DataTableModule | 配置表 | [data-table.md](data-table.md) |
| LocalizationModule | 本地化 | [localization.md](localization.md) |
| BehaviorModule | 行为树 | [behavior-tree.md](behavior-tree.md) |
| SystemModule | 系统管理 | [system.md](system.md) |
| UIModule | UI 界面 | [ui.md](ui.md) |
| ProcedureModule | 流程管理 | [procedure.md](procedure.md) |

## 模块基类

所有模块继承自 `ModuleBase<T>`：

```csharp
public abstract class ModuleBase<T> : ModuleBase where T : ModuleBase<T>
{
    public static T Instance { get; private set; }

    // 是否注册到 IOC（默认 true）
    protected virtual bool RegisterIOC => true;

    // 内部初始化（由 GameApp 调用）
    internal sealed override void Init()
    {
        Instance = (T)this;
        OnInit();
        if (RegisterIOC)
            IOCModule.Instance.Register(Instance);
    }

    // 子类实现
    protected abstract void OnInit();
    protected abstract void OnDestroy();
    protected abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);
}
```

## 访问模块

### 方式一：单例访问（推荐）

```csharp
UIModule.Instance.OpenForm(formId);
ResModule.Instance.LoadAssetAsync<T>("path");
EventModule.Instance.Subscribe(eventId, handler);
```

### 方式二：IOC 注入（解耦最佳实践）

```csharp
public class MyClass
{
    [Inject]
    private UIModule _uiModule;

    [Inject]
    public IResLoader ResLoader { get; set; }

    public void Init()
    {
        this.InjectThis(); // 触发注入
    }
}
```

## 模块生命周期

```
Init()          ← GameApp 构造函数中调用
    │
    ├── OnInit()           ← 模块初始化逻辑
    │
    └── IOC.Register()     ← 自动注册到 IOC 容器

Update()         ← 每帧调用
    │
    └── OnUpdate(elapseSeconds, realElapseSeconds)

Destroy()        ← 游戏退出时调用
    │
    ├── OnDestroy()        ← 清理逻辑
    │
    └── IOC.Unregister()    ← 自动从 IOC 移除
```

## 创建新模块

1. 继承 `ModuleBase<T>`：
```csharp
public sealed class MyModule : ModuleBase<MyModule>
{
    protected override void OnInit() { /* 初始化 */ }
    protected override void OnDestroy() { /* 清理 */ }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) { /* 每帧更新 */ }
}
```

2. 在 `GameApp` 构造函数中注册：
```csharp
// GameApp.cs 构造函数
new EventModule();
// ... 其他模块
new MyModule(); // 添加新模块
```

3. 模块自动成为单例，可通过 `MyModule.Instance` 访问。

## 最佳实践

- **使用 IOC 注入**：便于单元测试和模块解耦
- **避免直接引用**：优先通过事件系统通信
- **及时清理**：在 `OnDestroy` 中释放资源、取消订阅