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