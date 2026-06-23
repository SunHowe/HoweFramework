using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// Buff配置接口. 游戏层可用任意方式（配置表/代码/SO）实现，
    /// 注册到 <see cref="IBuffManager"/> 后供 BuffComponent 查询.
    /// </summary>
    public interface IBuffConfig
    {
        /// <summary>
        /// Buff id.
        /// </summary>
        int BuffId { get; }

        /// <summary>
        /// 叠加策略.
        /// </summary>
        BuffStackPolicy StackPolicy { get; }

        /// <summary>
        /// 最大层数. 仅当叠加策略为 Stack 时有效，&lt;= 0 表示不限制.
        /// </summary>
        int MaxStack { get; }

        /// <summary>
        /// 持续时间（秒）.&lt;= 0 表示永久.
        /// </summary>
        float Duration { get; }

        /// <summary>
        /// 周期触发间隔（秒）.&lt;= 0 表示不周期触发.
        /// </summary>
        float TickInterval { get; }

        /// <summary>
        /// 效果配置列表.
        /// </summary>
        IReadOnlyList<IBuffEffectConfig> EffectConfigs { get; }
    }
}
