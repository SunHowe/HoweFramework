using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏效果表现 Cue。逻辑层不持有 VFX/SFX。
    /// </summary>
    public sealed class GameplayCueEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GameplayCueEventArgs).GetHashCode();

        public override int Id => EventId;

        public IGameEntity Target { get; private set; }

        public GameplayTag CueTag { get; private set; }

        public bool IsAdded { get; private set; }

        public int EffectDefId { get; private set; }

        protected override void OnClear()
        {
            Target = null;
            CueTag = default;
            IsAdded = false;
            EffectDefId = 0;
        }

        public static GameplayCueEventArgs Create(IGameEntity target, GameplayTag cueTag, bool isAdded, int effectDefId)
        {
            var args = ReferencePool.Acquire<GameplayCueEventArgs>();
            args.Target = target;
            args.CueTag = cueTag;
            args.IsAdded = isAdded;
            args.EffectDefId = effectDefId;
            return args;
        }
    }
}
