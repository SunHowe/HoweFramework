using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// Buff管理器接口. 提供 Buff 配置与效果工厂的注册与查询能力.
    /// </summary>
    [GameManager(GameManagerType.Buff)]
    public interface IBuffManager : IGameManager
    {
        /// <summary>
        /// 注册 Buff 配置.
        /// </summary>
        /// <param name="config">Buff 配置.</param>
        void RegisterBuffConfig(IBuffConfig config);

        /// <summary>
        /// 注销 Buff 配置.
        /// </summary>
        /// <param name="buffId">Buff id.</param>
        void UnregisterBuffConfig(int buffId);

        /// <summary>
        /// 获取 Buff 配置.
        /// </summary>
        /// <param name="buffId">Buff id.</param>
        /// <returns>Buff 配置，未注册返回 null.</returns>
        IBuffConfig GetBuffConfig(int buffId);

        /// <summary>
        /// 注册效果工厂.
        /// </summary>
        /// <param name="factory">效果工厂.</param>
        void RegisterEffectFactory(IBuffEffectFactory factory);

        /// <summary>
        /// 注销效果工厂.
        /// </summary>
        /// <param name="effectType">效果类型.</param>
        void UnregisterEffectFactory(BuffEffectType effectType);

        /// <summary>
        /// 创建效果实例.
        /// </summary>
        /// <param name="config">效果配置.</param>
        /// <returns>效果实例.</returns>
        IBuffEffect CreateEffect(IBuffEffectConfig config);
    }
}
