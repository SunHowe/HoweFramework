using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 请求接口。
    /// </summary>
    public interface IRequest
    {
        UniTask<IResponse> Execute(CancellationToken token = default);
    }
}

