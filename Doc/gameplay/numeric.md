# NumericComponent

## 职责

按属性 id 存 long 值，分 `NumericSubType` 计算最终值。不是 HP 槽（消耗用 `ResourceComponent`）。

## 关键类型

- `NumericComponent`：`INumeric`，`[GameComponent(Numeric)]`
- `NumericSubType`：Final、Basic、BasicPercent、BasicConstAdd、FinalPercent、FinalConstAdd
- 公式（枚举注释）：`Final = (Basic * (1 + BasicPercent) + BasicConstAdd) * (1 + FinalPercent) + FinalConstAdd`
- `NumericHelper.EncodeNumericKey` / `DecodeNumericKey`
- 变更：`SimpleEvent<long>`（`Subscribe` 类 API 在组件内）

`this[id]` get 的是 Final；**set Final 会抛**「不允许直接设置最终值」。

## 用法

```csharp
numeric.Set(attrId, NumericSubType.Basic, 100);
long final = numeric[attrId];
```

## 扩展点

业务自己分配 attr id 常量，不要复用框架未定义的魔法数而不文档化。需要监听用组件内事件字典。

## 约束与坑

- 只改子项，让组件重算 Final。
- 与 `ResourceComponent` 分工：一个是属性面板，一个是当前存量。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Common/Numeric/NumericComponent.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Common/Numeric/NumericSubType.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Common/Numeric/NumericHelper.cs`
