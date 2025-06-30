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
        /// <param name="context">游戏上下文。</param>
        public static IGameEntity ConvertEntity(this GameObject gameObject, IGameContext context)
        {
            if (!gameObject.TryGetComponent<GameEntityConverter>(out var entityConverter))
            {
                return null;
            }

            return entityConverter.Convert(context);
        }
    }
}