namespace IGrains;

/// <summary>
/// 玩家功能模块Grain接口。
/// </summary>
public interface IPlayerFeatureGrain : IGrainWithGuidKey
{
    /// <summary>
    /// 登录成功。
    /// </summary>
    Task OnLoginSuccess();
}