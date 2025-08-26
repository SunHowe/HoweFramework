using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 贪吃蛇游戏输入控制器。
    /// </summary>
    public sealed class SnakeGameInputController : ProcedureControllerBase
    {
        private IGameContext m_GameContext;
        private ISnakeGameManager m_SnakeGameManager;

        protected override void OnInitialize()
        {
            m_GameContext = IOCModule.Instance.Get<IGameContext>();
            m_SnakeGameManager = m_GameContext.GetManager<ISnakeGameManager>();

            EnableFrameTimer();
        }

        protected override void OnDispose()
        {
        }

        protected override void OnUpdate()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var horizontalAbs = Mathf.Abs(horizontal);
            var verticalAbs = Mathf.Abs(vertical);

            if (horizontalAbs >= verticalAbs)
            {
                // 按水平方向移动。
                if (horizontal > 0)
                {
                    m_SnakeGameManager.SubmitMoveDirection(MoveDirection.Right);
                }
                else if (horizontal < 0)
                {
                    m_SnakeGameManager.SubmitMoveDirection(MoveDirection.Left);
                }
            }
            else
            {
                if (vertical > 0)
                {
                    m_SnakeGameManager.SubmitMoveDirection(MoveDirection.Up);
                }
                else if (vertical < 0)
                {
                    m_SnakeGameManager.SubmitMoveDirection(MoveDirection.Down);
                }
            }

        }
    }
}