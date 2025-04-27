using Grains.Utility;
using GrainStates.Login;
using IGrains;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 官方账号登录请求。
/// </summary>
public class OfficialAccountLoginGrain : Grain<OfficialAccountState>, IOfficialAccountLoginGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        await ReadStateAsync();
    }

    public async Task<Guid> Login(string password)
    {
        var passwordHash = EncryptUtility.MD5Encrypt(password);
        
        if (State.UserId == Guid.Empty)
        {
            // new create.
            var userId = Guid.NewGuid();
            State.UserId = Guid.NewGuid();
            State.PasswordHash = passwordHash;
            await WriteStateAsync();

            return userId;
        }

        if (State.PasswordHash != passwordHash)
        {
            throw new GameException(HoweFramework.ErrorCode.LoginAuthFailed);
        }
        
        return State.UserId;
    }
}