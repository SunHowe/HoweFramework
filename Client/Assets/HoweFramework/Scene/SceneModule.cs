using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 场景模块。
    /// </summary>
    public sealed class SceneModule : ModuleBase<SceneModule>
    {
        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="sceneName">场景名称。</param>
        /// <returns>场景。</returns>
        public UniTask LoadSceneAsync(string sceneName)
        {
            return ResModule.Instance.GetResCoreLoader().LoadScene(sceneName);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneName">场景名称。</param>
        public UniTask UnloadSceneAsync(string sceneName)
        {
            return ResModule.Instance.GetResCoreLoader().UnloadScene(sceneName);
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
