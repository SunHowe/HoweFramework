using System;

namespace HoweFramework
{
    public delegate void SoundStopCallback(int serialId, string soundAssetName, object soundAsset);

    /// <summary>
    /// 声音组辅助器接口。
    /// </summary>
    internal interface ISoundGroupHelper : IDisposable
    {
        /// <summary>
        /// 声音组编号。
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 声音组名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 声音组音量。
        /// </summary>
        float Volume { get; }

        /// <summary>
        /// 声音组中声音数量限制。
        /// </summary>
        int SoundLimit { get; }

        /// <summary>
        /// 设置声音组音量。
        /// </summary>
        /// <param name="volume">声音组音量。</param>
        void SetVolume(float volume);

        /// <summary>
        /// 设置声音停止回调。
        /// </summary>
        /// <param name="soundStopCallback">声音停止回调。</param>
        void SetSoundStopCallback(SoundStopCallback soundStopCallback);

        /// <summary>
        /// 设置全局音量。
        /// </summary>
        /// <param name="globalVolume">全局音量。</param>
        void SetGlobalVolume(float globalVolume);

        /// <summary>
        /// 播放声音。
        /// </summary>
        /// <param name="serialId">声音序列编号。</param>
        /// <param name="soundAssetName">声音资源名称。</param>
        /// <param name="soundAsset">声音资源。</param>
        /// <param name="playSoundParams">播放声音参数。</param>
        void PlaySound(int serialId, string soundAssetName, object soundAsset, PlaySoundParams playSoundParams);

        /// <summary>
        /// 停止声音。
        /// </summary>
        /// <param name="serialId">声音序列编号。</param>
        void StopSound(int serialId);

        /// <summary>
        /// 停止声音。
        /// </summary>
        /// <param name="soundAssetName">声音资源名称。</param>
        void StopSound(string soundAssetName);

        /// <summary>
        /// 停止所有声音。
        /// </summary>
        void StopAllSounds();

        /// <summary>
        /// 暂停声音。
        /// </summary>
        /// <param name="serialId">声音序列编号。</param>
        void PauseSound(int serialId);

        /// <summary>
        /// 恢复声音。
        /// </summary>
        /// <param name="serialId">声音序列编号。</param>
        void ResumeSound(int serialId);

        /// <summary>
        /// 更新声音组辅助器。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds, float realElapseSeconds);
    }
}
