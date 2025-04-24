using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏声音工具。
    /// </summary>
    public static class SoundUtility
    {
        /// <summary>
        /// 初始化声音组。
        /// </summary>
        public static void InitSoundGroup()
        {
            for (SoundGroupId id = 0; id < SoundGroupId.Count; ++id)
            {
                var setting = GameSoundConstant.SoundGroupSettings[id];
                var volume = GetSoundGroupVolume(id);
                SoundModule.Instance.CreateSoundGroup((int)id, id.ToString(), volume, setting.SoundLimit);
            }
        }

        /// <summary>
        /// 获取声音组音量。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        /// <returns>声音组音量。</returns>
        public static float GetSoundGroupVolume(SoundGroupId groupId)
        {
            if (!GameSoundConstant.SoundGroupSettings.TryGetValue(groupId, out var setting))
            {
                throw new ErrorCodeException(ErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
            }

            var key = string.Format(GameSoundConstant.SoundGroupVolumeSettingKeyFormat, groupId.ToString());
            return SettingModule.Instance.GetFloat(key, setting.Volume);
        }

        /// <summary>
        /// 设置声音组音量。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        /// <param name="volume">声音组音量。</param>
        public static void SetSoundGroupVolume(SoundGroupId groupId, float volume)
        {
            var key = string.Format(GameSoundConstant.SoundGroupVolumeSettingKeyFormat, groupId.ToString());
            SettingModule.Instance.SetFloat(key, volume);
        }

        /// <summary>
        /// 播放声音。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        /// <param name="soundAssetName">声音资源名称。</param>
        /// <param name="volume">音量。</param>
        /// <param name="loop">是否循环播放。</param>
        /// <returns>声音编号。</returns>
        public static int PlaySound(SoundGroupId groupId, string soundAssetName, float volume = 1f, bool loop = false)
        {
            return SoundModule.Instance.PlaySound((int)groupId, soundAssetName, volume, loop);
        }
    }
}
