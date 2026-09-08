using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏效果逻辑层生命周期通知（战报 / 被动 / 计数），表现仍走 <see cref="GameplayCueEventArgs"/>。
    /// </summary>
    public sealed class GameplayEffectLifecycleEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GameplayEffectLifecycleEventArgs).GetHashCode();

        public override int Id => EventId;

        public IGameEntity Target { get; private set; }

        public int EffectDefId { get; private set; }

        public int ActiveHandle { get; private set; }

        public IGameEntity Source { get; private set; }

        public GameplayEffectLifecycleKind Kind { get; private set; }

        public bool WasInstant { get; private set; }

        protected override void OnClear()
        {
            Target = null;
            EffectDefId = 0;
            ActiveHandle = 0;
            Source = null;
            Kind = GameplayEffectLifecycleKind.Applied;
            WasInstant = false;
        }

        public static GameplayEffectLifecycleEventArgs Create(
            IGameEntity target,
            int effectDefId,
            int activeHandle,
            IGameEntity source,
            GameplayEffectLifecycleKind kind,
            bool wasInstant)
        {
            var args = ReferencePool.Acquire<GameplayEffectLifecycleEventArgs>();
            args.Target = target;
            args.EffectDefId = effectDefId;
            args.ActiveHandle = activeHandle;
            args.Source = source;
            args.Kind = kind;
            args.WasInstant = wasInstant;
            return args;
        }
    }
}
