namespace IGrains;

/// <summary>
/// 官方账号登录。
/// </summary>
public interface IOfficialAccountLoginGrain : IGrainWithStringKey
{
    /// <summary>
    /// 登录请求, 若成功则返回唯一id.
    /// </summary>
    Task<Guid> Login(string password);
}