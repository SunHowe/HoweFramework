# BehaviorModule

## 职责

行为树加载。持有 `GlobalBehaviorLoader`；也可 `CreateBehaviorLoader(resLoader)`。

## 关键类型

- `BehaviorModule`
- `IBehaviorLoader` / `BehaviorLoader`
- 运行时节点：`BehaviorNodeBase`、Sequence/Selector/Parallel、装饰节点
- 配置：`BehaviorTreeConfig`、`BehaviorNodeConfig`
- 错误码 901–910（含 `BehaviorRunningState` 表示节点还在 Running）

编辑器图在 `HoweFramework.Editor/BehaviorTree/`，不要把图编辑代码当运行时 API。

## 用法

用全局或局部 loader 加载配置并创建树。局部 loader 传入的 `IResLoader` 为 null 时会自己分配加载器。模块销毁会 Dispose 全局 loader。

## 扩展点

自定义 Action/Condition 节点并注册到加载逻辑（以 `BehaviorLoader` 与节点配置为准）。Editor 侧加 `BehaviorNodeTemplate`。

## 约束与坑

- 装饰节点只能有一个子节点（902/903）。
- Running 用错误码 901 表达「下次从这里继续」，不是崩溃。
- 局部 loader 必须 Dispose。

## 相关源码

- `Client/Assets/HoweFramework/BehaviorTree/BehaviorModule.cs`
- `Client/Assets/HoweFramework/BehaviorTree/BehaviorLoader.cs`
- `Client/Assets/HoweFramework.Editor/BehaviorTree/`
