using System;
using UnityEngine;

namespace GameMain
{
    public delegate void ViewObjectLoadedDelegate(IViewObject viewObject);
    public delegate void ViewObjectUnloadedDelegate(IViewObject viewObject);

    /// <summary>
    /// 视图对象接口。
    /// </summary>
    public interface IViewObject : IDisposable
    {
        /// <summary>
        /// 加载完成事件.
        /// </summary>
        event ViewObjectLoadedDelegate OnLoaded;

        /// <summary>
        /// 卸载完成事件.
        /// </summary>
        event ViewObjectUnloadedDelegate OnUnloaded;

        /// <summary>
        /// 资源键值.
        /// </summary>
        string ResKey { get; }

        /// <summary>
        /// GameObject实例.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// Transform实例.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// 获取/设置父节点。
        /// </summary>
        Transform ParentTransform { get; set; }

        /// <summary>
        /// 获取/设置位置(基于本地坐标系)
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// 获取/设置欧拉角(基于本地坐标系)
        /// </summary>
        Vector3 EulerAngles { get; set; }

        /// <summary>
        /// 获取/设置缩放(基于本地坐标系)
        /// </summary>
        Vector3 Scale { get; set; }

        /// <summary>
        /// 获取/设置是否可见
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// 是否已完成加载。
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// 加载GameObject.
        /// </summary>
        void LoadGameObject(string resKey);

        /// <summary>
        /// 卸载GameObject.
        /// </summary>
        void UnloadGameObject();
    }
}
