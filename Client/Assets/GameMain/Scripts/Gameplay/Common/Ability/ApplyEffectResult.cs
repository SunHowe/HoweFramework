namespace GameMain
{
    /// <summary>
    /// 施加效果失败原因。
    /// </summary>
    public enum ApplyEffectFailReason : byte
    {
        None = 0,
        UnknownEffect = 1,
        MissingApplicationRequiredTags = 2,
        BlockedByImmunityTags = 3,
        MissingAsc = 4,
    }

    /// <summary>
    /// 施加游戏效果的结果。
    /// </summary>
    public readonly struct ApplyEffectResult
    {
        public bool Success { get; }
        public int ActiveHandle { get; }
        public bool WasInstant { get; }
        public bool RefreshedExisting { get; }
        public ApplyEffectFailReason FailReason { get; }

        public ApplyEffectResult(bool success, int activeHandle, bool wasInstant, bool refreshedExisting, ApplyEffectFailReason failReason)
        {
            Success = success;
            ActiveHandle = activeHandle;
            WasInstant = wasInstant;
            RefreshedExisting = refreshedExisting;
            FailReason = failReason;
        }

        public static ApplyEffectResult Failed(ApplyEffectFailReason reason = ApplyEffectFailReason.UnknownEffect)
        {
            return new ApplyEffectResult(false, 0, false, false, reason);
        }

        public static ApplyEffectResult Instant()
        {
            return new ApplyEffectResult(true, 0, true, false, ApplyEffectFailReason.None);
        }

        public static ApplyEffectResult Applied(int handle)
        {
            return new ApplyEffectResult(true, handle, false, false, ApplyEffectFailReason.None);
        }

        public static ApplyEffectResult Refreshed(int handle)
        {
            return new ApplyEffectResult(true, handle, false, true, ApplyEffectFailReason.None);
        }
    }
}
