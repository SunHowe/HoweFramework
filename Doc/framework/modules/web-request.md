# WebRequestModule

## 职责

HTTP GET/POST，经 `IWebRequestHelper`，返回带错误码的响应。`GameApp` 使用 `UseUnityWebRequest()`。

## 关键类型

- `WebRequestModule`：`SetWebRequestHelper`；内部 `Get`/`Post`
- `WebGetRequest` / `WebPostRequest`（`RequestBase`）
- 非 200 映射到 `FrameworkErrorCode` WebRequest 段（501 起）

未设 Helper 或 POST 无 ContentType 会抛 `InvalidOperationException` / `InvalidParam`。

## 用法

通过对应 Request 的 `Execute`，不要直接调 internal `Get`/`Post`。

## 扩展点

`IWebRequestHelper` 可换成非 UnityWebRequest 实现。

## 约束与坑

- 与 `RemoteRequestModule` 不是同一层。
- HTTP 状态码映射表在 `WebRequestModule`，注意源码里 `HttpVersionNotSupported` 常量与 `NotFound` 都曾标成 505，以 `FrameworkErrorCode` 定义为准。

## 相关源码

- `Client/Assets/HoweFramework/WebRequest/WebRequestModule.cs`
