using System.Collections.Generic;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 相机模块。
    /// </summary>
    public sealed class CameraModule : ModuleBase<CameraModule>
    {
        /// <summary>
        /// 主相机。
        /// </summary>
        public Camera MainCamera { get; private set; }

        /// <summary>
        /// 初始相机。
        /// </summary>
        private Camera m_InitCamera;

        /// <summary>
        /// 相机控制器列表。
        /// </summary>
        private readonly List<GameCamera> m_CameraControllers = new();

        /// <summary>
        /// 相机列表是否发生变化。
        /// </summary>
        private bool m_CameraListChanged = false;
        
        /// <summary>
        /// 注册相机。
        /// </summary>
        internal void RegisterCamera(GameCamera cameraController)
        {
            m_CameraControllers.BinaryInsert(cameraController);
            m_CameraListChanged = true;
        }

        /// <summary>
        /// 注销相机。
        /// </summary>
        internal void UnregisterCamera(GameCamera cameraController)
        {
            m_CameraControllers.Remove(cameraController);
            m_CameraListChanged = true;
        }

        protected override void OnInit()
        {
            // 获取初始相机。
            m_InitCamera = Camera.main;

            // 设置主相机。
            MainCamera = m_InitCamera;
        }
        protected override void OnDestroy()
        {
            m_CameraControllers.Clear();
            m_CameraListChanged = false;
            MainCamera = null;
            m_InitCamera = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (!m_CameraListChanged)
            {
                return;
            }

            m_CameraListChanged = false;

            if (m_CameraControllers.Count == 0)
            {
                MainCamera = m_InitCamera;
                MainCamera.enabled = true;
                return;
            }

            var cameraController = m_CameraControllers[0];
            MainCamera = cameraController.Camera;
            MainCamera.enabled = true;

            for (int i = 1; i < m_CameraControllers.Count; i++)
            {
                var camera = m_CameraControllers[i].Camera;
                camera.enabled = false;
            }
            
            m_InitCamera.enabled = false;
        }
    }
}