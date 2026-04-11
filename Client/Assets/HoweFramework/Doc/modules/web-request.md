# Web 请求系统

## 概述

WebRequestModule 提供 HTTP GET/POST 请求功能。

## 核心文件

| 文件 | 路径 |
|------|------|
| WebRequestModule | `Assets/HoweFramework/WebRequest/WebRequestModule.cs` |

## WebRequestModule API（内部）

```csharp
public sealed class WebRequestModule : ModuleBase<WebRequestModule>
{
    // 设置助手
    public void SetWebRequestHelper(IWebRequestHelper webRequestHelper);

    // 内部方法
    internal async UniTask<IResponse> Post(WebPostRequest request, CancellationToken token);
    internal async UniTask<IResponse> Get(WebGetRequest request, CancellationToken token);
}
```

## IWebRequestHelper

```csharp
public interface IWebRequestHelper : IDisposable
{
    // 发送 GET 请求
    UniTask<IResponse> Get(WebGetRequest request, CancellationToken token);

    // 发送 POST 请求
    UniTask<IResponse> Post(WebPostRequest request, CancellationToken token);
}
```

## 请求类

### WebGetRequest

```csharp
public sealed class WebGetRequest : WebRequestBase
{
    public string Url { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public int Timeout { get; set; }
}
```

### WebPostRequest

```csharp
public sealed class WebPostRequest : WebRequestBase
{
    public string Url { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public byte[] Body { get; set; }
    public int Timeout { get; set; }
}
```

## 基本用法

### 1. 设置助手

```csharp
// 通常使用 UnityWebRequest
WebRequestModule.Instance.SetWebRequestHelper(new UnityWebRequestHelper());
```

### 2. 发送 GET 请求

```csharp
var request = new WebGetRequest
{
    Url = "https://api.example.com/user/123",
    Headers = new Dictionary<string, string>
    {
        ["Authorization"] = "Bearer token"
    },
    Timeout = 30
};

var response = await WebRequestModule.Instance.Get(request, CancellationToken.None);

if (response.ErrorCode == 0)
{
    string data = response.GetString();
    Debug.Log(data);
}
```

### 3. 发送 POST 请求

```csharp
var request = new WebPostRequest
{
    Url = "https://api.example.com/login",
    Headers = new Dictionary<string, string>
    {
        ["Content-Type"] = "application/json"
    },
    Body = System.Text.Encoding.UTF8.GetBytes("{\"username\":\"test\",\"password\":\"123\"}"),
    Timeout = 30
};

var response = await WebRequestModule.Instance.Post(request, CancellationToken.None);
```

## UnityWebRequestHelper 实现

```csharp
public class UnityWebRequestHelper : IWebRequestHelper
{
    public async UniTask<IResponse> Get(WebGetRequest request, CancellationToken token)
    {
        using (var uwr = UnityWebRequest.Get(request.Url))
        {
            foreach (var header in request.Headers)
            {
                uwr.SetRequestHeader(header.Key, header.Value);
            }

            uwr.timeout = request.Timeout;

            await uwr.SendWebRequest();

            return CreateResponse(uwr);
        }
    }

    public async UniTask<IResponse> Post(WebPostRequest request, CancellationToken token)
    {
        using (var uwr = UnityWebRequest.Post(request.Url, "POST"))
        {
            foreach (var header in request.Headers)
            {
                uwr.SetRequestHeader(header.Key, header.Value);
            }

            uwr.uploadHandler = new UploadHandlerRaw(request.Body);
            uwr.timeout = request.Timeout;

            await uwr.SendWebRequest();

            return CreateResponse(uwr);
        }
    }

    private IResponse CreateResponse(UnityWebRequest uwr)
    {
        var response = CommonResponse.Create(
            uwr.isNetworkError || uwr.isHttpError ? -1 : 0,
            uwr.downloadHandler.data
        );
        return response;
    }

    public void Dispose() { }
}
```

## 完整示例

### HTTP 客户端

```csharp
public class HttpClient
{
    private IWebRequestHelper _helper;

    public void Initialize()
    {
        _helper = new UnityWebRequestHelper();
        WebRequestModule.Instance.SetWebRequestHelper(_helper);
    }

    public async UniTask<UserData> GetUserAsync(int userId)
    {
        var request = new WebGetRequest
        {
            Url = $"https://api.example.com/users/{userId}"
        };

        var response = await WebRequestModule.Instance.Get(request, CancellationToken.None);

        if (response.ErrorCode == 0)
        {
            string json = System.Text.Encoding.UTF8.GetString(response.GetBytes());
            return JsonUtility.FromJson<UserData>(json);
        }

        throw new Exception($"Failed to get user: {response.ErrorCode}");
    }

    public async UniTask<bool> LoginAsync(string username, string password)
    {
        var json = JsonUtility.ToJson(new { username, password });
        var request = new WebPostRequest
        {
            Url = "https://api.example.com/login",
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json"
            },
            Body = System.Text.Encoding.UTF8.GetBytes(json)
        };

        var response = await WebRequestModule.Instance.Post(request, CancellationToken.None);
        return response.ErrorCode == 0;
    }

    public void Dispose()
    {
        _helper?.Dispose();
    }
}
```

## 最佳实践

### 1. 使用 CancellationToken

```csharp
private CancellationTokenSource _cts;

public async UniTask FetchDataAsync()
{
    _cts = new CancellationTokenSource();

    try
    {
        var response = await WebRequestModule.Instance.Get(request, _cts.Token);
        // 处理响应
    }
    catch (OperationCanceledException)
    {
        Debug.Log("Request cancelled");
    }
}

public void Cancel()
{
    _cts?.Cancel();
}
```

### 2. 设置超时

```csharp
request.Timeout = 30; // 30 秒超时
```

### 3. 添加错误处理

```csharp
public async UniTask<Result<T>> TryGetAsync<T>(string url)
{
    try
    {
        var response = await WebRequestModule.Instance.Get(new WebGetRequest { Url = url });
        if (response.ErrorCode == 0)
        {
            var data = JsonUtility.FromJson<T>(response.GetString());
            return Result<T>.Success(data);
        }
        return Result<T>.Failure(response.ErrorCode);
    }
    catch (Exception ex)
    {
        return Result<T>.Failure(-1, ex.Message);
    }
}
```