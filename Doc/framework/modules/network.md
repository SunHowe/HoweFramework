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

- 错误码 401–413。
- 发包走 Request 抽象，Execute 结束会 `ReferencePool.Release` 请求对象。
- 无默认频道时发送会 `NetworkChannelNotExist`。

## 相关源码

- `Client/Assets/HoweFramework/Network/NetworkModule.cs`
- `Client/Assets/HoweFramework/Network/Core/Packet.cs`
- `Client/Assets/HoweFramework/Network/NetworkPacketRequest.cs`
- `Client/Assets/HoweFramework/Extensions/NetworkModuleExtensions.cs`
