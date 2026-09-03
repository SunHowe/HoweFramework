using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 施加游戏效果的运行时请求。
    /// </summary>
    public struct EffectSpec
    {
        public int EffectDefId;
        public IGameEntity Source;
        public int Level;
        public IReadOnlyDictionary<string, long> Params;

        public EffectSpec(int effectDefId, IGameEntity source = null, int level = 1, IReadOnlyDictionary<string, long> paramBag = null)
        {
            EffectDefId = effectDefId;
            Source = source;
            Level = level < 1 ? 1 : level;
            Params = paramBag;
        }
    }
}
