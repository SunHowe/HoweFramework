using HoweFramework;
using UnityEngine;

namespace GameMain
{
    public class GameEntry : MonoBehaviour
    {
        private GameApp m_GameApp;

        private void Awake()
        {
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
                new ProcedureLobby(),
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