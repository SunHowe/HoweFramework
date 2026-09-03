namespace GameMain
{
    /// <summary>
    /// 技能逻辑。激活提交成功后同步调用。
    /// </summary>
    public interface IGameplayAbility
    {
        /// <summary>
        /// 提交成功后调用。即时技能可在此结束。
        /// </summary>
        void Activate(in AbilityActivationContext context);

        /// <summary>
        /// 技能正常结束或被取消时调用。
        /// </summary>
        void OnEnd(in AbilityActivationContext context, bool wasCancelled);
    }
}
