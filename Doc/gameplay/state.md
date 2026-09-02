# StateComponent

## 职责

记录「当前有哪些 stateId」，每个 id 对应一组 provider。有任意 provider 则状态为真。这不是 buff 系统（没有 duration/tick），只是集合。

## 关键类型

- `StateComponent`：`CheckState`、`AddState(state, provider)`、`RemoveState(state, provider)`、`RemoveState(state)` 清该 id 全部引用
- 变化派发 `SimpleEvent<bool>`（出现/消失）

同一 state 多个 provider 时，只有集合变空才派发 false。

## 用法

```csharp
state.AddState(stateId, this);
if (state.CheckState(stateId)) { }
state.RemoveState(stateId, this);
```

## 扩展点

Buff 应作为 provider 持有自己的时间逻辑，在添加/到期时调 StateComponent。不要把持续时间塞进 StateComponent。

## 约束与坑

- provider 必须能稳定 Remove，避免泄漏导致状态永不消失。
- `OnDispose` 需释放内部 `ReusableHashSet`（见源码）。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Common/State/StateComponent.cs`
