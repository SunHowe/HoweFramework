# NetworkModule

## 职责

管理网络频道（连接、收发、Handler）。协议包是 `Packet`（也是 `GameEventArgs`）。

## 关键类型

- `NetworkModule`：`CreateNetworkChannel`、`CreateDefaultNetworkChannel`、`GetNetworkChannel`、`DefaultChannel`
- `INetworkChannel` / `INetworkChannelHelper` / `IPacketHandler` / `IPacketHeader`
- `Packet`、`NetworkPacketRequest`
- 扩展：`ConnectAsync`、`SendPacketAsync` / `SendPacketAsync<T>`、`Disconnect`、`MappingPacketHandler`

`SendPacketAsync` 要求 Packet 实现 `IRemoteRequest`，否则 `NetworkPacketRequestNotRemoteRequest`。未指定频道用 `DefaultChannel`。

## 用法

```csharp
var code = await NetworkModule.Instance.ConnectAsync("127.0.0.1", 9000).GetErrorCode();
using var response = await somePacket.SendPacketAsync<LoginResponse>();
```

## 扩展点

自备 `INetworkChannelHelper`（含序列化与 `RequestDispatcher`）。程序集内 Handler 可用 `MappingPacketHandler`。

## 约束与坑

- 错误码 401–414（414 为 `NetworkChannelClosed`，连接中关闭/重连/销毁频道时 `ConnectAsync` 以此码完成，不再挂起）。
- 发包走 Request 抽象，Execute 结束会 `ReferencePool.Release` 请求对象。
- 无默认频道时发送会 `NetworkChannelNotExist`。
- `Packet.Id` 继承 `GameEventArgs` 的运行时 TypeId，不是跨进程协议号。收包池按 `IPacketHandler.Id` 订阅、按 `packet.Id` 派发，Handler 的 `Id` 须与对应 Packet 类型的 TypeId 一致（例如 `TypeId<T>.Id`）。
- **收包池派发模式**：频道收包池固定为 `AllowNoHandler | AlwaysInvokeDefaultHandler`。未注册 Handler 的服务器推送包由默认处理函数兜底（不抛异常）；已注册 Handler 的 RPC 响应包仍会先经默认处理函数路由到 `RequestDispatcher` 完成 `SendPacketAsync`，业务 Handler 视为"额外监听"，二者都会收到包。
- **RPC 超时**：`RemoteRequestDispatcher.RequestTimeout` 默认 30 秒（≤0 关闭），超时请求以 `RequestTimeout`(704) 完成；超时检测在创建新请求时惰性扫描。`Send` 抛异常时请求注册项会被注销并以 `RequestCanceled` 完成。
- 收包长度字段有 64MB 上限（`ReceiveState.MaxPacketLength`），超限抛 `InvalidParam`，防止恶意/损坏数据导致 OOM。
- `ReferencePool` 已加锁，网络线程（Socket 回调）与主线程并发 Acquire/Release 是安全的。

## 相关源码

- `Client/Assets/HoweFramework/Network/NetworkModule.cs`
- `Client/Assets/HoweFramework/Network/Core/Packet.cs`
- `Client/Assets/HoweFramework/Network/NetworkPacketRequest.cs`
- `Client/Assets/HoweFramework/Extensions/NetworkModuleExtensions.cs`
