namespace GameMain
{
    /// <summary>
    /// Buff组件扩展方法.
    /// </summary>
    public static class BuffComponentExtensions
    {
        /// <summary>
        /// 添加 Buff.
        /// </summary>
        /// <param name="entity">游戏实体.</param>
        /// <param name="buffId">Buff id.</param>
        public static void AddBuff(this IGameEntity entity, int buffId)
        {
            var buffComponent = entity.GetComponent<BuffComponent>();
            if (buffComponent == null)
            {
                buffComponent = entity.AddComponent<BuffComponent>();
            }

            buffComponent.AddBuff(buffId);
        }

        /// <summary>
        /// 移除 Buff.
        /// </summary>
        /// <param name="entity">游戏实体.</param>
        /// <param name="buffId">Buff id.</param>
        public static void RemoveBuff(this IGameEntity entity, int buffId)
        {
            var buffComponent = entity.GetComponent<BuffComponent>();
            buffComponent?.RemoveBuff(buffId);
        }

        /// <summary>
        /// 检查 Buff 是否存在.
        /// </summary>
        /// <param name="entity">游戏实体.</param>
        /// <param name="buffId">Buff id.</param>
        /// <returns>是否存在.</returns>
        public static bool HasBuff(this IGameEntity entity, int buffId)
        {
            var buffComponent = entity.GetComponent<BuffComponent>();
            return buffComponent != null && buffComponent.HasBuff(buffId);
        }

        /// <summary>
        /// 获取 Buff 层数.
        /// </summary>
        /// <param name="entity">游戏实体.</param>
        /// <param name="buffId">Buff id.</param>
        /// <returns>层数，不存在返回 0.</returns>
        public static int GetBuffStack(this IGameEntity entity, int buffId)
        {
            var buffComponent = entity.GetComponent<BuffComponent>();
            return buffComponent?.GetBuffStack(buffId) ?? 0;
        }

        /// <summary>
        /// 获取 Buff 剩余时间.
        /// </summary>
        /// <param name="entity">游戏实体.</param>
        /// <param name="buffId">Buff id.</param>
        /// <returns>剩余时间（秒），永久返回 -1，不存在返回 0.</returns>
        public static float GetBuffRemainingTime(this IGameEntity entity, int buffId)
        {
            var buffComponent = entity.GetComponent<BuffComponent>();
            return buffComponent?.GetBuffRemainingTime(buffId) ?? 0;
        }
    }
}
