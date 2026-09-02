# FairyGUI

## 职责

界面源工程在 `FGUIProject/`（`FGUIProject.fairy`）。运行时由 `UIModule` + `FairyGUIFormLogicBase` 打开。编辑器：`HoweFramework.Editor/FairyGUI/`（导入、目录设置、代码生成）。

## 用法

1. 在 FairyGUI 编辑器改包。
2. 导入/发布到 Unity 资源目录（以 `FairyGUIResDirectorySetting` 为准）。
3. 用编辑器生成绑定；业务逻辑放在 `GameMain/Scripts/UI/`，继承 `FullScreenFormLogicBase` 等。
4. `UIFormId` 与 bindings 对齐后 `UIModule.OpenUIForm`。

不要在运行时代码里直接操作未纳入 UIModule 的包生命周期。

## 相关源码

- `FGUIProject/`
- `Client/Assets/HoweFramework.Editor/FairyGUI/`
- `Client/Assets/HoweFramework/UI/FairyGUI/`
- `Client/Assets/GameMain/Scripts/UI/`
