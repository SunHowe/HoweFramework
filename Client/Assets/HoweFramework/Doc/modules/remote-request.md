# 远程请求调度器

## 概述

RemoteRequestModule 提供远程请求的分发和响应管理，支持请求-响应模式。

## 核心文件

| 文件 | 路径 |
|------|------|
| RequestModule | `Assets/HoweFramework/RemoteRequest/RequestModule.cs` |

## RemoteRequestModule API

```csharp
public sealed class RemoteRequestModule : ModuleBase<RemoteRequestModule>
{
    // 创建请求调度器
    public IRemoteRequestDispatcher CreateRemoteRequestDispatcher();
}
```

## IRemoteRequestDispatcher

```csharp
public interface IRemoteRequestDispatcher : IDisposable
{
    // 创建请求（返回请求 ID 和响应的 Task）
    (int requestId, UniTask<IResponse> task) CreateRemoteRequest();

    // 设置响应（内部使用）
    void SetResponse(int requestId, IResponse response);

    // 中断所有请求
    void InterruptAllRequests(int errorCode = FrameworkErrorCode.RequestCanceled);
}
```

## 与 NetworkModule 配合

`INetworkChannelHelper` 使用 `IRemoteRequestDispatcher`：

```csharp
public interface INetworkChannelHelper : IDisposable
{
    // ...
    IRemoteRequestDispatcher RequestDispatcher { get; }
    // ...
}
```

## 基本用法

### 1. 创建调度器

```csharp
var dispatcher = RemoteRequestModule.Instance.CreateRemoteRequestDispatcher();
```

### 2. 创建请求

```csharp
// 创建请求
var (requestId, task) = dispatcher.CreateRemoteRequest();

// 发送网络请求
channel.Send(new MyRequest { RequestId = requestId });

// 等待响应
var response = await task;
```

### 3. 处理响应

```csharp
// 在网络包处理中
public void OnResponseReceived(int requestId, byte[] data)
{
    var response = CommonResponse.Create(0, data);
    dispatcher.SetResponse(requestId, response);
}
```

## 完整示例

### 带请求-响应模式的 RPC 调用

```csharp
public class RpcClient
{
    private INetworkChannel _channel;
    private IRemoteRequestDispatcher _dispatcher;

    public void Initialize(INetworkChannel channel)
    {
        _channel = channel;
        _dispatcher = RemoteRequestModule.Instance.CreateRemoteRequestDispatcher();

        // 注册响应处理器
        _channel.RegisterHandler(new ResponseHandler(_dispatcher));
    }

    public async UniTask<T> CallAsync<T>(int opcode, object request)
    {
        // 创建 RPC 请求
        var (requestId, task) = _dispatcher.CreateRemoteRequest();

        // 发送请求
        _channel.Send(new RpcRequest
        {
            Opcode = opcode,
            RequestId = requestId,
            Data = Serialize(request)
        });

        // 等待响应
        var response = await task;

        if (response.ErrorCode != 0)
        {
            throw new Exception($"RPC call failed: {response.ErrorCode}");
        }

        return Deserialize<T>(response.GetBytes());
    }
}

public class ResponseHandler : IPacketHandler
{
    private IRemoteRequestDispatcher _dispatcher;

    public ResponseHandler(IRemoteRequestDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public int OpCode => (int)MyOpCodes.Response;

    public void Handle(object sender, Packet packet)
    {
        var response = (RpcResponse)packet;
        _dispatcher.SetResponse(response.RequestId, CommonResponse.Create(0, response.Data));
    }
}
```

## 请求超时处理

```csharp
public async UniTask<IResponse> CreateRemoteRequestWithTimeout(
    IRemoteRequestDispatcher dispatcher,
    Action sendAction,
    int timeoutMs = 5000)
{
    var (requestId, task) = dispatcher.CreateRemoteRequest();

    sendAction();

    // 使用 TimeoutExtension
    var result = await task.Timeout(TimeSpan.FromMilliseconds(timeoutMs));

    return result;
}
```

## 最佳实践

### 1. 统一的错误码处理

```csharp
public async UniTask<IResponse> CallAsync(Action sendAction)
{
    var (requestId, task) = _dispatcher.CreateRemoteRequest();

    sendAction();

    try
    {
        return await task.Timeout(TimeSpan.FromSeconds(30));
    }
    catch (TimeoutException)
    {
        return CommonResponse.Create(FrameworkErrorCode.RequestTimeout);
    }
}
```

### 2. 取消请求

```csharp
// 中断所有请求
_dispatcher.InterruptAllRequests(FrameworkErrorCode.RequestCanceled);
```

### 3. 记得 Dispose

```csharp
public void Dispose()
{
    _dispatcher?.Dispose();
}
```