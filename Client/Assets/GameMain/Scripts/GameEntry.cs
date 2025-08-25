using HoweFramework;
using UnityEngine;

namespace GameMain
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField]
        private GameConfig m_GameConfig;

        private GameApp m_GameApp;

        private void Awake()
        {
            GameConfig.Instance = m_GameConfig;

            m_GameApp = new GameApp();
            m_GameApp.OnGameAppDestroyed += OnGameAppDestroyed;
        }

        private void Start()
        {
            var procedures = new ProcedureBase[]
            {
                new ProcedureSplash(),
                new ProcedureLoadDataTable(),
                new ProcedureLoadLocalization(),
                new ProcedureInitSystem(),
                new ProcedureLogin(),
                new ProcedureLoading(),
                new ProcedureGame(),
            };
            
            ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures);
        }

        private void OnDestroy()
        {
            m_GameApp.Destroy();
        }

        private void Update()
        {
            m_GameApp.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnGameAppDestroyed()
        {
        }
    }
}