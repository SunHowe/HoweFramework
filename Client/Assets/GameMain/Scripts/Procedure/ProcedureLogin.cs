using System;
using Cysharp.Threading.Tasks;
using GameMain.UI;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 新游戏请求事件。
    /// </summary>
    public sealed class NewGameRequestEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(NewGameRequestEventArgs).GetHashCode();

        public override int Id => EventId;

        public override void Clear()
        {
        }

        public static NewGameRequestEventArgs Create()
        {
            return ReferencePool.Acquire<NewGameRequestEventArgs>();
        }
    }

    /// <summary>
    /// 登录流程。
    /// </summary>
    public sealed class ProcedureLogin : ProcedureBase
    {
        public const string LoginSceneAssetName = "Assets/GameMain/Scene/Login.unity";
        public override int Id => (int)ProcedureId.Login;

        private bool m_ReceiveNewGameRequest = false;

        protected override void OnEnter()
        {
            m_ReceiveNewGameRequest = false;

            // 打开登录场景。
            SceneModule.Instance.LoadSceneAsync(LoginSceneAssetName).Forget();

            // 打开登录界面。
            UIModule.Instance.OpenUIForm(UIFormId.LoginForm).Forget();

            EventModule.Instance.Subscribe(NewGameRequestEventArgs.EventId, OnNewGameRequest);
        }

        protected override void OnLeave()
        {
            EventModule.Instance.Unsubscribe(NewGameRequestEventArgs.EventId, OnNewGameRequest);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_ReceiveNewGameRequest)
            {
                // 创建游戏上下文实例，切换到加载游戏流程。
                var context = SnakeGameContext.Create(1, DateTime.Now.ToUnixTimestamp());
                IOCModule.Instance.Register<IGameContext>(context);

                ChangeProcedure((int)ProcedureId.Loading);
            }
        }

        /// <summary>
        /// 收到新游戏请求事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewGameRequest(object sender, GameEventArgs e)
        {
            m_ReceiveNewGameRequest = true;
        }
    }
}

