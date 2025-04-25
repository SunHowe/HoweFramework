using IGrains;

namespace Grains;

/// <summary>
/// 登录Grain.
/// </summary>
public class LoginGrain : Grain, ILoginGrain
{
    public Task<Guid> Login(string account, string password)
    {
        // TODO.

        return Task.FromResult(Guid.NewGuid());
    }
}