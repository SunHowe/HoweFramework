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
    }
}