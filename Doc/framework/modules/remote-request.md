# RemoteRequestModule

## 职责

生成请求 id 并等待对应响应。网络频道的 request/response 匹配用它，不是业务 HTTP。

## 关键类型

- `RemoteRequestModule.CreateRemoteRequestDispatcher` → `RemoteRequestDispatcher`（`IReference`）
- `IRemoteRequestDispatcher`：`CreateRemoteRequest` 返回 `(requestId, UniTask<IResponse>)`；`SetResponse`；`InterruptAllRequests`；`Dispose`

找不到 id 的响应会被 `Dispose` 掉。`Dispose` 调度器会用 `RequestDispatcherDisposing` 打断未完成请求。

## 用法

由 `NetworkPacketRequest` 调用：`NetworkChannel.Helper.RequestDispatcher.CreateRemoteRequest()`，把 id 写进 `IRemoteRequest`，再 `Send`。

业务一般不要直接 new 调度器，除非实现自定义频道 Helper。

## 扩展点

`CreateRemoteRequestDispatcher` 从引用池取 `RemoteRequestDispatcher`。用完 `Dispose`。

## 约束与坑

- 与 `WebRequestModule` 无关。
- 模块本身 `OnInit`/`OnDestroy` 为空，调度器生命周期在频道 Helper 一侧。

## 相关源码

- `Client/Assets/HoweFramework/RemoteRequest/RequestModule.cs`
- `Client/Assets/HoweFramework/RemoteRequest/RemoteRequestDispatcher.cs`
- `Client/Assets/HoweFramework/RemoteRequest/IRemoteRequestDispatcher.cs`
