using System;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 本地化数据源。
    /// </summary>
    public interface ILocalizationSource : IDisposable
    {
        /// <summary>
        /// 加载本地化数据。
        /// </summary>
        /// <param name="language">语言。</param>
        /// <returns>本地化数据。</returns>
        UniTask Load(Language language);
    }
}

