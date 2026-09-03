namespace GameMain
{
    /// <summary>
    /// 激活技能失败原因。
    /// </summary>
    public enum AbilityActivationFailReason : byte
    {
        None = 0,
        NotGranted = 1,
        AlreadyActive = 2,
        MissingRequiredTags = 3,
        BlockedByTags = 4,
        OnCooldown = 5,
        CannotAffordCost = 6,
        UnknownAbility = 7,
        MissingAsc = 8,
        TargetRejected = 9,
    }

    /// <summary>
    /// 激活技能的结果。
    /// </summary>
    public readonly struct AbilityActivationResult
    {
        public bool Success { get; }
        public int ActiveHandle { get; }
        public AbilityActivationFailReason FailReason { get; }

        public AbilityActivationResult(bool success, int activeHandle, AbilityActivationFailReason failReason)
        {
            Success = success;
            ActiveHandle = activeHandle;
            FailReason = failReason;
        }

        public static AbilityActivationResult Ok(int handle)
        {
            return new AbilityActivationResult(true, handle, AbilityActivationFailReason.None);
        }

        public static AbilityActivationResult Fail(AbilityActivationFailReason reason)
        {
            return new AbilityActivationResult(false, 0, reason);
        }
    }
}
