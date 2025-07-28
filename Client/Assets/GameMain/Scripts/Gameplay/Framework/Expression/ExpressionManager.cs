namespace GameMain
{
    /// <summary>
    /// 表达式管理器。
    /// </summary>
    public sealed class ExpressionManager : GameManagerBase, IExpressionManager
    {
        private readonly ExpressionEvaluator m_ExpressionEvaluator = new();

        /// <summary>
        /// 注册表达式解析器。
        /// </summary>
        /// <param name="parser">表达式解析器。</param>
        public void RegisterTokenParser(TokenParser parser)
        {
            m_ExpressionEvaluator.RegisterTokenParser(parser);
        }

        /// <summary>
        /// 根据表达式计算结果。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="userData">用户数据。</param>
        /// <returns>计算结果。</returns>
        public double Evaluate(string expression, object userData)
        {
            return m_ExpressionEvaluator.Evaluate(expression, userData);
        }

        protected override void OnAwake()
        {
        }

        protected override void OnDispose()
        {
            m_ExpressionEvaluator.UnRegisterAllTokenParser();
        }
    }
}