namespace HoweFramework
{
    /// <summary>
    /// 播放声音参数。
    /// </summary>
    public sealed class PlaySoundParams : IReference
    {
        /// <summary>
        /// 是否循环播放。
        /// </summary>
        public bool Loop { get; set; }

        /// <summary>
        /// 音量。实际音量=全局音量*声音组音量*播放时指定的音量。
        /// </summary>
        public float Volume { get; set; }


        public void Clear()
        {
        }
    }
}

