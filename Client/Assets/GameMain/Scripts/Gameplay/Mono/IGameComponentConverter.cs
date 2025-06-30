namespace GameMain
{
    /// <summary>
    /// 游戏组件转换器。
    /// </summary>
    public interface IGameComponentConverter
    {
        /// <summary>
        /// 排序。
        /// </summary>
        int SortingOrder { get; }

        /// <summary>
        /// 转换组件。
        /// </summary>
        /// <param name="entity">实体。</param>
        void Convert(IGameEntity entity);
    }
}