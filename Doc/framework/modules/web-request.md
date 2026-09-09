# WebRequestModule

## 职责

HTTP GET/POST，经 `IWebRequestHelper`，返回带错误码的响应。`GameApp` 使用 `UseUnityWebRequest()`。

## 关键类型

- `WebRequestModule`：`SetWebRequestHelper`；内部 `Get`/`Post`
- `WebGetRequest` / `WebPostRequest`（`RequestBase`）
- 非 2xx 映射到 `FrameworkErrorCode` WebRequest 段（501 起）；`HttpVersionNotSupported` 为 533（曾误标 505 与 `NotFound` 重复，已修正）

未设 Helper 或 POST 无 ContentType 会抛 `InvalidOperationException` / `InvalidParam`。

## 用法

通过对应 Request 的 `Execute`，不要直接调 internal `Get`/`Post`。

## 扩展点

`IWebRequestHelper` 可换成非 UnityWebRequest 实现。

## 约束与坑

- 与 `RemoteRequestModule` 不是同一层。
- HTTP 状态码映射表在 `WebRequestModule`；2xx 均算成功（不再只认 200）。
- GET 的 `Parameters` 会做 URL 编码后拼接查询串。
- 默认请求超时 30 秒（`UnityWebRequestHelper`），超时/取消经 `RequestBase` 统一回 `RequestCanceled`。
- `UnityWebRequest` 实例随请求结束 `Dispose`，无原生内存泄漏。

## 相关源码

- `Client/Assets/HoweFramework/WebRequest/WebRequestModule.cs`
