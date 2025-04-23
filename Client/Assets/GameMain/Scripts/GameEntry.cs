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
            var procedures = new ProcedureBase[]
            {
                new ProcedureSplash(),
                new ProcedureLoadDataTable(),
                new ProcedureLoadLocalization(),
                new ProcedureLogin(),
            };
            
            ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures);
        }

        private void OnGameAppDestroyed()
        {
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