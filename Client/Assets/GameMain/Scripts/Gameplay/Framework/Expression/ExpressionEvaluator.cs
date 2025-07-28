using System;
using System.Collections.Generic;
using System.Text;

namespace GameMain
{
    public delegate bool TokenParser(string token, object userData, out double value);

    public sealed class ExpressionEvaluator
    {
        /// <summary>
        /// Token类型
        /// </summary>
        private enum TokenType
        {
            Number,
            Operator,
            LeftParen,
            RightParen,
            Variable
        }

        /// <summary>
        /// Token类
        /// </summary>
        private class Token
        {
            public TokenType Type { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// 表达式树节点
        /// </summary>
        private class TreeNode
        {
            public Token Token { get; set; }
            public TreeNode Left { get; set; }
            public TreeNode Right { get; set; }
        }


        /// <summary>
        /// 运算符优先级
        /// </summary>
        private static readonly Dictionary<char, int> OperatorPrecedence = new Dictionary<char, int>
        {
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 }
        };

        /// <summary>
        /// 括号映射
        /// </summary>
        private static readonly Dictionary<char, TokenType> ParenMap = new Dictionary<char, TokenType>
        {
            { '(', TokenType.LeftParen },
            { ')', TokenType.RightParen }
        };

        private readonly List<TokenParser> m_TokenParsers = new List<TokenParser>();
        private readonly Queue<TreeNode> m_NodePool = new Queue<TreeNode>();
        private readonly Stack<TreeNode> m_TreeNodeStack = new Stack<TreeNode>();
        private readonly Stack<double> m_EvaluateStack = new Stack<double>();
        private readonly Stack<TreeNode> m_EvaluateNodeStack = new Stack<TreeNode>();
        private readonly HashSet<TreeNode> m_VisitedNode = new HashSet<TreeNode>();

        private readonly Queue<Token> m_TokenPool = new Queue<Token>();
        private readonly List<Token> m_TokenInitBuffer = new List<Token>();
        private readonly List<Token> m_TokenPostfixBuffer = new List<Token>();
        private readonly Stack<Token> m_TokenStack = new Stack<Token>();

        private readonly StringBuilder m_StringBuilder = new StringBuilder();

        /// <summary>
        /// 注册表达式解析器
        /// </summary>
        public void RegisterTokenParser(TokenParser parser)
        {
            m_TokenParsers.Add(parser);
        }

        /// <summary>
        /// 取消注册表达式解析器。
        /// </summary>
        public void UnRegisterTokenParser(TokenParser parser)
        {
            m_TokenParsers.Remove(parser);
        }

        /// <summary>
        /// 取消注册所有表达式解析器。
        /// </summary>
        public void UnRegisterAllTokenParser()
        {
            m_TokenParsers.Clear();
        }

        /// <summary>
        /// 根据表达式计算结果
        /// </summary>
        public double Evaluate(string expression, object userData)
        {
            Tokenize(expression);
            InfixToPostfix();
            BuildExpressionTree();
            return EvaluateTree(userData);
        }

        /// <summary>
        /// 解析表达式为Token列表
        /// </summary>
        private void Tokenize(string expression)
        {
            m_TokenInitBuffer.Clear();
            for (int i = 0; i < expression.Length; i++)
            {
                char current = expression[i];
                if (char.IsWhiteSpace(current))
                {
                    continue;
                }

                if (char.IsDigit(current))
                {
                    // 纯数字
                    m_StringBuilder.Clear();

                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        m_StringBuilder.Append(expression[i]);
                        ++i;
                    }

                    m_TokenInitBuffer.Add(CreateToken(TokenType.Number, m_StringBuilder.ToString()));
                    --i;
                }
                else if (ParenMap.TryGetValue(current, out var tokenType))
                {
                    m_TokenInitBuffer.Add(CreateToken(tokenType, current.ToString()));
                }
                else if (OperatorPrecedence.ContainsKey(current))
                {
                    // 运算符
                    m_TokenInitBuffer.Add(CreateToken(TokenType.Operator, current.ToString()));
                }
                else
                {
                    // 自定义表达式
                    m_StringBuilder.Clear();

                    do
                    {
                        current = expression[i];
                        m_StringBuilder.Append(current);
                        ++i;
                    } while (i < expression.Length && IsVariableMiddleOrEndChar(expression[i]));

                    m_TokenInitBuffer.Add(CreateToken(TokenType.Variable, m_StringBuilder.ToString()));
                    --i;
                }
            }
        }

        /// <summary>
        /// 中缀表达式转后缀表达式
        /// </summary>
        private void InfixToPostfix()
        {
            m_TokenPostfixBuffer.Clear();
            m_TokenStack.Clear();

            foreach (var token in m_TokenInitBuffer)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                    case TokenType.Variable:
                        m_TokenPostfixBuffer.Add(token);
                        break;
                    case TokenType.LeftParen:
                        m_TokenStack.Push(token);
                        break;
                    case TokenType.RightParen:
                    {
                        while (m_TokenStack.Count > 0 && m_TokenStack.Peek().Type != TokenType.LeftParen)
                        {
                            m_TokenPostfixBuffer.Add(m_TokenStack.Pop());
                        }

                        m_TokenStack.Pop(); // 移除 '('
                        break;
                    }
                    case TokenType.Operator:
                    {
                        while (m_TokenStack.Count > 0 && m_TokenStack.Peek().Type == TokenType.Operator &&
                               OperatorPrecedence[m_TokenStack.Peek().Value[0]] >= OperatorPrecedence[token.Value[0]])
                        {
                            m_TokenPostfixBuffer.Add(m_TokenStack.Pop());
                        }

                        m_TokenStack.Push(token);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            while (m_TokenStack.Count > 0)
            {
                m_TokenPostfixBuffer.Add(m_TokenStack.Pop());
            }
        }

        /// <summary>
        /// 构建表达式树
        /// </summary>
        private void BuildExpressionTree()
        {
            m_TreeNodeStack.Clear();
            foreach (var token in m_TokenPostfixBuffer)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                    case TokenType.Variable:
                        m_TreeNodeStack.Push(CreateNode(token));
                        break;
                    case TokenType.Operator:
                    {
                        var node = CreateNode(token);
                        node.Right = m_TreeNodeStack.Pop();
                        node.Left = m_TreeNodeStack.Pop();
                        m_TreeNodeStack.Push(node);
                        break;
                    }
                    case TokenType.LeftParen:
                    case TokenType.RightParen:
                        // 无用Token 将其归还
                        ReturnToken(token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 计算表达式树
        /// </summary>
        private double EvaluateTree(object userData)
        {
            m_EvaluateStack.Clear();
            m_EvaluateNodeStack.Clear();
            m_VisitedNode.Clear();

            if (m_TreeNodeStack.Count > 0)
                m_EvaluateNodeStack.Push(m_TreeNodeStack.Peek());

            while (m_EvaluateNodeStack.Count > 0)
            {
                var node = m_EvaluateNodeStack.Peek();

                switch (node.Token.Type)
                {
                    case TokenType.Number:
                        m_EvaluateNodeStack.Pop();
                        m_EvaluateStack.Push(double.Parse(node.Token.Value));
                        break;
                    case TokenType.Variable:
                    {
                        m_EvaluateNodeStack.Pop();
                        var parsed = false;

                        foreach (var parser in m_TokenParsers)
                        {
                            if (!parser(node.Token.Value, userData, out var value))
                                continue;

                            m_EvaluateStack.Push(value);
                            parsed = true;
                            break;
                        }

                        if (!parsed)
                            throw new ArgumentException($"Unknown token: {node.Token.Value}");
                        
                        break;
                    }
                    case TokenType.Operator:
                        if (m_VisitedNode.Add(node))
                        {
                            m_EvaluateNodeStack.Push(node.Right);
                            m_EvaluateNodeStack.Push(node.Left);
                        }
                        else
                        {
                            double rightValue = m_EvaluateStack.Pop();
                            double leftValue = m_EvaluateStack.Pop();

                            double result = node.Token.Value switch
                            {
                                "+" => leftValue + rightValue,
                                "-" => leftValue - rightValue,
                                "*" => leftValue * rightValue,
                                "/" => leftValue / rightValue,
                                _ => throw new InvalidOperationException("Invalid operator")
                            };

                            m_EvaluateStack.Push(result);
                            m_EvaluateNodeStack.Pop();
                        }

                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // 计算完成 归还节点
            while (m_TreeNodeStack.Count > 0)
            {
                ReturnNode(m_TreeNodeStack.Pop());
            }

            return m_EvaluateStack.Pop();
        }

        /// <summary>
        /// 是否是变量表达式能接受的字符
        /// </summary>
        private bool IsVariableMiddleOrEndChar(char c)
        {
            if (char.IsWhiteSpace(c))
                return false;

            if (OperatorPrecedence.ContainsKey(c))
                return false;

            if (ParenMap.ContainsKey(c))
                return false;

            return true;
        }

        /// <summary>
        /// 创建树节点
        /// </summary>
        private TreeNode CreateNode(Token token)
        {
            var node = m_NodePool.Count > 0 ? m_NodePool.Dequeue() : new TreeNode();
            node.Token = token;
            return node;
        }

        /// <summary>
        /// 归还节点
        /// </summary>
        private void ReturnNode(TreeNode node)
        {
            if (node.Token != null)
                ReturnToken(node.Token);

            node.Token = null;
            node.Left = null;
            node.Right = null;

            m_NodePool.Enqueue(node);
        }

        /// <summary>
        /// 创建token
        /// </summary>
        private Token CreateToken(TokenType tokenType, string value)
        {
            var token = m_TokenPool.Count > 0 ? m_TokenPool.Dequeue() : new Token();
            token.Value = value;
            token.Type = tokenType;
            return token;
        }

        /// <summary>
        /// 归还token
        /// </summary>
        private void ReturnToken(Token token)
        {
            m_TokenPool.Enqueue(token);
        }
    }
}