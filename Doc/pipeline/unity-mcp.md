# Unity MCP（Cursor）

把 Cursor 接到 Unity Editor：CLI 装插件、本机起 MCP Server、Cursor 用 HTTP 调工具。上游：[IvanMurzak/Unity-MCP](https://github.com/IvanMurzak/Unity-MCP)。

## 职责

让 Agent 在 Editor 里查场景、改 GameObject、跑测试、读写脚本。不进入运行时玩法分层，也不改 `HoweFramework` 契约。

## 本仓库现状

| 项 | 值 |
|----|----|
| Unity 工程 | `Client/` |
| 插件 | `com.ivanmurzak.unity.mcp` `0.90.0`（OpenUPM） |
| 连接模式 | **Custom / 本机**，`http://localhost:22516` |
| Cursor 配置 | `Client/.cursor/mcp.json`（仓库根 `.gitignore` 忽略 `.cursor`） |
| 凭证 | `~/.ai-game-dev/credentials.json`（机器级，勿提交） |

默认的 Cloud（`https://ai-game.dev/mcp`）要等 Unity 插件真正连上云端会话后，CLI / Cursor 的 token 才会被接受。本机 Unity 已打开、且 GitHub 上下载 `gamedev-mcp-server` 可用时，用 Custom 更稳：Editor 拉起本机 Server，Cursor 直连 `localhost`。

## 用法

本机已装 Node.js `^20.19.0` 或 `>=22.12.0`。

```bash
npm install -g unity-mcp-cli
unity-mcp-cli install-plugin Client
unity-mcp-cli login
```

`login` 走浏览器 OAuth。若 Unity **当时已经开着**，登录完成后必须重启 Editor，否则插件仍用旧会话，云端会 401。

本仓库当前走本机模式（工程已 pin 过，一般不用再做）：

```bash
# 用 UserSettings 里已有 token pin 到 localhost（UserSettings 已被 Client/.gitignore 忽略）
unity-mcp-cli bootstrap-local --url http://localhost:22516 --token <local-token> Client
unity-mcp-cli setup-mcp cursor --transport http --url http://localhost:22516 --no-pin Client
unity-mcp-cli open --url http://localhost:22516 --keep-connected --start-server true --transport streamableHttp --auth none Client
unity-mcp-cli wait-for-ready Client
unity-mcp-cli setup-skills cursor Client
```

检查：

```bash
unity-mcp-cli status Client
```

Cursor 侧启用 MCP server `ai-game-developer`（必要时 Reload MCP）。配置形如：

```json
{
  "mcpServers": {
    "ai-game-developer": {
      "type": "http",
      "url": "http://localhost:22516"
    }
  }
}
```

Unity 菜单：`Window / AI Game Developer`。本机 Server 二进制在 `Client/Library/mcp-server/osx-arm64/gamedev-mcp-server`（`Library/` 不入库）。

## 约束与坑

- 工程路径不能有空格（上游限制）。
- `unity-mcp-cli close` 依赖 `Temp/UnityLockfile`。锁文件为空时可能误匹配 AssetImportWorker，关不掉主 Editor；此时对主进程发 `SIGTERM`。
- 首次从 GitHub 拉 `gamedev-mcp-server` 可能 TLS 失败。重新 `open` 工程会再下；下成功后 `Library/mcp-server/<rid>/version` 会写上。
- 插件会在 `Client/Assets/Plugins/NuGet/` 写入依赖 DLL。卸载时按上游说明删该目录。
- OpenUPM scoped registry 会加上 `com.ivanmurzak`、`extensions.unity`（与已有 `com.tuyoogame.yooasset` 并列），见 `Client/Packages/manifest.json`。

## 相关源码

- `Client/Packages/manifest.json`
- `Client/UserSettings/AI-Game-Developer-Config.json`（本机，不入库）
- `Client/.cursor/mcp.json`、`Client/.cursor/skills/`（本机，不入库）
