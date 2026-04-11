# 状态机系统

## 概述

FSM（Finite State Machine）模块提供轻量级的状态机实现。

## 核心文件

| 文件 | 路径 |
|------|------|
| FsmMachine | `Assets/HoweFramework/Fsm/FsmMachine.cs` |
| FsmStateBase | `Assets/HoweFramework/Fsm/FsmStateBase.cs` |
| IFsmMachine | `Assets/HoweFramework/Fsm/IFsmMachine.cs` |

## IFsmMachine

```csharp
public interface IFsmMachine : IDisposable
{
    event FsmStateChangeHandler OnStateEnter;
    event FsmStateChangeHandler OnStateExit;

    int CurrentState { get; }
    IBlackboard Blackboard { get; }

    void AddState(int stateId);
    void ChangeState(int stateId);

    // 注册状态进入/退出回调
    void RegisterStateEnter(int stateId, FsmStateHandler handler);
    void RegisterStateExit(int stateId, FsmStateHandler handler);
}

public delegate void FsmStateHandler();
public delegate void FsmStateChangeHandler(int stateId);
```

## FsmMachine

```csharp
public sealed class FsmMachine : IFsmMachine, IReference
{
    public event FsmStateChangeHandler OnStateEnter;
    public event FsmStateChangeHandler OnStateExit;

    public int CurrentState { get; private set; }
    public IBlackboard Blackboard { get; } = new Blackboard();

    public void AddState(int stateId);
    public void ChangeState(int stateId);
    public void RegisterStateEnter(int stateId, FsmStateHandler handler);
    public void RegisterStateExit(int stateId, FsmStateHandler handler);
    public void Clear();

    public static FsmMachine Create();
}
```

## FsmStateBase

使用 `FsmStateBase` 的另一种方式：

```csharp
public abstract class FsmStateBase
{
    protected IFsmMachine Fsm { get; }
    protected IBlackboard Blackboard => Fsm?.Blackboard;

    protected abstract int State { get; }

    internal void Initialize(IFsmMachine fsm);
    protected void ChangeState(int stateId);

    protected abstract void OnEnter();
    protected abstract void OnExit();
}
```

## 基本用法

### 方式一：使用 FsmMachine（直接注册）

```csharp
// 创建状态机
var fsm = FsmMachine.Create();

// 添加状态
fsm.AddState((int)PlayerState.Idle);
fsm.AddState((int)PlayerState.Run);
fsm.AddState((int)PlayerState.Attack);

// 注册状态回调
fsm.RegisterStateEnter((int)PlayerState.Attack, () =>
{
    Debug.Log("Enter Attack");
    PlayAnimation("Attack");
});

fsm.RegisterStateExit((int)PlayerState.Attack, () =>
{
    Debug.Log("Exit Attack");
    StopAnimation("Attack");
});

// 切换状态
fsm.ChangeState((int)PlayerState.Attack);

// 使用 Blackboard 共享数据
fsm.Blackboard.SetValue("CanMove", false);

// 销毁
fsm.Dispose();
```

### 方式二：使用 FsmStateBase（OOP 风格）

```csharp
// 定义状态
public class PlayerIdleState : FsmStateBase
{
    protected override int State => (int)PlayerState.Idle;

    protected override void OnEnter()
    {
        Blackboard.SetValue("CanMove", true);
        PlayAnimation("Idle");
    }

    protected override void OnExit()
    {
        StopAnimation("Idle");
    }
}

public class PlayerAttackState : FsmStateBase
{
    protected override int State => (int)PlayerState.Attack;

    protected override void OnEnter()
    {
        Blackboard.SetValue("CanMove", false);
        PlayAnimation("Attack");
    }

    protected override void OnExit()
    {
        StopAnimation("Attack");
    }
}

// 使用
var fsm = FsmMachine.Create();

fsm.AddState((int)PlayerState.Idle);
fsm.AddState((int)PlayerState.Run);
fsm.AddState((int)PlayerState.Attack);

var idleState = new PlayerIdleState();
idleState.Initialize(fsm);

var attackState = new PlayerAttackState();
attackState.Initialize(fsm);

// 切换（自动调用 OnEnter/OnExit）
fsm.ChangeState((int)PlayerState.Attack);
```

## 状态枚举

```csharp
public enum PlayerState
{
    Idle = 0,
    Run = 1,
    Attack = 2,
    Skill = 3,
    Dead = 4,
}
```

## 状态转换规则

```csharp
public void Update()
{
    if (fsm.CurrentState == (int)PlayerState.Idle)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            fsm.ChangeState((int)PlayerState.Attack);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            fsm.ChangeState((int)PlayerState.Run);
        }
    }
    else if (fsm.CurrentState == (int)PlayerState.Run)
    {
        if (!Input.GetKey(KeyCode.W))
        {
            fsm.ChangeState((int)PlayerState.Idle);
        }
    }
    else if (fsm.CurrentState == (int)PlayerState.Attack)
    {
        // 动画播放完毕
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            fsm.ChangeState((int)PlayerState.Idle);
        }
    }
}
```

## Blackboard 用法

```csharp
// 设置值
fsm.Blackboard.SetValue("CanMove", true);
fsm.Blackboard.SetValue("Speed", 5f);
fbm.Blackboard.SetValue("Target", enemyObject);

// 获取值
bool canMove = fsm.Blackboard.GetValue<bool>("CanMove");
float speed = fsm.Blackboard.GetValue<float>("Speed");
GameObject target = fsm.Blackboard.GetValue<GameObject>("Target");

// 检查存在
if (fsm.Blackboard.HasValue("Target"))
{
    // ...
}

// 移除
fsm.Blackboard.RemoveValue("Target");
```

## 完整示例

```csharp
public class Player : MonoBehaviour
{
    private IFsmMachine _fsm;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 创建状态机
        _fsm = FsmMachine.Create();

        // 添加状态
        _fsm.AddState((int)PlayerState.Idle);
        _fsm.AddState((int)PlayerState.Run);
        _fsm.AddState((int)PlayerState.Attack);

        // 注册事件
        _fsm.OnStateEnter += OnStateEnter;
        _fsm.OnStateExit += OnStateExit;

        // 设置初始状态
        _fsm.ChangeState((int)PlayerState.Idle);
    }

    private void Update()
    {
        UpdateStateTransition();
    }

    private void UpdateStateTransition()
    {
        switch (_fsm.CurrentState)
        {
            case (int)PlayerState.Idle:
                if (Input.GetKey(KeyCode.W))
                    _fsm.ChangeState((int)PlayerState.Run);
                else if (Input.GetKeyDown(KeyCode.J))
                    _fsm.ChangeState((int)PlayerState.Attack);
                break;

            case (int)PlayerState.Run:
                if (!Input.GetKey(KeyCode.W))
                    _fsm.ChangeState((int)PlayerState.Idle);
                else if (Input.GetKeyDown(KeyCode.J))
                    _fsm.ChangeState((int)PlayerState.Attack);
                break;

            case (int)PlayerState.Attack:
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    _fsm.ChangeState((int)PlayerState.Idle);
                break;
        }
    }

    private void OnStateEnter(int stateId)
    {
        Debug.Log($"Enter state: {(PlayerState)stateId}");
    }

    private void OnStateExit(int stateId)
    {
        Debug.Log($"Exit state: {(PlayerState)stateId}");
    }

    private void OnDestroy()
    {
        _fsm.Dispose();
    }
}
```

## 与其他系统对比

| 系统 | 适用场景 |
|------|----------|
| FSM | 简单状态切换 |
| BehaviorTree | 复杂 AI 决策 |
| Procedure | 游戏主流程 |

## 最佳实践

### 1. 避免在 OnEnter/OnExit 中执行耗时操作

```csharp
// 不推荐
protected override void OnEnter()
{
    // 异步操作可能导致状态不一致
    LoadResourceAsync();
}

// 推荐
protected override void OnEnter()
{
    _state = State.Loading;
    LoadResourceAsync().ContinueWith(() =>
    {
        _state = State.Ready;
    });
}
```

### 2. 使用枚举替代魔法数字

```csharp
// 好
fsm.ChangeState((int)PlayerState.Idle);

// 不好
fsm.ChangeState(0);
```

### 3. 记得 Dispose

```csharp
private void OnDestroy()
{
    _fsm?.Dispose();
}
```