using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 游戏实体转换器扩展。
    /// </summary>
    public static class GameEntityConverterExtensions
    {
        /// <summary>
        /// 转换实体。
        /// </summary>
        /// <param name="gameObject">游戏对象。</param>
        /// <param name="entity">游戏实体。</param>
        public static void ConvertEntity(this GameObject gameObject, IGameEntity entity)
        {
            if (!gameObject.TryGetComponent<GameEntityConverter>(out var entityConverter))
            {
                return;
            }

            entityConverter.Convert(entity);
        }

        /// <summary>
        /// 转换实体。
        /// </summary>
        /// <param name="gameObject">游戏对象。</param>
        /// <param name="context">游戏上下文。</param>
        /// <returns>游戏实体。</returns>
        public static IGameEntity ConvertEntity(this GameObject gameObject, IGameContext context)
        {
            var entity = context.GetManager<IGameEntityManager>().CreateEntity();
            gameObject.ConvertEntity(entity);
            return entity;
        }
    }
}