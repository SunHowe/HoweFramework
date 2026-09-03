namespace GameMain
{
    /// <summary>
    /// 已施加游戏效果的生命周期阶段。
    /// </summary>
    public enum GameplayEffectLifecycleKind : byte
    {
        Applied = 0,
        Removed = 1,
        Expired = 2,
    }
}
