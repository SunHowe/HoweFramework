using System;
using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 玩家模块。
    /// </summary>
    public sealed class PlayerModule : ModuleBase<PlayerModule>
    {
        /// <summary>
        /// 玩家Id。
        /// </summary>
        public Guid PlayerId { get; private set; }

        /// <summary>
        /// 玩家名称。
        /// </summary>
        public string PlayerName { get; private set; }

        /// <summary>
        /// 登录时间。
        /// </summary>
        public DateTime LoginTime { get; private set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 更新玩家信息。
        /// </summary>
        /// <param name="playerPush">玩家信息推送消息包。</param>
        public void UpdatePlayerInfo(PlayerPush playerPush)
        {
            PlayerId = playerPush.PlayerId;
            PlayerName = playerPush.PlayerName;
            LoginTime = playerPush.LoginTime;
            CreateTime = playerPush.CreateTime;
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnInit()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}