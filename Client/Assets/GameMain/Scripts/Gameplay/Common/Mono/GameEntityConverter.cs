using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 实体转换器。
    /// </summary>
    public sealed class GameEntityConverter : MonoBehaviour
    {
        /// <summary>
        /// 转换实体。
        /// </summary>
        /// <param name="context">游戏上下文。</param>
        /// <returns>转换后的实体。</returns>
        public IGameEntity Convert(IGameContext context)
        {
            var entityManager = context.GetManager<IGameEntityManager>();
            var entity = entityManager.CreateEntity();

            using var componentConverterBuffer = ReusableList<IGameComponentConverter>.Create();
            GetComponents(componentConverterBuffer);
            componentConverterBuffer.Sort(SortBySortingOrder);

            foreach (var componentConverter in componentConverterBuffer)
            {
                componentConverter.Convert(entity);
            }

            return entity;
        }

        /// <summary>
        /// 排序。
        /// </summary>
        private static int SortBySortingOrder(IGameComponentConverter x, IGameComponentConverter y)
        {
            return x.SortingOrder.CompareTo(y.SortingOrder);
        }
    }
}