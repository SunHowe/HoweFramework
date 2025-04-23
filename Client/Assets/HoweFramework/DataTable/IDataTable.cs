using System;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 配置表接口。
    /// </summary>
    public interface IDataTable : IDisposable
    {
        /// <summary>
        /// 同步加载配置表。
        /// </summary>
        void Load(bool autoResolveRef = true);

        /// <summary>
        /// 异步加载配置表。
        /// </summary>
        UniTask LoadAsync(bool autoResolveRef = true);

        /// <summary>
        /// 处理引用关系。
        /// </summary>
        void ResolveRef();
    }
}