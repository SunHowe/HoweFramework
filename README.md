# HoweFramework

## 介绍

Unity3d客户端开发框架，部分模块采用现有的工具或参考现有的框架实现。

## 相关框架、工具说明

- GameFramework: [https://github.com/EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework)
- UnityGameFramework: [https://github.com/EllanJiang/UnityGameFramework](https://github.com/EllanJiang/UnityGameFramework)
- FairyGUI: [https://www.fairygui.com](https://www.fairygui.com)
- FairyGUI-Dynamic: [https://github.com/SunHowe/FairyGUI-Dynamic](https://github.com/SunHowe/FairyGUI-Dynamic)
- FairyGUI-CodeGenerator: [https://github.com/SunHowe/FairyGUI-CodeGenerator](https://github.com/SunHowe/FairyGUI-CodeGenerator)
- YooAsset: [https://github.com/tuyoogame/YooAsset](https://github.com/tuyoogame/YooAsset)
- Luban: [https://github.com/focus-creative-games/luban](https://github.com/focus-creative-games/luban)
- UniTask: [https://github.com/Cysharp/UniTask.git](https://github.com/Cysharp/UniTask.git)

## 现有模块

- BaseModule

    基础模块，主要为一些基础工具提供注册服务。
    
    通过`IJsonHelper`、`ITextTemplateHelper`为使用者自定义拓展需求提供支持。

- DataTable

    配置表模块，基于Luban实现，提供了多种加载模式:
    - 启动时异步加载配置表
    - 启动时同步加载配置表
    - 懒加载模式。使用时同步加载配置表
    - 在懒加载模式的基础上，使用异步的方式异步预加载特定的配置表
    - 在懒加载模式的基础上，使用同步的方式同步预加载特定的配置表
    
    通过`IDataTableSource`为使用者自定义拓展需求提供支持。

- Event

    事件管线模块，参考GameFramework实现，为项目全局与局部提供事件调度器的相关功能。
    
    局部事件调度器不使用后需要调用`Dispose()`进行销毁。

- GameObjectPool

    游戏对象池模块，为项目全局与局部提供游戏对象池的相关功能，并提供预实例化指定资源对象、限制池内对象数量的功能。
    
    局部游戏对象池不使用后需要调用`Dispose()`进行销毁，以保证及时回收资源。

- Gameplay

    玩法模块，为项目提供基础的玩法抽象，基于EC(实体-组件)模式设计，可将其当做不依赖于Unity的GameObject-Component模式。

- Localization

    本地化模块，为项目提供本地化文本管理的功能。
    
    通过`ILocalizationSource`为使用者自定义拓展需求提供支持。

- Network

    网络模块，参考GameFramework实现，为项目提供通用的网络通信功能。

    通过`INetworkChannel`、`INetworkChannelHelper`、`IPacketHandler`、`IPacketHeader`为使用者自定义拓展需求提供支持。

- Procedure

    流程模块，参考GameFramework实现，为项目提供全局的流程管理功能。

- Reference

    引用池模块，参考GameFramework实现，用于管理项目的引用对象的创建与回收。

- Request

    异步请求模块，基于UniTask实现，为项目的异步交互进行统一的抽象，并提供一些拓展方法方便功能开发。
    
    诸如`打开UI界面并等待UI交互`、`发送HTTP请求并等待返回`、`发送协议给服务端并等待返回`这样的异步交互流程都可以基于异步请求实现，统一的抽象中会确保必定会返回一个带错误码的响应包实例，方便业务开发(例如后续可以基于这个相应包做错误码提示功能)。

- Res

    资源模块，基于YooAsset实现，为项目提供局部的资源加载器功能(不直接提供全局资源加载器的原因是为了确保资源的及时回收，防止内存泄漏)。
    
    当资源加载器不使用后需要调用`Dispose()`进行销毁，以保证及时回收资源。

    通过`IResLoader`为使用者自定义拓展需求提供支持。

- SafeArea

    安全区域模块，为项目提供获取和监听屏幕安全区域范围的功能，用于做屏幕适配使用。在Editor提供了可支持调试的工具。

    通过`ISafeAreaHelper`为使用者自定义拓展需求提供支持。

    运行时一般使用默认的实现接口即可(基于Unity的接口实现)，若存在旧版本Unity接口获取新款设备失败的情况，也可以自己实现安全区域辅助器，通过native的代码进行安全区域的获取。

- Scene

    场景模块，基于项目的资源模块实现，为项目提供加载场景、卸载场景的功能，项目内加载场景统一采用Additive模式，加载完成后会统一设置ActiveScene，若有多个场景，会根据设置的场景优先级进行设置。

- Setting

    设置模块，为项目提供基于键值对形式的用户设置功能，支持基础类型与复杂对象类型(基于Json序列化器实现)。
    
    通过`ISettingHelper`为使用者自定义拓展需求提供支持。

- Sound

    声音模块，基于项目的资源模块实现，为项目提供音频管理的功能。
    
    通过`ISoundHelper`和`ISoundGroupHelper`为使用者自定义拓展需求提供支持。

- Timer

    定时器模块，为项目全局与局部提供帧定时器与时间定时器的功能支持。
    
    当资源加载器不使用后需要调用`Dispose()`进行销毁。

- UI

    UI模块，为项目提供通用的UI管理服务，并基于项目的请求模块实现UI交互的异步请求封装。
    
    通过`IUIFormGroupHelper`、`IUIFormHelper`、`IUIFormLogic`为使用者自定义拓展需求提供支持。

    项目内已基于FairyGUI实现了一套UI界面的逻辑，并通过编辑器工具实现了界面代码与组件代码的自动生成功能。

- WebRequest

    Web请求模块，为项目提供通用的HTTP交互功能，并基于项目的请求模块实现异步请求封装。

    通过`IWebRequestHelper`为使用者自定义拓展需求提供支持。

    项目内已基于UnityWebRequest实现了一套HTTP交互的逻辑。