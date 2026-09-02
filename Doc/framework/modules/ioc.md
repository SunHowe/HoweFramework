# IOCModule

## 职责

为全局单例提供注册、获取、字段/属性注入。不管理实例生命周期。

## 关键类型

- `IOCModule`：`Register<T>`（已存在则覆盖）、`UnRegister`、`Get<T>`（没有则 default）、`Inject`
- `InjectAttribute`：标在字段或属性上

`ModuleBase<T>.OnInit` 默认会 `Register(Instance)`。

## 用法

```csharp
IOCModule.Instance.Register(myService);
var s = IOCModule.Instance.Get<MyService>();

[Inject] private EventModule _event;
this.InjectThis(); // IOCModuleExtensions，内部调用 IOCModule.Instance.Inject
```

`Inject` 会沿继承链收集成员；属性必须有非 private 的 setter。容器里没有对应类型时跳过，不抛错。

## 扩展点

业务服务也可 `Register`，但销毁时要自己 `UnRegister`，模块销毁不会替你管理非模块对象。

## 约束与坑

- 覆盖注册不会销毁旧实例。
- 带实例的 `UnRegister<T>(instance)` 会核对引用，对不上则不删。
- `OnDestroy` 清空字典。

## 相关源码

- `Client/Assets/HoweFramework/IOC/IOCModule.cs`
- `Client/Assets/HoweFramework/IOC/InjectAttribute.cs`
