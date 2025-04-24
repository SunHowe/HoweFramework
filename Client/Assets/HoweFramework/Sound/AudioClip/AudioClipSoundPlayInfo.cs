using System;
using System.Threading;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 使用AudioClip的声音播放信息。
    /// </summary>
    internal sealed class AudioClipSoundPlayInfo : IReference, IDisposable
    {
        /// <summary>
        /// 声音序列编号。
        /// </summary>
        public int SerialId { get; set; }

        /// <summary>
        /// 音频源。
        /// </summary>
        public AudioSource AudioSource { get; set; }

        /// <summary>
        /// 声音资源名称。
        /// </summary>
        public string SoundAssetName { get; set; }

        /// <summary>
        /// 声音资源。
        /// </summary>
        public AudioClip SoundAsset { get; set; }

        /// <summary>
        /// 播放声音参数。
        /// </summary>
        public PlaySoundParams PlaySoundParams { get; set; }

        /// <summary>
        /// 已流逝时间。
        /// </summary>
        public float ElapseSeconds { get; set; }

        /// <summary>
        /// 声音时长。
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// 是否暂停。
        /// </summary>
        public bool IsPause { get; private set; }

        public void Clear()
        {
            SerialId = 0;
            AudioSource = null;
            SoundAssetName = null;
            SoundAsset = null;

            if (PlaySoundParams != null)
            {
                ReferencePool.Release(PlaySoundParams);
                PlaySoundParams = null;
            }

            if (AudioSource != null)
            {
                AudioSource.Stop();
                AudioSource = null;
            }

            ElapseSeconds = 0f;
            Duration = 0f;
            IsPause = false;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Pause()
        {
            if (IsPause)
            {
                return;
            }

            IsPause = true;
            AudioSource.Pause();
        }

        public void Resume()
        {
            if (!IsPause)
            {
                return;
            }

            IsPause = false;
            AudioSource.UnPause();
        }
    }
}

