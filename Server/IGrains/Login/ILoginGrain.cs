namespace IGrains;

/// <summary>
/// 登录Grain.
/// </summary>
public interface ILoginGrain : IGrainWithGuidKey
{
    /// <summary>
    /// 登录请求, 若成功则返回唯一id.
    /// </summary>
    Task<Guid> Login(string account, string password);
}