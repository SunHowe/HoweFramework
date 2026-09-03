namespace GameMain
{
    /// <summary>
    /// 授予给 ASC 的技能（不一定正在激活）。
    /// </summary>
    public struct GrantedAbility
    {
        public int AbilityDefId;
        public int Level;

        public GrantedAbility(int abilityDefId, int level = 1)
        {
            AbilityDefId = abilityDefId;
            Level = level < 1 ? 1 : level;
        }
    }
}
