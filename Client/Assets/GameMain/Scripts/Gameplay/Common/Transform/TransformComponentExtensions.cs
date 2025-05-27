using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// Transform组件扩展。
    /// </summary>
    public static class TransformComponentExtensions
    {
        /// <summary>
        /// 获取实体位置。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <returns>位置。</returns>
        public static Vector3 GetPosition(this IGameEntity entity)
        {
            if (entity == null)
            {
                return Vector3.zero;
            }

            var transformComponent = entity.GetComponent<TransformComponent>();
            if (transformComponent == null)
            {
                return Vector3.zero;
            }

            return transformComponent.Position;
        }

        /// <summary>
        /// 获取目标相对于自己的方向。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <param name="targetEntity">目标实体。</param>
        /// <returns>目标相对于自己的方向。</returns>
        public static Vector3 GetTargetDirection(this IGameEntity entity, IGameEntity targetEntity)
        {
            if (entity == null || targetEntity == null)
            {
                return Vector3.zero;
            }

            var entityPosition = entity.GetPosition();
            var targetPosition = targetEntity.GetPosition();

            return (targetPosition - entityPosition).normalized;
        }
    }
}
