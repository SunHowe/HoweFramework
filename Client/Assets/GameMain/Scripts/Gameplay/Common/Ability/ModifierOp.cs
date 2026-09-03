namespace GameMain
{
    /// <summary>
    /// 修正运算。聚合顺序：Add → Multiply → Override。
    /// </summary>
    public enum ModifierOp : byte
    {
        Add = 0,
        Multiply = 1,
        Override = 2,
    }
}
