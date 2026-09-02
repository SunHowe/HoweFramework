# SystemModule

## 职责

注册、获取、销毁业务 `ISystem`。这是业务服务容器，不是新的框架 Module。

## 关键类型

- `SystemModule`：`RegisterSystem<T>`、`RegisterSystem<TInterface, TSystem>`、`GetSystem<T>`、`DestroySystem<T>`、`OnSystemDestroyed`
- `ISystem` / `SystemBase` / `SystemFacade`

重复注册抛框架异常。注册时调用 `system.Init()`。

业务示例：`ProcedureInitSystem` 里注册登录等；`GameMain/Scripts/System/Login/`。

## 用法

```csharp
SystemModule.Instance.RegisterSystem<ILoginSystem, OfflineLoginSystem>();
var login = SystemModule.Instance.GetSystem<ILoginSystem>();
```

## 扩展点

长期存在的业务服务用 System，不要再加 `ModuleBase`。需要全局事件/资源时注入或调已有 Module。

## 约束与坑

- `GetSystem` 未注册返回 default。
- 按接口注册时，`GetSystem` 要用同一接口类型。
- 模块销毁会清系统列表（见 `SystemModule.OnDestroy`）。

## 相关源码

- `Client/Assets/HoweFramework/System/SystemModule.cs`
- `Client/Assets/HoweFramework/System/ISystem.cs`
- `Client/Assets/HoweFramework/System/SystemBase.cs`
- `Client/Assets/GameMain/Scripts/System/Login/`
