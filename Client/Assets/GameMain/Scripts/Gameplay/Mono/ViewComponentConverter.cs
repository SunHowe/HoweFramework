using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 视图组件转换器。
    /// </summary>
    public sealed class ViewComponentConverter : GameComponentConverterBase<ViewComponent>
    {
        /// <summary>
        /// 是否同步变换组件。
        /// </summary>
        [SerializeField]
        private bool syncTransform = true;

        /// <summary>
        /// 转换组件。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void Convert(IGameEntity entity)
        {
            var component = entity.AddComponent<ViewComponent>();
            component.SetGameObject(gameObject);

            if (syncTransform)
            {
                entity.AddComponent<ViewTransformSyncComponent>();
            }
        }
    }
}