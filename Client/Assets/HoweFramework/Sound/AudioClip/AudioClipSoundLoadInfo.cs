using System;
using System.Threading;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 音频资源加载信息。
    /// </summary>
    internal sealed class AudioClipSoundLoadInfo : IReference, IDisposable
    {
        /// <summary>
        /// 声音序列编号。
        /// </summary>
        public int SerialId { get; set; }

        /// <summary>
        /// 声音组编号。
        /// </summary>
        public int GroupId { get; set; }

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
        /// 取消令牌源。
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// 是否暂停。
        /// </summary>
        public bool IsPause { get; set; }

        public void Clear()
        {
            SerialId = 0;
            GroupId = 0;
            SoundAssetName = null;
            SoundAsset = null;

            if (PlaySoundParams != null)
            {
                ReferencePool.Release(PlaySoundParams);
                PlaySoundParams = null;
            }

            IsPause = false;

            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}
