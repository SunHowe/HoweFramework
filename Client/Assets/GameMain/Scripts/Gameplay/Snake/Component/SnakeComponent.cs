using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 贪吃蛇组件。
    /// </summary>
    [GameComponent(GameComponentType.Snake)]
    public sealed class SnakeComponent : GameComponentBase
    {
        /// <summary>
        /// 贪吃蛇节点列表。
        /// </summary>
        public List<SnakeNodeComponent> Nodes { get; } = new();

        /// <summary>
        /// 移动方向。
        /// </summary>
        public MoveDirection Direction => m_SnakeGameManager.MoveDirection.Value;

        /// <summary>
        /// 移动速度。
        /// </summary>
        public float MoveSpeed => m_SnakeGameManager.MoveSpeed.Value;

        /// <summary>
        /// 移动间隔(移动1个单位长度所需的时间)。
        /// </summary>
        public float MoveInterval => 1f / MoveSpeed;

        private ISnakeGameManager m_SnakeGameManager;
        private IGameUpdateManager m_GameUpdateManager;
        private IGameEntityManager m_GameEntityManager;
        private float m_ElapsedTime;

        /// <summary>
        /// 初始化贪吃蛇。
        /// </summary>
        /// <param name="count">节点数量。</param>
        /// <param name="headPosition">头部位置。</param>
        /// <param name="direction">移动方向。</param>
        public void InitSnake(int count, Vector2 headPosition, MoveDirection direction)
        {
            var position = headPosition;
            var positionOffset = GetMoveOffset(direction);

            for (int i = 0; i < count; i++)
            {
                var entity = m_GameEntityManager.CreateEntity();
                var node = entity.AddComponent<SnakeNodeComponent>();
                node.Position = position;
                node.Direction = direction;
                node.ViewTransformSyncComponent.ForceSync();
                Nodes.Add(node);

                position -= positionOffset;
            }
        }

        protected override void OnAwake()
        {
            m_SnakeGameManager = Context.GetManager<ISnakeGameManager>();
            m_GameUpdateManager = Context.GetManager<IGameUpdateManager>();
            m_GameEntityManager = Context.GetManager<IGameEntityManager>();

            m_GameUpdateManager.RegisterFixedUpdate(this, OnFixedUpdate);
        }

        protected override void OnDispose()
        {
            Nodes.Clear();
            m_GameUpdateManager.UnregisterByTarget(this);
            m_GameEntityManager = null;
            m_GameUpdateManager = null;
            m_SnakeGameManager = null;
        }

        private void OnFixedUpdate(float deltaTime)
        {
            m_ElapsedTime += deltaTime;

            if (m_ElapsedTime < MoveInterval)
            {
                return;
            }

            var interval = MoveInterval;
            var direction = Direction;
            var moveOffset = GetMoveOffset(direction);

            do
            {
                m_ElapsedTime -= MoveInterval;

                var head = Nodes[0];
                head.Position += moveOffset;
                head.Direction = direction;

                for (var i = 1; i < Nodes.Count; i++)
                {
                    var node = Nodes[i];
                    var prevNode = Nodes[i - 1];
                    node.Position = prevNode.PrevPosition;
                    node.Direction = prevNode.PrevDirection;
                }
            }
            while (m_ElapsedTime >= interval);
        }

        private Vector2 GetMoveOffset(MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Up => new Vector2(0, 1),
                MoveDirection.Down => new Vector2(0, -1),
                MoveDirection.Left => new Vector2(-1, 0),
                MoveDirection.Right => new Vector2(1, 0),
                _ => Vector2.zero
            };
        }
    }
}