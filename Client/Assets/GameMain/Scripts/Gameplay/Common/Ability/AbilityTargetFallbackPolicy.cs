namespace GameMain
{
    /// <summary>
    /// 请求的目标未通过关系 / 标签过滤时的处理。空间选敌由调用方完成。
    /// </summary>
    public enum AbilityTargetFallbackPolicy : byte
    {
        None = 0,
        RandomMatching = 1,
    }
}
