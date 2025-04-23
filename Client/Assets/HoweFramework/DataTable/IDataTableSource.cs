using System;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 配置表数据源接口。
    /// </summary>
    public interface IDataTableSource : IDisposable
    {
        /// <summary>
        /// 初始化配置表数据源。
        /// </summary>
        /// <param name="loadMode">配置表加载模式。</param>
        void Init(DataTableLoadMode loadMode);

        /// <summary>
        /// 使用同步的方式预加载配置表。
        /// </summary>
        void Preload();

        /// <summary>
        /// 使用异步的方式预加载配置表。
        /// </summary>
        UniTask PreloadAsync();
    }
}
