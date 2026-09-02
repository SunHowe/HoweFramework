# 模块系统

## 职责

全局服务以模块形式存在：单例访问、可选注册 IOC、随 `GameApp` 初始化和逐帧更新。

## 关键类型

- `ModuleBase`：`Init` / `Destroy` / `Update`（internal）。
- `ModuleBase<T>`：`Instance`；`OnInit` / `OnDestroy` / `OnUpdate`；`RegisterIOC` 默认 true，Init 时 `IOCModule.Instance.Register(Instance)`。
- `GameApp.AddModule<T>()`：`Activator.CreateInstance` 后立刻 `Init` 再加入列表。

重复 Init 同一模块类型会抛「模块已初始化」。Destroy 时先 UnRegister 再 `OnDestroy`，然后 `Instance = null`。`GameApp.Destroy` 逆序销毁模块。

## 用法

```csharp
UIModule.Instance.OpenUIForm(formId); // 扩展方法，见 UI 模块文档

[Inject]
private EventModule _eventModule;
IOCModule.Instance.Inject(this);
```

不要自己 `new` 一个已由 `GameApp` 创建的 `ModuleBase<T>`。

## 扩展点

- 新的全局服务：继承 `ModuleBase<T>`，在 `GameApp` 构造函数里按依赖顺序 `AddModule`。
- 可替换实现：模块上的 `Set*Helper` / `Use*` 扩展（资源、声音、设置、WebRequest、Json）。
- 局部能力：许多模块提供 `Create*`（事件调度器、资源加载器、对象池、计时器），用完 `Dispose`。

业务默认不要新增框架模块，优先用现有模块或 `SystemModule` 注册业务 System。

## 约束与坑

- IOC **不负责**实例生命周期，只存引用。注释见 `IOCModule`。
- `RegisterIOC = false` 的模块不会进容器，`[Inject]` 拿不到。
- 属性注入要求 setter 存在且不是 private。
- 模块 `OnUpdate` 顺序等于注册顺序。

## 相关源码

- `Client/Assets/HoweFramework/Base/ModuleBase.cs`
- `Client/Assets/HoweFramework/GameApp.cs`
- `Client/Assets/HoweFramework/IOC/IOCModule.cs`
