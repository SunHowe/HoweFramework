using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 游戏组件转换器。
    /// </summary>
    public interface IGameComponentConverter
    {
        /// <summary>
        /// 排序。
        /// </summary>
        int SortingOrder { get; }

        /// <summary>
        /// 转换组件。
        /// </summary>
        /// <param name="entity">实体。</param>
        void Convert(IGameEntity entity);
    }

    /// <summary>
    /// 游戏组件转换器基类。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    [RequireComponent(typeof(GameEntityConverter))]
    public abstract class GameComponentConverterBase<T> : MonoBehaviour, IGameComponentConverter where T : GameComponentBase, new()
    {
        /// <summary>
        /// 排序。默认使用组件类型作为排序。
        /// </summary>
        public virtual int SortingOrder => GameEntityHelper.GetComponentType<T>();

        /// <summary>
        /// 转换组件。
        /// </summary>
        /// <param name="entity">实体。</param>
        public virtual void Convert(IGameEntity entity)
        {
            entity.AddComponent<T>();
        }
    }
}