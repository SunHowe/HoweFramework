# RemoteRequestModule

## 职责

生成请求 id 并等待对应响应。网络频道的 request/response 匹配用它，不是业务 HTTP。

## 关键类型

- `RemoteRequestModule.CreateRemoteRequestDispatcher` → `RemoteRequestDispatcher`（`IReference`）
- `IRemoteRequestDispatcher`：`CreateRemoteRequest` 返回 `(requestId, UniTask<IResponse>)`；`SetResponse`；`Remove`（以 `RequestCanceled` 完成并回收注册项，用于发送失败等场景）；`InterruptAllRequests`；`Dispose`

找不到 id 的响应会被 `Dispose` 掉。`Dispose` 调度器会用 `RequestDispatcherDisposing` 打断未完成请求。

## 用法

由 `NetworkPacketRequest` 调用：`NetworkChannel.Helper.RequestDispatcher.CreateRemoteRequest()`，把 id 写进 `IRemoteRequest`，再 `Send`。

业务一般不要直接 new 调度器，除非实现自定义频道 Helper。

## 扩展点

`CreateRemoteRequestDispatcher` 从引用池取 `RemoteRequestDispatcher`。用完 `Dispose`。

## 约束与坑

- 与 `WebRequestModule` 无关。
- 模块本身 `OnInit`/`OnDestroy` 为空，调度器生命周期在频道 Helper 一侧。
- **请求超时**：`RemoteRequestDispatcher.RequestTimeout` 默认 30 秒（≤0 关闭）。超时请求以 `RequestTimeout`(704) 完成。模块每帧扫描已创建的调度器；创建新请求时也会惰性扫描。停发新 RPC 时超时仍会触发。
- `NetworkPacketRequest` 会把 `CancellationToken` 接到等待上：取消时 `Remove` 该请求并以 `RequestCanceled` 完成。

## 相关源码

- `Client/Assets/HoweFramework/RemoteRequest/RequestModule.cs`
- `Client/Assets/HoweFramework/RemoteRequest/RemoteRequestDispatcher.cs`
- `Client/Assets/HoweFramework/RemoteRequest/IRemoteRequestDispatcher.cs`
