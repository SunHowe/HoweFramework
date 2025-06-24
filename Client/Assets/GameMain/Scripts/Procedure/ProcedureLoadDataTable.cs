using System;
using Cysharp.Threading.Tasks;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 加载数据表流程。
    /// </summary>
    public sealed class ProcedureLoadDataTable : ProcedureBase
    {
        public override int Id => (int)ProcedureId.LoadDataTable;
        
        private bool m_IsComplete;

        protected override void OnEnter()
        {
            m_IsComplete = false;
            
            // 添加数据表数据源。
            DataTableModule.Instance.AddDataTableSource(new GameMainDataTableSource());

            switch (DataTableModule.Instance.LoadMode)
            {
                case DataTableLoadMode.AsyncLoad:
                case DataTableLoadMode.LazyLoadAndPreloadAsync:
                    // 异步预加载配置。
                    DataTableModule.Instance.PreloadAsync().ContinueWith(() => m_IsComplete = true).Forget();
                    break;
                case DataTableLoadMode.LazyLoadAndPreloadSync:
                case DataTableLoadMode.SyncLoad:
                    // 同步预加载配置。
                    DataTableModule.Instance.Preload();
                    m_IsComplete = true;
                    break;
                case DataTableLoadMode.LazyLoad:
                    // 不做预加载。
                    m_IsComplete = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnLeave()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (!m_IsComplete) 
            {
                return;
            }
            
            ChangeNextProcedure();
        }
    }
}
