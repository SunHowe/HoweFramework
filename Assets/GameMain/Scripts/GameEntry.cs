using System;
using Cysharp.Threading.Tasks;
using FairyGUI;
using GameMain.UI;
using HoweFramework;
using UnityEngine;

namespace GameMain
{
    public class GameEntry : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            GameApp.OnGameAppInited += OnGameAppInited;
            GameApp.OnGameAppDestroyed += OnGameAppDestroyed;
        }

        private void Start()
        {
            GameApp.NewApp();
        }

        private void OnDestroy()
        {
            GameApp.Instance?.Destroy();
        }

        private void Update()
        {
            GameApp.Instance?.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnGameAppInited()
        {
            LaunchGame().Forget();
        }

        private void OnGameAppDestroyed()
        {
        }

        private async UniTask LaunchGame()
        {
            await ResModule.Instance.UseYooAssetEditorSimulateMode();
            await ResModule.Instance.RequestUpdatePackageManifest();

            await UIModule.Instance.UseFairyGUI(new FairyGUISettings());
            await UIModule.Instance.LoadFairyGUIPackagesAsync(UIConst.UIPackageMappingAssetPath);
            UIModule.Instance.AddFairyGUIFormBindings(UIFormBindings.Bindings);
            UIModule.Instance.AddFairyGUIComponentBindings(UIComponentBindings.Bindings);

            for (UIGroupId i = 0; i < UIGroupId.Count; i++)
            {
                UIModule.Instance.CreateUIFormGroup((int)i, i.ToString());
            }

            await UIModule.Instance.OpenUIForm(UIFormId.LoginForm);
        }

        private AutoResetUniTaskCompletionSource m_NextTcs;

        void OnGUI()
        {
            if (m_NextTcs != null && GUILayout.Button("Next"))
            {
                var cts = m_NextTcs;
                m_NextTcs = null;
                cts.TrySetResult();
            }
        }

        private UniTask WaitForNext()
        {
            m_NextTcs = AutoResetUniTaskCompletionSource.Create();
            return m_NextTcs.Task;
        }
    }
}