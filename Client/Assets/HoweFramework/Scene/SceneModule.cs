using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoweFramework
{
    /// <summary>
    /// 场景模块。
    /// </summary>
    public sealed class SceneModule : ModuleBase<SceneModule>
    {
        /// <summary>
        /// 场景顺序。
        /// </summary>
        private readonly SortedDictionary<string, int> m_SceneOrder = new SortedDictionary<string, int>(StringComparer.Ordinal);

        /// <summary>
        /// 游戏框架场景。
        /// </summary>
        private Scene m_GameFrameworkScene;
        
        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景。</returns>
        public async UniTask LoadSceneAsync(string sceneAssetName)
        {
            try
            {
                var scene = await ResModule.Instance.GetResCoreLoader().LoadScene(sceneAssetName);
                RefreshSceneOrder();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        public UniTask UnloadSceneAsync(string sceneAssetName)
        {
            m_SceneOrder.Remove(sceneAssetName);
            return ResModule.Instance.GetResCoreLoader().UnloadScene(sceneAssetName);
        }

        /// <summary>
        /// 设置场景顺序。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="sceneOrder">要设置的场景顺序。</param>
        public void SetSceneOrder(string sceneAssetName, int sceneOrder)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            if (SceneIsLoading(sceneAssetName))
            {
                m_SceneOrder[sceneAssetName] = sceneOrder;
                return;
            }

            if (SceneIsLoaded(sceneAssetName))
            {
                m_SceneOrder[sceneAssetName] = sceneOrder;
                RefreshSceneOrder();
                return;
            }

            Log.Error($"Scene '{sceneAssetName}' is not loaded or loading.");
        }

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            return ResModule.Instance.GetResCoreLoader().SceneIsLoaded(sceneAssetName);
        }
        
        /// <summary>
        /// 获取场景是否正在加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在加载。</returns>
        public bool SceneIsLoading(string sceneAssetName)
        {
            return ResModule.Instance.GetResCoreLoader().SceneIsLoading(sceneAssetName);
        }

        /// <summary>
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在卸载。</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            return ResModule.Instance.GetResCoreLoader().SceneIsUnloading(sceneAssetName);
        }

        /// <summary>
        /// 获取场景名称。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景名称。</returns>
        public static string GetSceneName(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return null;
            }

            int sceneNamePosition = sceneAssetName.LastIndexOf('/');
            if (sceneNamePosition + 1 >= sceneAssetName.Length)
            {
                Log.Error($"Scene asset name '{sceneAssetName}' is invalid.");
                return null;
            }

            string sceneName = sceneAssetName.Substring(sceneNamePosition + 1);
            sceneNamePosition = sceneName.LastIndexOf(".unity");
            if (sceneNamePosition > 0)
            {
                sceneName = sceneName.Substring(0, sceneNamePosition);
            }

            return sceneName;
        }

        /// <summary>
        /// 刷新场景排序。
        /// </summary>
        private void RefreshSceneOrder()
        {
            if (m_SceneOrder.Count > 0)
            {
                string maxSceneName = null;
                int maxSceneOrder = 0;
                foreach (KeyValuePair<string, int> sceneOrder in m_SceneOrder)
                {
                    if (SceneIsLoading(sceneOrder.Key))
                    {
                        continue;
                    }

                    if (maxSceneName == null)
                    {
                        maxSceneName = sceneOrder.Key;
                        maxSceneOrder = sceneOrder.Value;
                        continue;
                    }

                    if (sceneOrder.Value > maxSceneOrder)
                    {
                        maxSceneName = sceneOrder.Key;
                        maxSceneOrder = sceneOrder.Value;
                    }
                }

                if (maxSceneName == null)
                {
                    SetActiveScene(m_GameFrameworkScene);
                    return;
                }

                Scene scene = SceneManager.GetSceneByName(GetSceneName(maxSceneName));
                if (!scene.IsValid())
                {
                    Log.Error($"Active scene '{maxSceneName}' is invalid.");
                    return;
                }

                SetActiveScene(scene);
            }
            else
            {
                SetActiveScene(m_GameFrameworkScene);
            }
        }

        private void SetActiveScene(Scene activeScene)
        {
            Scene lastActiveScene = SceneManager.GetActiveScene();
            if (lastActiveScene != activeScene)
            {
                SceneManager.SetActiveScene(activeScene);
                EventModule.Instance.Dispatch(this, ActiveSceneChangedEventArgs.Create(lastActiveScene, activeScene));
            }
        }

        protected override void OnInit()
        {
            m_GameFrameworkScene = SceneManager.GetSceneAt(0);
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
