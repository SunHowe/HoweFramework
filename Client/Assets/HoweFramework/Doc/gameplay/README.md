# Gameplay 框架概览

## 概述

Gameplay 框架是构建于 HoweFramework 之上的游戏逻辑框架，采用 **Entity-Component** 架构，实现游戏逻辑与 Unity 对象（GameObject）的分离。

## 目录结构

```
Assets/GameMain/Scripts/Gameplay/
├── Framework/
│   ├── Entity/           # 实体组件核心
│   │   ├── GameEntityManager.cs
│   │   ├── GameEntityManager.GameEntity.cs
│   │   ├── GameComponentBase.cs
│   │   └── ...
│   ├── View/             # 视图系统
│   ├── Update/           # 更新管理
│   ├── Timer/            # 定时器
│   ├── Random/           # 随机数
│   ├── Expression/       # 表达式
│   └── Helper/           # 辅助工具
├── Common/               # 通用组件
│   ├── Transform/        # 变换组件
│   ├── View/             # 视图组件
│   ├── Numeric/          # 数值组件
│   ├── State/            # 状态组件
│   └── Resource/         # 资源组件
├── Mono/                 # Unity 集成
│   ├── GameEntityConverter.cs
│   ├── TransformComponentConverter.cs
│   └── ...
└── Doc/
    └── README.md
```

## 核心概念

### 1. Entity（实体）

实体是游戏对象的唯一标识，不包含任何数据或逻辑。

```csharp
public interface IGameEntity
{
    int EntityId { get; }
    IGameContext Context { get; }

    void Awake(int entityId, IGameEntityManager entityManager);
    void AddComponent(IGameComponent component);
    IGameComponent GetComponent(int componentType);
    void RemoveComponent(int componentType);
    void GetComponents(List<IGameComponent> components);
    void Dispose();
}
```

### 2. Component（组件）

组件是数据的容器，附加到实体上。

```csharp
public interface IGameComponent
{
    int ComponentType { get; }
    int ComponentId { get; }
    IGameEntity Entity { get; }

    void Awake(int componentId, IGameEntity entity);
    void Dispose();
}
```

### 3. Manager（管理器）

管理器负责管理实体和组件的创建、销毁。

```csharp
public interface IGameEntityManager : IGameManager
{
    IGameEntity CreateEntity();
    IGameEntity GetEntity(int entityId);
    void DestroyEntity(int entityId);
    int SpawnComponentId();
}
```

## 文档索引

| 文档 | 说明 |
|------|------|
| [entity-component.md](entity-component.md) | 实体-组件系统 |
| [view-system.md](view-system.md) | 视图系统 |
| [numeric-system.md](numeric-system.md) | 数值系统 |
| [state-component.md](state-component.md) | 状态组件 |
| [resource-component.md](resource-component.md) | 资源组件 |
| [game-context.md](game-context.md) | 游戏上下文 |
| [managers.md](managers.md) | 游戏管理器 |

## 类型枚举

### GameComponentType

| 类型 | 值 | 说明 |
|------|-----|------|
| Transform | 1 | 位置、旋转、缩放 |
| View | 2 | Unity GameObject 关联 |
| ViewTransformSync | 3 | 同步 Transform 到 View |
| Numeric | 4 | 游戏数值/属性 |
| State | 5 | 布尔状态 |
| Resource | 6 | 资源值（HP、MP） |

### GameManagerType

| 类型 | 值 | 说明 |
|------|-----|------|
| Update | 1 | 更新/帧循环管理 |
| Random | 2 | 随机数生成 |
| Scene | 3 | 场景对象管理 |
| View | 4 | 视图对象管理 |
| Timer | 5 | 计时器管理 |
| Expression | 6 | 表达式求值 |
| Entity | 100 | 实体管理 |

## 快速开始

### 创建实体

```csharp
var entity = context.GetManager<IGameEntityManager>().CreateEntity();
```

### 添加组件

```csharp
entity.AddComponent<TransformComponent>();
entity.AddComponent<NumericComponent>();
```

### 获取组件

```csharp
var transform = entity.GetComponent<TransformComponent>();
```

### 销毁实体

```csharp
context.GetManager<IGameEntityManager>().DestroyEntity(entity.EntityId);
```

## 关键文件

| 文件 | 路径 |
|------|------|
| IGameEntity | `Gameplay/Framework/Entity/IGameEntity.cs` |
| IGameComponent | `Gameplay/Framework/Entity/IGameComponent.cs` |
| GameEntityManager | `Gameplay/Framework/Entity/GameEntityManager.cs` |
| GameContextBase | `Gameplay/Framework/GameContextBase.cs` |
| GameComponentType | `Gameplay/Framework/Entity/GameComponentType.cs` |
| GameManagerType | `Gameplay/Framework/GameManagerType.cs` |