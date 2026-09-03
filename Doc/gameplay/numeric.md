# NumericComponent

## 职责

按属性 id 存 long 值，分 `NumericSubType` 计算最终值。不是 HP 槽（消耗用 `ResourceComponent`）。GAS Overlay 以 ASC 为 source 写入同一套 Basic/Final 子项，不另开 Gas 层。

## 关键类型

- `NumericComponent`：`INumeric`，`ComponentType` 为该类 `TypeId`
- `NumericSubType`：Final、Basic、BasicPercent、BasicConstAdd、FinalPercent、FinalConstAdd、Override
- 公式：
  - 若存在 `Override`（该子项在字典中有键，含 0）：`Final = Override`
  - 否则：`Final = (Basic * (1 + BasicPercent) + BasicConstAdd) * (1 + FinalPercent) + FinalConstAdd`
- 来源叠加：写入必须带 `source`。`Set(id, subType, value, source)` 同子项下来源求和；`RemoveFromSource(source)` 去掉该来源全部贡献
- `NumericHelper.EncodeNumericKey` / `DecodeNumericKey`（子类型占低 4 位）
- 变更：`SimpleEvent<long>`（`Subscribe` 类 API 在组件内）

`this[id]` 只读 Final；**直接 Set Final 会抛**「不允许直接设置最终值」。

## 用法

```csharp
numeric.Set(attrId, NumericSubType.Basic, 100, owner);
numeric.Set(attrId, NumericSubType.BasicConstAdd, 10, equipment);
long final = numeric[attrId];
```

GAS Overlay 以 `AbilitySystemComponent` 为 source 写 BasicConstAdd / BasicPercent / FinalConstAdd / FinalPercent / Override，见 [`ability-system.md`](ability-system.md)。

## 扩展点

业务自己分配 attr id 常量，不要复用框架未定义的魔法数而不文档化。需要监听用组件内事件字典。多来源叠加必须各自带 `source`，不要省略来源。

## 约束与坑

- 只改子项，让组件重算 Final。
- 快照只保存求和后的值，`RestoreSnapshot(snapshot, source)` 把合计写回指定来源。
- 与 `ResourceComponent` 分工：一个是属性面板，一个是当前存量。
- `Override` 用「键是否存在」判断，写入 0 也会覆盖 Final。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Common/Numeric/NumericComponent.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Common/Numeric/NumericSubType.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Common/Numeric/NumericHelper.cs`
