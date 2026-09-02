# HoweFramework.Editor

## 职责

仅编辑器：FairyGUI 资源导入与代码生成、YooAsset 收集器、行为树图编辑、ObjectCollector/ObjectExport 检视器。

## 关键目录

| 路径 | 用途 |
|------|------|
| `Client/Assets/HoweFramework.Editor/FairyGUI/` | 导入、目录设置、代码生成 |
| `Client/Assets/HoweFramework.Editor/YooAsset/` | `CollectTextAsset`、`CollectTexture` |
| `Client/Assets/HoweFramework.Editor/BehaviorTree/` | `BehaviorGraphWindow` 与节点模板 |
| `Client/Assets/HoweFramework.Editor/Inspector/` | ObjectCollector / ObjectExport |
| `Client/Assets/HoweFramework.Editor/Helper/` | `CodeGenerateHelper` |

运行时 UI/资源/行为树逻辑在 `HoweFramework`，不要把运行时依赖写进 Editor 程序集，也不要在 Editor 里实现业务玩法。

## 相关源码

- `Client/Assets/HoweFramework.Editor/HoweFramework.Editor.asmdef`
