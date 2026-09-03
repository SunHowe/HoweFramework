using System;
using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 单条属性修正。
    /// 设置了 <see cref="MagnitudeParamKey"/> 时，施加时从 <see cref="EffectSpec.Params"/> 取值并快照。
    /// </summary>
    public struct GameplayEffectModifier
    {
        public int AttributeId;
        public ModifierOp Op;
        public GameplayModifierChannel Channel;

        /// <summary>
        /// Overlay 写入的 Numeric 子项。
        /// <see cref="NumericSubType.Final"/>（默认）表示按 <see cref="Op"/> 推断：
        /// Add → BasicConstAdd，Multiply → BasicPercent，Override → Override。
        /// 也可显式指定 BasicConstAdd / FinalConstAdd / BasicPercent / FinalPercent。
        /// </summary>
        public NumericSubType NumericSubType;

        /// <summary>
        /// 无参数键时使用的字面量；百分比与 Numeric 一致（20 表示 +20%）。
        /// </summary>
        public long Magnitude;

        /// <summary>
        /// 从 <see cref="EffectSpec.Params"/> 取值的键。空表示直接用 <see cref="Magnitude"/>。
        /// </summary>
        public string MagnitudeParamKey;

        /// <summary>
        /// 参数取值后的缩放。0 视为 1。
        /// </summary>
        public long MagnitudeScale;

        public bool HasParamMagnitude => !string.IsNullOrEmpty(MagnitudeParamKey);

        public long ResolveMagnitude(IReadOnlyDictionary<string, long> paramBag)
        {
            var scale = MagnitudeScale == 0 ? 1L : MagnitudeScale;
            if (!HasParamMagnitude)
            {
                return Magnitude * scale;
            }

            if (paramBag == null || !paramBag.TryGetValue(MagnitudeParamKey, out var value))
            {
                throw new InvalidOperationException(
                    string.Format("Effect modifier requires param '{0}' but it was not provided on EffectSpec.", MagnitudeParamKey));
            }

            return value * scale;
        }

        public GameplayEffectModifier Snapshot(IReadOnlyDictionary<string, long> paramBag)
        {
            return new GameplayEffectModifier
            {
                AttributeId = AttributeId,
                Op = Op,
                Channel = Channel,
                NumericSubType = NumericSubType,
                Magnitude = ResolveMagnitude(paramBag),
                MagnitudeParamKey = null,
                MagnitudeScale = 1,
            };
        }
    }
}
