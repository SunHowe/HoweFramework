using Cysharp.Threading.Tasks;
using GameMain.UI;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 加载进度更新事件。
    /// </summary>
    public sealed class LoadingProgressUpdateEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(LoadingProgressUpdateEventArgs).GetHashCode();
        public override int Id => EventId;

        /// <summary>
        /// 加载描述。
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 加载进度。
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 创建加载进度更新事件。
        /// </summary>
        /// <param name="description">加载描述。</param>
        /// <param name="progress">加载进度。</param>
        /// <returns>创建的加载进度更新事件。</returns>
        public static LoadingProgressUpdateEventArgs Create(string description, float progress)
        {
            LoadingProgressUpdateEventArgs loadingProgressUpdateEventArgs = ReferencePool.Acquire<LoadingProgressUpdateEventArgs>();
            loadingProgressUpdateEventArgs.Description = description;
            loadingProgressUpdateEventArgs.Progress = progress;
            return loadingProgressUpdateEventArgs;
        }

        public override void Clear()
        {
            Description = null;
            Progress = 0f;
        }
    }

    /// <summary>
    /// 加载游戏流程。
    /// </summary>
    public sealed class ProcedureLoading : ProcedureBase
    {
        public override int Id => (int)ProcedureId.Loading;

        private bool m_IsLoadComplete = false;

        protected override void OnEnter()
        {
            LoadGameAsync().Forget();
        }

        protected override void OnLeave()
        {
            UIModule.Instance.CloseUIForm(UIFormId.LoadingForm).Forget();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_IsLoadComplete)
            {
                ChangeProcedure((int)ProcedureId.Game);
            }
        }

        private async UniTask LoadGameAsync()
        {
            // 打开Loading界面。
            await UIModule.Instance.OpenUIFormOnlyCareAboutFormOpen(UIFormId.LoadingForm);

            // 关闭登录界面，卸载登录场景。
            UIModule.Instance.CloseUIForm(UIFormId.LoginForm).Forget();
            SceneModule.Instance.UnloadSceneAsync(ProcedureLogin.LoginSceneAssetName).Forget();

            var gameContext = IOCModule.Instance.Get<IGameContext>();
            if (gameContext is ILoadable loadable)
            {
                await loadable.LoadAsync(OnLoadProgress);
            }
            
            EventModule.Instance.Dispatch(this, LoadingProgressUpdateEventArgs.Create("加载完成", 1f));

            m_IsLoadComplete = true;
        }

        /// <summary>
        /// 加载进度回调。
        /// </summary>
        /// <param name="progress">加载进度。0-1。</param>
        private void OnLoadProgress(float progress)
        {
            EventModule.Instance.Dispatch(this, LoadingProgressUpdateEventArgs.Create("加载中", progress));
        }
    }
}