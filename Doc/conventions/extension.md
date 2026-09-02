# 何时扩展什么

按范围选原语，避免再实现一套事件/资源/对象池。

| 需求 | 用什么 | 不要 |
|------|--------|------|
| 全应用单例服务，且要进 `GameApp` 生命周期 | 新 `ModuleBase<T>`（须明确要求才能改框架） | 在 GameMain 里仿单例 Module |
| 长期业务服务（登录、公会） | `SystemModule.RegisterSystem` | 新框架模块 |
| 仅某段玩法会话 | `GameContextBase` + Manager | 全局静态 |
| 挂在实体上的能力 | `GameComponentBase` | MonoBehaviour 当逻辑组件 |
| 驱动一堆实体/规则 | `GameManagerBase` | 在组件里扫全世界 |
| 应用阶段（闪屏/登录） | `ProcedureBase` | `FsmMachine` 当启动流程 |
| 局部状态图 | `FsmMachine` | 改 Procedure |
| 异步打开 UI / 发包 / HTTP | `RequestBase` | 裸 UniTask 没有错误码约定 |
| 当前是否有某标记 | `StateComponent` | 在 Numeric 里用 0/1 冒充 |
| 属性计算公式 | `NumericComponent` | 用 Resource 存攻击力面板 |
| 当前血量/蓝量 | `ResourceComponent` | 用 Numeric Final 当血条 |

加组件 / Manager 的枚举规则见 [`naming.md`](naming.md)。加完若形成可重复步骤，按沉淀协议更新本页或 gameplay 文档。
