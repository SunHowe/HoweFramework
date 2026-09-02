# Request

## 职责

统一异步交互：执行后必定得到带 `ErrorCode` 的 `IResponse`。UI 打开、HTTP、网络包都走这套。

## 关键类型

- `RequestBase`：`Execute` → `OnExecute`；catch 后变成 `CommonResponse`；`finally` 里 `ReferencePool.Release(this)`
- `IResponse` / `CommonResponse`
- `RequestExtensions`：`GetErrorCode`（会 Dispose 响应）、`As<T>`、`Forget`

取消：`FrameworkErrorCode.RequestCanceled`。类型不符且成功：`ResponseTypeMismatch`。

## 用法

```csharp
var error = await someRequest.Execute().GetErrorCode();
using var typed = await someRequest.Execute().As<MyResponse>();
```

## 扩展点

新交互：继承 `RequestBase`，在 `OnExecute` 里干活并返回 `IResponse`。从池子 `Acquire` 再 `Execute`。

## 约束与坑

- `Execute` 结束后请求对象已回收，不要继续用同一个 Request 实例。
- `GetErrorCode` 会释放响应；还要读业务字段时用 `As<T>` 并 `using`。
- 调度器销毁中：`RequestDispatcherDisposing`。

## 相关源码

- `Client/Assets/HoweFramework/Request/Core/RequestBase.cs`
- `Client/Assets/HoweFramework/Request/RequestExtensions.cs`
- `Client/Assets/HoweFramework/Request/CommonResponse.cs`
