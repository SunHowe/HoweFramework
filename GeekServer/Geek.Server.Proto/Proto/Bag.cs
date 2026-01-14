
using MessagePack;
using System.Collections.Generic;

namespace Geek.Server.Proto
{
    /// <summary>
    /// 用户背包信息。
    /// </summary>
    [MessagePackObject(true)]
    public class UserBagInfo : Message
    {
        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();
    }

    /// <summary>
    /// 请求合成宠物
    /// </summary>
    [MessagePackObject(true)]
    public class BagComposePetReq : Message
    {
        /// <summary>
        /// 碎片id
        /// </summary>
        public int FragmentId { get; set; }
    }

    /// <summary>
    /// 返回合成宠物
    /// </summary>
    [MessagePackObject(true)]
    public class BagComposePetResp : ResponseMessage
    {
        /// <summary>
        /// 合成宠物的Id
        /// </summary>
        public int PetId { get; set; }
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    [MessagePackObject(true)]
    public class BagUseItemReq : Message
    {
        /// <summary>
        /// 道具id
        /// </summary>
        public int ItemId { get; set; }
    }

    /// <summary>
    /// 出售道具
    /// </summary>
    [MessagePackObject(true)]
    public class BagSellItemReq : Message
    {
        /// <summary>
        /// 道具id
        /// </summary>
        public int ItemId { get; set; }
    }

}
