using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏声音组设置。
    /// </summary>
    public sealed class SoundGroupSetting
    {
        public float Volume;
        public int SoundLimit;

        public SoundGroupSetting(float volume = SoundConstant.DefaultVolume, int soundLimit = SoundConstant.DefaultSoundLimit)
        {
            Volume = volume;
            SoundLimit = soundLimit;
        }
    }
}
