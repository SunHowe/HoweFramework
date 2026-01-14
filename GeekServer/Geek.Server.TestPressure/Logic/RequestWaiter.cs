using System.Collections.Concurrent;

namespace Geek.Server.TestPressure.Logic;

public class RequestWaiter : IDisposable
{
    private readonly ConcurrentDictionary<int, TaskCompletionSource<ResponseMessage>> m_Waiters = new();
    
    /// <summary>
    /// 创建异步任务实例。
    /// </summary>
    public Task<ResponseMessage> CreateWait(int uniId)
    {
        var tcs = new TaskCompletionSource<ResponseMessage>();
        m_Waiters.TryAdd(uniId, tcs);
        return tcs.Task;
    }

    /// <summary>
    /// 设置响应结果。
    /// </summary>
    public void SetResponse(int uniId, ResponseMessage resp)
    {
        if (m_Waiters.TryRemove(uniId, out var tcs))
        {
            tcs.TrySetResult(resp);
        }
    }
    
    public void Dispose()
    {
        while (!m_Waiters.IsEmpty)
        {
            var first = m_Waiters.First();
            if (m_Waiters.TryRemove(first.Key, out var tcs))
            {
                tcs.TrySetCanceled();
            }
        }
    }
}