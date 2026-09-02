# 服务端域

以仓库**实际存在的工程**为准，不要把文档或参考仓库当成可运行的生产服。

知识：[`Doc/server/README.md`](../../Doc/server/README.md)。

## 目录

| 路径 | 角色 |
|------|------|
| `Server/` | 自研服务端；若几乎无源码，先读 Doc 再决定能否改 |
| `GeekServer/` | 参考实现，不是本项目默认启动入口 |

客户端网络契约在 `HoweFramework/Network/` 与 `GameMain` 的网络/协议代码。改协议时同时核对客户端发包/收包，并按 [`distill.md`](../workflow/distill.md) 更新 `Doc/framework/modules/network.md` 或 `Doc/server/`。

## 不要

- 不要在 `GeekServer/` 里「顺便」做本项目需求，除非任务明确说改参考实现。
- 不要假设存在 `Server/AGENTS.md`（当前没有独立入口，统一走仓库根 `AGENTS.md`）。
