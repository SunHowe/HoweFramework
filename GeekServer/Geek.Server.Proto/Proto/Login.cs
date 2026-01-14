
using MessagePack;

namespace Geek.Server.Proto
{
    /// <summary>
    /// 玩家基础信息
    /// </summary>
    [MessagePackObject(true)]
    public class UserInfo
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// vip等级
        /// </summary>
        public int VipLevel { get; set; }
    }

    /// <summary>
    /// 请求登录
    /// </summary>
    [MessagePackObject(true)]
    public class LoginReq : Message
    {
        public string UserName { get; set; }
        public string Platform { get; set; }
        public int SdkType { get; set; }
        public string SdkToken { get; set; }
        public string Device { get; set; }
    }

    /// <summary>
    /// 请求登录
    /// </summary>
    [MessagePackObject(true)]
    public class LoginResp : ResponseMessage
    {
        public UserInfo UserInfo { get; set; }
        public UserBagInfo BagInfo { get; set; }
    }

    /// <summary>
    /// 被踢下线原因
    /// </summary>
    public enum KickOutReason
    {
        /// <summary>
        /// 其他设备登录
        /// </summary>
        OtherClientLogin = 1,
    }

    /// <summary>
    /// 被踢下线
    /// </summary>
    [MessagePackObject(true)]
    public class KickOut : Message
    {
        public KickOutReason Reason { get; set; }
    }
}
