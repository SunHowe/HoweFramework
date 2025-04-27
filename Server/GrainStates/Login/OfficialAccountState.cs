using System;

namespace GrainStates.Login;

[Serializable]
public class OfficialAccountState
{
    /// <summary>
    /// 密码的hash值。
    /// </summary>
    public string PasswordHash { get; set; }
    
    /// <summary>
    /// 用户唯一id。
    /// </summary>
    public Guid UserId { get; set; }
}