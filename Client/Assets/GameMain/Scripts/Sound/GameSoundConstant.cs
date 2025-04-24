using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 游戏声音常量。
    /// </summary>
    public static class GameSoundConstant
    {
        /// <summary>
        /// 声音组设置。
        /// </summary>
        public static readonly Dictionary<SoundGroupId, SoundGroupSetting> SoundGroupSettings = new()
        {
            { SoundGroupId.Music, new SoundGroupSetting(1f, 1) },
            { SoundGroupId.Effect, new SoundGroupSetting(1f, 10) },
            { SoundGroupId.UI, new SoundGroupSetting(1f, 3) },
        };

        /// <summary>
        /// 声音组音量设置键值格式。
        /// </summary>
        public const string SoundGroupVolumeSettingKeyFormat = "Game.Sound.{0}.Volume";
    }
}
