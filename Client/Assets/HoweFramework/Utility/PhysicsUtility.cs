using System;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 物理辅助类。
    /// </summary>
    public static class PhysicsUtility
    {
        private static RaycastHit[] s_RaycastHits = new RaycastHit[16];

        /// <summary>
        /// 设置射线检测缓冲区容量。默认为16。
        /// </summary>
        /// <param name="capacity">容量。</param>
        public static void SetRaycastHitBufferCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Capacity is invalid.");
            }

            s_RaycastHits = new RaycastHit[capacity];
        }

        /// <summary>
        /// 射线检测。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <returns>射线检测结果。</returns>
        public static ReadOnlySpan<RaycastHit> Raycast(in Ray ray)
        {
            var count = Physics.RaycastNonAlloc(ray, s_RaycastHits);
            return s_RaycastHits.AsSpan(0, count);
        }

        /// <summary>
        /// 射线检测。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="maxDistance">最大距离。</param>
        /// <returns>射线检测结果。</returns>
        public static ReadOnlySpan<RaycastHit> Raycast(in Ray ray, float maxDistance)
        {
            var count = Physics.RaycastNonAlloc(ray, s_RaycastHits, maxDistance);
            return s_RaycastHits.AsSpan(0, count);
        }

        /// <summary>
        /// 射线检测。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="layerMask">层掩码。</param>
        /// <returns>射线检测结果。</returns>
        public static ReadOnlySpan<RaycastHit> Raycast(in Ray ray, int layerMask)
        {
            var count = Physics.RaycastNonAlloc(ray, s_RaycastHits, layerMask);
            return s_RaycastHits.AsSpan(0, count);
        }

        /// <summary>
        /// 射线检测。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="layerMask">层掩码。</param>
        /// <param name="maxDistance">最大距离。</param>
        /// <returns>射线检测结果。</returns>
        public static ReadOnlySpan<RaycastHit> Raycast(in Ray ray, int layerMask, float maxDistance)
        {
            var count = Physics.RaycastNonAlloc(ray, s_RaycastHits, maxDistance, layerMask);
            return s_RaycastHits.AsSpan(0, count);
        }

        /// <summary>
        /// 射线检测。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="layerMask">层掩码。</param>
        /// <param name="maxDistance">最大距离。</param>
        /// <param name="queryTriggerInteraction">触发器交互。</param>
        /// <returns>射线检测结果。</returns>
        public static ReadOnlySpan<RaycastHit> Raycast(in Ray ray, int layerMask, float maxDistance, QueryTriggerInteraction queryTriggerInteraction)
        {
            var count = Physics.RaycastNonAlloc(ray, s_RaycastHits, maxDistance, layerMask, queryTriggerInteraction);
            return s_RaycastHits.AsSpan(0, count);
        }
    }
}