namespace HoweFramework
{
    /// <summary>
    /// 声音模块扩展。
    /// </summary>
    public static class SoundModuleExtensions
    {
        /// <summary>
        /// 使用Unity的AudioClip声音模块。
        /// </summary>
        /// <param name="soundModule">声音模块。</param>
        /// <returns>声音模块。</returns>
        public static SoundModule UseAudioClipSound(this SoundModule soundModule)
        {
            soundModule.SetSoundHelper(new AudioClipSoundHelper());
            return soundModule;
        }
    }
}
