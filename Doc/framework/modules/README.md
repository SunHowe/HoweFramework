# Doc/framework/modules/

各运行时模块。父索引：[`../README.md`](../README.md)。`FsmMachine` 不是 Module，但同属框架原语。

## 本目录用途

按模块查阅职责、用法、扩展点与坑。新增模块文档时在本表加一行。

## 子文件

| 文件 | 一句话 |
|------|--------|
| [`ioc.md`](ioc.md) | 注册/获取/注入，不管生命周期 |
| [`event.md`](event.md) | 全局与局部事件调度器 |
| [`procedure.md`](procedure.md) | 应用级流程状态机 |
| [`ui.md`](ui.md) | FairyGUI 界面打开/关闭、缓存与请求队列 |
| [`res.md`](res.md) | 局部 `IResLoader`，核心为 YooAsset |
| [`network.md`](network.md) | 频道、Packet、异步收发 |
| [`request.md`](request.md) | `RequestBase` 与错误码响应 |
| [`remote-request.md`](remote-request.md) | 请求 id 与响应匹配 |
| [`web-request.md`](web-request.md) | HTTP GET/POST |
| [`data-table.md`](data-table.md) | Luban 数据源与加载模式 |
| [`localization.md`](localization.md) | 多语言文本与源 |
| [`timer.md`](timer.md) | 帧/秒定时器 |
| [`fsm.md`](fsm.md) | `FsmMachine`（非模块） |
| [`reference.md`](reference.md) | `ReferencePool` |
| [`game-object-pool.md`](game-object-pool.md) | GameObject 池 |
| [`scene.md`](scene.md) | Additive 场景与优先级 |
| [`camera.md`](camera.md) | `GameCamera` 与主相机 |
| [`sound.md`](sound.md) | 声音组播放 |
| [`setting.md`](setting.md) | 键值设置 |
| [`safe-area.md`](safe-area.md) | 安全区 |
| [`behavior-tree.md`](behavior-tree.md) | 行为树加载 |
| [`system.md`](system.md) | 业务 System 注册 |
| [`base.md`](base.md) | Json、文本模板、TypeId、黑板 |

## 子目录

无。
