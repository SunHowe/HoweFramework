# 网络系统

## 概述

NetworkModule 提供 TCP 网络通信能力，支持自定义协议解析。

## 核心文件

| 文件 | 路径 |
|------|------|
| NetworkModule | `Assets/HoweFramework/Network/NetworkModule.cs` |
| INetworkChannel | `Assets/HoweFramework/Network/Core/INetworkChannel.cs` |
| INetworkChannelHelper | `Assets/HoweFramework/Network/Core/INetworkChannelHelper.cs` |
| ServiceType | `Assets/HoweFramework/Network/Core/ServiceType.cs` |
| Packet | `Assets/HoweFramework/Network/Core/Packet.cs` |

## ServiceType

```csharp
public enum ServiceType : byte
{
    Tcp = 0,                  // TCP 异步收发
    TcpWithSyncReceive = 1,   // TCP 同步接收
}
```

## INetworkChannel

```csharp
public interface INetworkChannel
{
    string Name { get; }
    Socket Socket { get; }
    bool Connected { get; }
    ServiceType ServiceType { get; }
    AddressFamily AddressFamily { get; }

    // 统计
    int SendPacketCount { get; }
    int ReceivedPacketCount { get; }

    // 心跳
    float HeartBeatInterval { get; set; }
    float HeartBeatElapseSeconds { get; }
    int MissHeartBeatCount { get; }
    bool ResetHeartBeatElapseSecondsWhenReceivePacket { get; set; }

    INetworkChannelHelper Helper { get; }

    // 连接
    void Connect(IPAddress ipAddress, int port);
    void Connect(IPAddress ipAddress, int port, object userData);

    // 关闭
    void Close();

    // 发送
    void Send<T>(T packet) where T : Packet;

    // 处理器
    void RegisterHandler(IPacketHandler handler);
    void SetDefaultHandler(GameEventHandlerFunc handler);
}
```

## INetworkChannelHelper

```csharp
public interface INetworkChannelHelper : IDisposable
{
    // 包头长度（用于粘包处理）
    int PacketHeaderLength { get; }

    // 请求分发器
    IRemoteRequestDispatcher RequestDispatcher { get; }

    // 初始化
    void Initialize(INetworkChannel networkChannel);

    // 连接前准备
    void PrepareForConnecting();

    // 心跳
    bool SendHeartBeat();

    // 序列化
    bool Serialize<T>(T packet, Stream destination) where T : Packet;

    // 反序列化包头
    IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData);

    // 反序列化包体
    Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData);
}
```

## Packet

```csharp
public abstract class Packet : GameEventArgs
{
    public override bool IsReleaseAfterFire => m_IsReleaseAfterFire;
    private bool m_IsReleaseAfterFire = true;

    public void SetIsReleaseAfterFire(bool isReleaseAfterFire);
    public override void Clear();
}
```

## NetworkModule API

```csharp
public sealed class NetworkModule : ModuleBase<NetworkModule>
{
    public string DefaultChannelName { get; }
    public INetworkChannel DefaultChannel { get; }

    // 检查通道是否存在
    public bool HasNetworkChannel(string name);

    // 获取通道
    public INetworkChannel GetNetworkChannel(string name);

    // 获取所有通道
    public INetworkChannel[] GetAllNetworkChannels();

    // 创建通道
    public INetworkChannel CreateNetworkChannel(string name, ServiceType serviceType, INetworkChannelHelper networkChannelHelper);
    public INetworkChannel CreateDefaultNetworkChannel(string name, ServiceType serviceType, INetworkChannelHelper networkChannelHelper);

    // 销毁通道
    public bool DestroyNetworkChannel(string name);
}
```

## 网络事件

通过 EventModule 订阅：

| 事件 | 说明 |
|------|------|
| `NetworkConnectedEventArgs` | 连接成功 |
| `NetworkClosedEventArgs` | 连接关闭 |
| `NetworkMissHeartBeatEventArgs` | 心跳丢失 |
| `NetworkErrorEventArgs` | 网络错误 |
| `NetworkCustomErrorEventArgs` | 自定义错误 |

## 创建自定义网络助手

```csharp
public class MyNetworkChannelHelper : INetworkChannelHelper
{
    private INetworkChannel _channel;

    public int PacketHeaderLength => 4;

    public IRemoteRequestDispatcher RequestDispatcher => null;

    public void Initialize(INetworkChannel networkChannel)
    {
        _channel = networkChannel;
    }

    public void PrepareForConnecting()
    {
        // 连接前准备
    }

    public bool SendHeartBeat()
    {
        // 发送心跳包
        _channel.Send(new HeartBeatPacket());
        return true;
    }

    public bool Serialize<T>(T packet, Stream destination) where T : Packet
    {
        // 序列化包体
        return true;
    }

    public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
    {
        customErrorData = null;
        // 读取包头
        return new MyPacketHeader { ... };
    }

    public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
    {
        customErrorData = null;
        // 反序列化包体
        return new MyPacket { ... };
    }

    public void Dispose()
    {
    }
}
```

## 基本用法

### 1. 创建通道

```csharp
var helper = new MyNetworkChannelHelper();
var channel = NetworkModule.Instance.CreateNetworkChannel(
    "Gateway",
    ServiceType.Tcp,
    helper
);
```

### 2. 订阅事件

```csharp
EventModule.Instance.Subscribe(NetworkConnectedEventArgs.EventId, OnConnected);
EventModule.Instance.Subscribe(NetworkClosedEventArgs.EventId, OnClosed);
EventModule.Instance.Subscribe(NetworkErrorEventArgs.EventId, OnError);

private void OnConnected(object sender, GameEventArgs e)
{
    var args = (NetworkConnectedEventArgs)e;
    Debug.Log($"Connected to {args.NetworkChannel.Name}");
}

private void OnClosed(object sender, GameEventArgs e)
{
    Debug.Log("Connection closed");
}

private void OnError(object sender, GameEventArgs e)
{
    var args = (NetworkErrorEventArgs)e;
    Debug.LogError($"Network error: {args.ErrorCode}");
}
```

### 3. 连接

```csharp
channel.Connect(IPAddress.Parse("127.0.0.1"), 8888);

// 或带用户数据
channel.Connect(IPAddress.Parse("127.0.0.1"), 8888, token);
```

### 4. 发送数据包

```csharp
public class LoginRequest : Packet
{
    public string Username { get; set; }
    public string Password { get; set; }

    public override void Clear()
    {
        Username = null;
        Password = null;
    }
}

// 发送
channel.Send(new LoginRequest
{
    Username = "player1",
    Password = "123456"
});
```

### 5. 注册处理器

```csharp
public class LoginResponseHandler : IPacketHandler
{
    public int OpCode => 1001; // 登录响应码

    public void Handle(object sender, Packet packet)
    {
        var response = (LoginResponse)packet;
        Debug.Log($"Login result: {response.ErrorCode}");
    }
}

channel.RegisterHandler(new LoginResponseHandler());

// 设置默认处理器
channel.SetDefaultHandler((sender, packet) =>
{
    Debug.Log($"Unhandled packet: {packet.GetType().Name}");
});
```

### 6. 心跳配置

```csharp
channel.HeartBeatInterval = 5f; // 5秒一次心跳
channel.MissHeartBeatCount = 3; // 丢失3次心跳认为断线
channel.ResetHeartBeatElapseSecondsWhenReceivePacket = true;
```

### 7. 关闭连接

```csharp
channel.Close();
```

## 完整示例

```csharp
public class NetworkManager
{
    private INetworkChannel _channel;

    [Inject]
    private EventModule _eventModule;

    public void Init()
    {
        this.InjectThis();

        // 订阅事件
        _eventModule.Subscribe(NetworkConnectedEventArgs.EventId, OnConnected);
        _eventModule.Subscribe(NetworkClosedEventArgs.EventId, OnClosed);
        _eventModule.Subscribe(NetworkMissHeartBeatEventArgs.EventId, OnMissHeartBeat);

        // 创建通道
        var helper = new MyNetworkChannelHelper();
        _channel = NetworkModule.Instance.CreateNetworkChannel(
            NetworkConst.GatewayChannelName,
            ServiceType.Tcp,
            helper
        );

        // 注册处理器
        _channel.RegisterHandler(new LoginResponseHandler());
    }

    public void Connect()
    {
        _channel.Connect(IPAddress.Parse("127.0.0.1"), 8888);
    }

    public void Disconnect()
    {
        _channel?.Close();
    }

    private void OnConnected(object sender, GameEventArgs e)
    {
        Debug.Log("Connected to server");
    }

    private void OnClosed(object sender, GameEventArgs e)
    {
        Debug.Log("Connection closed");
    }

    private void OnMissHeartBeat(object sender, GameEventArgs e)
    {
        Debug.LogWarning("Miss heartbeat!");
    }
}
```