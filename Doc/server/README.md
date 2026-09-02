# Doc/server/

父索引：[`../README.md`](../README.md)。约定：[`../../AGENTS/domains/server.md`](../../AGENTS/domains/server.md)。

## 本目录用途

记录服务端在本仓库的**实际**状态，避免把空目录或参考工程当成可启动生产服。

## 子文件

无。

## 子目录

无。

## 现状（以磁盘为准）

- `Server/`：未见可编译的服务端源码工程（几乎无 `.cs`）。不能假设存在 Gateway/GameServer。
- `GeekServer/`：目前主要是 IDE 元数据，不能当作本项目默认服务端。
- 客户端网络：`HoweFramework/Network/` + `GameMain/Scripts/Network/`（如 `NetworkConst`）。协议联调前先确认对端是否另有仓库。

若后续加入真实服务端工程，在本目录新增模块文档，并更新本 README 与 `Doc/README.md` 的「先读哪篇」。
