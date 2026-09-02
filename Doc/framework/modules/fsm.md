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

## 相关源码

- `Client/Assets/HoweFramework/Fsm/FsmMachine.cs`
- `Client/Assets/HoweFramework/Fsm/IFsmMachine.cs`
- `Client/Assets/HoweFramework/Fsm/FsmStateBase.cs`
