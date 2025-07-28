namespace GameMain
{
    /// <summary>
    /// 表达式管理器。
    /// </summary>
    [GameManager(GameManagerType.Expression)]
    public interface IExpressionManager : IGameManager
    {
        /// <summary>
        /// 注册表达式解析器。
        /// </summary>
        /// <param name="parser">表达式解析器。</param>
        void RegisterTokenParser(TokenParser parser);

        /// <summary>
        /// 根据表达式计算结果。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="userData">用户数据。</param>
        /// <returns>计算结果。</returns>
        double Evaluate(string expression, object userData);
    }
}