namespace Grains.Extensions;

public static class TaskExtensions
{
    /// <summary>
    /// 忽略任务。
    /// </summary>
    /// <param name="task">任务。</param>
    public static void Forget(this Task task)
    {
        _ = task;
    }

    /// <summary>
    /// 忽略任务。
    /// </summary>
    /// <param name="task">任务。</param>
    public static void Forget<T>(this Task<T> task)
    {
        _ = task;
    }

    /// <summary>
    /// 忽略任务。
    /// </summary>
    /// <param name="task">任务。</param>
    public static void Forget(this ValueTask task)
    {
        _ = task;
    }

    /// <summary>
    /// 忽略任务。
    /// </summary>
    /// <param name="task">任务。</param>
    public static void Forget<T>(this ValueTask<T> task)
    {
        _ = task;
    }
}

