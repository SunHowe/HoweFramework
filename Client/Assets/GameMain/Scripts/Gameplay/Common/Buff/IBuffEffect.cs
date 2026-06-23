namespace GameMain
{
    /// <summary>
    /// Buff效果接口. 在 buff 生命周期的不同时机被回调，
    /// 可结合 <see cref="NumericComponent"/> / <see cref="StateComponent"/> 等组件修改实体状态.
    /// </summary>
    public interface IBuffEffect
    {
        /// <summary>
        /// Buff添加时触发. 层数为 1.
        /// </summary>
        /// <param name="buff">所属 buff 实例.</param>
        void OnAdd(IBuffInstance buff);

        /// <summary>
        /// Buff移除时触发.
        /// </summary>
        /// <param name="buff">所属 buff 实例.</param>
        void OnRemove(IBuffInstance buff);

        /// <summary>
        /// 层数变化时触发.
        /// </summary>
        /// <param name="buff">所属 buff 实例.</param>
        /// <param name="oldStack">变化前层数.</param>
        /// <param name="newStack">变化后层数.</param>
        void OnStackChange(IBuffInstance buff, int oldStack, int newStack);

        /// <summary>
        /// 周期触发. 由配置的 <see cref="IBuffConfig.TickInterval"/> 决定调用频率.
        /// </summary>
        /// <param name="buff">所属 buff 实例.</param>
        void OnTick(IBuffInstance buff);

        /// <summary>
        /// 释放效果. 清理自身引用的资源.
        /// </summary>
        void Dispose();
    }
}
