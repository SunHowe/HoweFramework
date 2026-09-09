# FsmMachine

## 职责

通用有限状态机，**不是** `ModuleBase`。带黑板。应用级流程请用 `ProcedureModule`，不要用 Fsm 替代。

## 关键类型

- `FsmMachine`：`Create()`、`AddState`、`ChangeState`、`CurrentState`、`Blackboard`、`OnStateEnter`/`OnStateExit`，`IReference`
- `FsmStateBase`；扩展 `AddState<T>`
- 状态 id 不能为 0；**0 表示停机**

## 用法

```csharp
var fsm = FsmMachine.Create();
fsm.AddState(stateId);
fsm.ChangeState(stateId);
fsm.Dispose(); // 内部 ChangeState(0) 再入池
```

## 扩展点

进入/退出可用事件或 `FsmStateBase`。黑板存跨状态数据。

## 约束与坑

- 与 `ProcedureModule` 职责不同：Procedure 是全局唯一运行中的应用阶段。
- 玩法状态机可以自建 Fsm，不要改 Procedure 去模拟技能状态。
- **重入约定**：在状态进入/退出回调（`OnStateEnter`/`OnStateExit`、注册的 enter/exit handler）中调用 `ChangeState` 是允许的，切换请求会延迟到本次切换完成后生效；多次重入以最后一次为准。同一次切换最多 32 次重入切换，超出打错误日志并停止，防止回调互相切换卡死主线程。
- **回调中 Dispose**：若在进入/退出回调里 `Dispose`，会先切到停机（状态 0），再延迟 `Release`，避免使用中的实例入池污染下一个租用者。
- `FsmMachine` 走引用池：`Dispose` 后必须重新 `Create`；`Clear` 会清空 `OnStateEnter`/`OnStateExit` 订阅与黑板，复用实例不会残留旧订阅。

## 相关源码

- `Client/Assets/HoweFramework/Fsm/FsmMachine.cs`
- `Client/Assets/HoweFramework/Fsm/IFsmMachine.cs`
- `Client/Assets/HoweFramework/Fsm/FsmStateBase.cs`
