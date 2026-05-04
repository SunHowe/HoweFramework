# 行为树系统

## 概述

BehaviorModule 提供基于行为树的 AI 决策框架。

## 核心文件

| 文件 | 路径 |
|------|------|
| BehaviorModule | `Assets/HoweFramework/BehaviorTree/BehaviorModule.cs` |
| BehaviorLoader | `Assets/HoweFramework/BehaviorTree/BehaviorLoader.cs` |
| IBehaviorLoader | `Assets/HoweFramework/BehaviorTree/IBehaviorLoader.cs` |
| BehaviorTreeConfig | `Assets/HoweFramework/BehaviorTree/Config/BehaviorTreeConfig.cs` |
| IBehaviorNode | `Assets/HoweFramework/BehaviorTree/Core/IBehaviorNode.cs` |

## 节点类型

### Composite Nodes（组合节点）

| 节点 | 说明 |
|------|------|
| BehaviorSequence | 顺序执行，遇到失败停止 |
| BehaviorSelector | 选择执行，遇到成功停止 |
| BehaviorParallel | 并行执行 |

### Decorator Nodes（装饰节点）

| 节点 | 说明 |
|------|------|
| BehaviorRepeat | 重复执行 |
| BehaviorAlwaysFailure | 总是返回失败 |
| BehaviorAlwaysSuccess | 总是返回成功 |

### Action Nodes（动作节点）

| 节点 | 说明 |
|------|------|
| BehaviorFailure | 返回失败 |
| BehaviorSuccess | 返回成功 |
| BehaviorLog | 输出日志 |

## BehaviorModule API

```csharp
public sealed class BehaviorModule : ModuleBase<BehaviorModule>
{
    public IBehaviorLoader GlobalBehaviorLoader { get; }

    // 创建加载器
    public IBehaviorLoader CreateBehaviorLoader(IResLoader resLoader = null);
}
```

## IBehaviorLoader

```csharp
public interface IBehaviorLoader : IDisposable
{
    // 加载行为树
    UniTask<BehaviorRoot> LoadBehaviorTree(string assetKey, CancellationToken token = default);

    // 清空
    void Clear();
}
```

## IBehaviorNode

```csharp
public interface IBehaviorNode
{
    BehaviorNodeType NodeType { get; }
    string Name { get; set; }

    void Evaluate(object userData);
    BehaviorResult Execute(object userData);
    void Reset(object userData);
}

public enum BehaviorNodeType
{
    Action,
    Composite,
    Condition,
    Decorator,
}
```

## BehaviorRoot

```csharp
public sealed class BehaviorRoot : IBehaviorNode
{
    public IBehaviorNode Child { get; set; }

    public void Evaluate(object userData);
    public BehaviorResult Execute(object userData);
    public void Reset(object userData);
}

public enum BehaviorResult
{
    Success,
    Failure,
    Running,
}
```

## 基本用法

### 1. 加载行为树

```csharp
// 创建加载器
var loader = BehaviorModule.Instance.CreateBehaviorLoader();

// 加载行为树配置
var behaviorTree = await loader.LoadBehaviorTree("BehaviorTrees/EnemyAI");

// 执行
behaviorTree.Execute(context);

// 重置
behaviorTree.Reset(context);
```

### 2. 配置格式

行为树通常以 JSON 或二进制格式存储：

```json
{
  "name": "EnemyAI",
  "type": "Composite",
  "compositeType": "Selector",
  "children": [
    {
      "type": "Condition",
      "conditionType": "HasTarget"
    },
    {
      "type": "Action",
      "actionType": "Attack"
    }
  ]
}
```

## 自定义行为节点

### 自定义 Action

```csharp
public class MoveToTargetAction : BehaviorActionNodeBase
{
    protected override void OnInit()
    {
    }

    protected override BehaviorResult DoExecute(object userData)
    {
        var context = (BattleContext)userData;
        if (context.Target == null)
            return BehaviorResult.Failure;

        // 移动向目标
        _agent.SetDestination(context.Target.Position);

        if (_agent.remainingDistance < 0.5f)
            return BehaviorResult.Success;

        return BehaviorResult.Running;
    }
}
```

### 自定义 Condition

```csharp
public class HasTargetCondition : BehaviorConditionNodeBase
{
    protected override bool OnEvaluate(object userData)
    {
        var context = (BattleContext)userData;
        return context.Target != null;
    }
}
```

## 上下文对象

```csharp
public class BattleContext
{
    public IGameEntity Self { get; set; }
    public IGameEntity Target { get; set; }
    public Vector3 Position { get; set; }
    public float Distance { get; set; }
}
```

## 完整示例

```csharp
public class Enemy : MonoBehaviour
{
    private BehaviorRoot _behaviorTree;
    private BattleContext _context;

    private async void Start()
    {
        _context = new BattleContext
        {
            Self = this.entity
        };

        var loader = BehaviorModule.Instance.CreateBehaviorLoader();
        _behaviorTree = await loader.LoadBehaviorTree($"BehaviorTrees/Enemy_{enemyType}");
    }

    private void Update()
    {
        // 更新上下文
        _context.Target = FindNearestEnemy();

        // 执行行为树
        if (_behaviorTree != null)
        {
            _behaviorTree.Execute(_context);
        }
    }
}
```

## 最佳实践

### 1. 避免每帧创建新上下文

```csharp
// 好
private BattleContext _context;

private void Start()
{
    _context = new BattleContext();
}

// 好：复用上下文
private void Update()
{
    _context.Self = entity;
    _context.Target = FindTarget();
    _behaviorTree.Execute(_context);
}
```

### 2. 在 OnInit 中初始化

```csharp
protected override void OnInit()
{
    // 一次性初始化
    _animator = GetComponent<Animator>();
    _agent = GetComponent<NavMeshAgent>();
}
```

### 3. 使用 Running 状态实现持续行为

```csharp
protected override BehaviorResult DoExecute(object userData)
{
    if (!_isMoving)
    {
        // 开始移动
        _agent.SetDestination(_target);
        _isMoving = true;
        return BehaviorResult.Running;
    }

    if (_agent.remainingDistance < 0.5f)
    {
        _isMoving = false;
        return BehaviorResult.Success;
    }

    return BehaviorResult.Running;
}
```

### 4. 记得 Dispose

```csharp
private void OnDestroy()
{
    _behaviorTree?.Reset(null);
    _loader?.Dispose();
}
```