using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 数值组件转换器。
    /// </summary>
    public sealed class NumericComponentConverter : GameComponentConverterBase<NumericComponent>
    {
        [SerializeField]
        private KeyValuePair<int, long>[] numericDict;

        /// <summary>
        /// 转换组件。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void Convert(IGameEntity entity)
        {
            var component = entity.AddComponent<NumericComponent>();

            if (numericDict == null || numericDict.Length == 0)
            {
                return;
            }

            foreach (var numeric in numericDict)
            {
                component.Set(numeric.Key, numeric.Value);
            }
        }
    }
}