namespace GameMain
{
    /// <summary>
    /// 技能相对施法者的目标关系。敌/友由调用方按自己的阵营模型解释。
    /// </summary>
    public enum AbilityTargetRelation : byte
    {
        None = 0,
        Self = 1,
        Ally = 2,
        Enemy = 3,
        Any = 4,
    }
}
