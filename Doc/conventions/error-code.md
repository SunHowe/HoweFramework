# 错误码

成功为 **0**（`FrameworkErrorCode.Success`）。异步请求用 `IResponse.ErrorCode`，不要用异常当正常失败路径（`RequestBase` 会把异常收成响应）。

## 分层

- 框架：`HoweFramework.FrameworkErrorCode`
- 业务：`GameMain.ErrorCode` 目前只是把框架通用码（Success/Exception/InvalidParam 等）转出来；业务特有码应在此扩展并避开框架已用区间。

## 框架已用段（摘要）

| 段 | 含义 |
|----|------|
| 0–5 | 通用成功/异常/参数 |
| 100–112 | UI |
| 201–209 | 资源/场景 |
| 301–303 | 流程 |
| 401–413 | 网络 |
| 501+ | HTTP 映射 |
| 601–603 | 声音 |
| 701–703 | Request |
| 801–802 | Login（框架文件内） |
| 901–910 | 行为树 |

新增框架码加在对应 `#region`，不要复用已有数字。`ErrorCodeException` 带 `ErrorCode`。

## 相关源码

- `Client/Assets/HoweFramework/FrameworkErrorCode.cs`
- `Client/Assets/GameMain/Scripts/ErrorCode.cs`
