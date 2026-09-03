# ResourceComponent

## 职责

管理可涨可扣的存量（注释写明血量、魔法值等）。`Cost` 不足返回 false，不扣成负数。

## 关键类型

- `ResourceComponent`：`Get`/`Set`/`Modify`/`Cost`、`SetMax`、`RecoverToMax`、`BindNumericMax`、索引器

与 `NumericComponent`：Numeric 是计算公式后的属性；Resource 是当前值/消耗。

## 用法

```csharp
resource[hpId] = 100;
resource.Modify(hpId, -10);
bool ok = resource.Cost(mpId, 20);
```

## 扩展点

资源 id 由业务常量定义。上限用 `SetMax`，回满用 `RecoverToMax`，上限跟 Numeric 走时用 `BindNumericMax`。不要另写一套平行组件。GAS Instant/Period/Cost 默认 Execute 到本组件，见 [`ability-system.md`](ability-system.md)。

## 约束与坑

- id 冲突会串资源。
- Dispose 时清字典与事件。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Common/Resource/ResourceComponent.cs`
