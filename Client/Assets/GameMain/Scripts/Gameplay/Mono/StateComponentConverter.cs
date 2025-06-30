using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 状态组件转换器。
    /// </summary>
    public sealed class StateComponentConverter : GameComponentConverterBase<StateComponent>
    {
        /// <summary>
        /// 初始状态列表。
        /// </summary>
        [SerializeField]
        private int[] states;

        /// <summary>
        /// 转换组件。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void Convert(IGameEntity entity)
        {
            var component = entity.AddComponent<StateComponent>();

            if (states == null || states.Length == 0)
            {
                return;
            }

            foreach (var state in states)
            {
                component.AddState(state, entity);
            }
        }
    }
}