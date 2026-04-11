# 相机管理

## 概述

CameraModule 管理游戏中的相机，支持多相机优先级管理。

## 核心文件

| 文件 | 路径 |
|------|------|
| CameraModule | `Assets/HoweFramework/Camera/CameraModule.cs` |
| GameCamera | `Assets/HoweFramework/Camera/GameCamera.cs` |

## CameraModule API

```csharp
public sealed class CameraModule : ModuleBase<CameraModule>
{
    // 主相机（优先级最高的相机）
    public Camera MainCamera { get; }
}
```

## GameCamera 组件

```csharp
[RequireComponent(typeof(Camera))]
public sealed class GameCamera : MonoBehaviour, IComparable<GameCamera>
{
    public Camera Camera { get; }
    public int Priority { get; }

    // 自动注册到 CameraModule
    // 禁用时自动注销
}
```

## 基本用法

### 1. 创建 GameCamera

```csharp
// 在场景中创建空物体，添加 GameCamera 组件
var go = new GameObject("MainCamera");
var gameCamera = go.AddComponent<GameCamera>();
gameCamera.Priority = 100;
```

### 2. 设置相机参数

```csharp
var gameCamera = GetComponent<GameCamera>();
gameCamera.Camera.backgroundColor = Color.black;
gameCamera.Camera.fieldOfView = 60f;
gameCamera.Camera.depth = 1f;
```

### 3. 访问主相机

```csharp
// 获取当前主相机
Camera main = CameraModule.Instance.MainCamera;
```

## 最佳实践

### 1. 使用优先级控制相机切换

```csharp
// 相机 A（高优先级）
cameraA.Priority = 100;

// 相机 B（低优先级）
cameraB.Priority = 50;

// CameraModule.MainCamera 指向相机 A
```

### 2. 禁用/启用相机

```csharp
// 禁用相机（自动从 CameraModule 注销）
gameCamera.enabled = false;

// 启用相机（自动注册到 CameraModule）
gameCamera.enabled = true;
```

### 3. 处理相机切换

```csharp
public class CameraManager
{
    private GameCamera _battleCamera;
    private GameCamera _uiCamera;

    public void SwitchToBattle()
    {
        _battleCamera.Priority = 100;
        _uiCamera.Priority = 50;
    }

    public void SwitchToUI()
    {
        _battleCamera.Priority = 50;
        _uiCamera.Priority = 100;
    }
}
```