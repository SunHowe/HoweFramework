using System;

namespace GameMain
{
    /// <summary>
    /// 玩家信息。
    /// </summary>
    public readonly struct PlayerInfo
    {
        public Guid PlayerId { get; }
        public string PlayerName { get; }
        public DateTime LoginTime { get; }
        public DateTime CreateTime { get; }

        public PlayerInfo(Guid playerId, string playerName, DateTime loginTime, DateTime createTime)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            LoginTime = loginTime;
            CreateTime = createTime;
        }
    }
}
