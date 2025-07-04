using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// Transform数据，用于打包位置、旋转、缩放等数据。
    /// </summary>
    public struct TransformData
    {
        public Vector3 Position;
        public Vector3 EulerAngles;
        public Vector3 Scale;

        public static TransformData Zero => new(Vector3.zero, Vector3.zero, Vector3.one);

        public TransformData(Vector3 position, Vector3 eulerAngles, Vector3 scale)
        {
            Position = position;
            EulerAngles = eulerAngles;
            Scale = scale;
        }

        public TransformData(TransformComponent component)
        {
            Position = component.Position;
            EulerAngles = component.EulerAngles;
            Scale = component.Scale;
        }

        public TransformData(Transform transform, bool isLocal = false)
        {
            if (isLocal)
            {
                Position = transform.localPosition;
                EulerAngles = transform.localEulerAngles;
                Scale = transform.localScale;
            }
            else
            {
                Position = transform.position;
                EulerAngles = transform.eulerAngles;
                Scale = transform.lossyScale;
            }
        }

        /// <summary>
        /// 应用到Transform。
        /// </summary>
        /// <param name="transform">Transform。</param>
        /// <param name="isLocal">是否是本地坐标。</param>
        public void Apply(Transform transform, bool isLocal = false)
        {
            if (isLocal)
            {
                transform.localPosition = Position;
                transform.localEulerAngles = EulerAngles;
                transform.localScale = Scale;
            }
            else
            {
                transform.position = Position;
                transform.eulerAngles = EulerAngles;
                transform.localScale = Scale;
            }
        }

        /// <summary>
        /// 应用到Transform组件。
        /// </summary>
        /// <param name="component">Transform组件。</param>
        public void Apply(TransformComponent component)
        {
            component.Position = Position;
            component.EulerAngles = EulerAngles;
            component.Scale = Scale;
        }
    }

    /// <summary>
    /// TransformData扩展。
    /// </summary>
    public static class TransformDataExtensions
    {
        /// <summary>
        /// 获取Transform数据。
        /// </summary>
        /// <param name="transform">Transform。</param>
        /// <param name="isLocal">是否是本地坐标。</param>
        /// <returns>Transform数据。</returns>
        public static TransformData GetTransformData(this Transform transform, bool isLocal = false)
        {
            return new TransformData(transform, isLocal);
        }

        /// <summary>
        /// 获取Transform数据。
        /// </summary>
        /// <param name="component">Transform组件。</param>
        /// <returns>Transform数据。</returns>
        public static TransformData GetTransformData(this TransformComponent component)
        {
            return new TransformData(component);
        }

        /// <summary>
        /// 应用到Transform。
        /// </summary>
        /// <param name="transform">Transform。</param>
        /// <param name="isLocal">是否是本地坐标。</param>
        public static void Apply(this Transform transform, TransformData data, bool isLocal = false)
        {
            data.Apply(transform, isLocal);
        }

        /// <summary>
        /// 应用到Transform组件。
        /// </summary>
        /// <param name="component">Transform组件。</param>
        public static void Apply(this TransformComponent component, TransformData data)
        {
            data.Apply(component);
        }
    }
}