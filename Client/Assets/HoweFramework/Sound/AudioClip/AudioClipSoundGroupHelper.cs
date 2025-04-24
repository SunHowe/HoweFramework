using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 使用AudioClip的声音组辅助器。
    /// </summary>
    internal sealed class AudioClipSoundGroupHelper : ISoundGroupHelper
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public float Volume { get; private set; }

        public int SoundLimit { get; private set; }

        private float m_GlobalVolume;
        private float m_MixedVolume;
        private readonly Queue<AudioSource> m_AudioSourcePool = new();
        private readonly LinkedListEx<AudioClipSoundPlayInfo> m_PlayInfoList = new();
        private readonly Transform m_Root = null;
        private SoundStopCallback m_SoundStopCallback;

        public AudioClipSoundGroupHelper(int id, string name, float volume, float globalVolume, int soundLimit, Transform root)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(root);
            m_Root = gameObject.transform;

            Id = id;
            Name = name;
            Volume = volume;
            SoundLimit = soundLimit;
            m_GlobalVolume = globalVolume;
            m_MixedVolume = Volume * m_GlobalVolume;
        }

        public void Dispose()
        {
            m_AudioSourcePool.Clear();
            m_PlayInfoList.Clear();

            if (m_Root != null)
            {
                UnityEngine.Object.Destroy(m_Root.gameObject);
            }
        }

        public void SetSoundStopCallback(SoundStopCallback soundStopCallback)
        {
            m_SoundStopCallback = soundStopCallback;
        }

        public void SetVolume(float volume)
        {
            Volume = volume;
            m_MixedVolume = Volume * m_GlobalVolume;
            UpdatePlayingAudioSourceVolume();
        }

        public void SetGlobalVolume(float globalVolume)
        {
            m_GlobalVolume = globalVolume;
            m_MixedVolume = Volume * m_GlobalVolume;
            UpdatePlayingAudioSourceVolume();
        }

        public void PlaySound(int serialId, string soundAssetName, object soundAsset, PlaySoundParams playSoundParams)
        {
            var audioClip = soundAsset as AudioClip;
            if (audioClip == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, $"Sound asset '{soundAssetName}' is not a AudioClip.");
            }

            CheckSoundLimit(true);

            AudioSource audioSource;
            if (m_AudioSourcePool.Count > 0)
            {
                audioSource = m_AudioSourcePool.Dequeue();
            }
            else
            {
                var gameObject = new GameObject("Audio Source");
                gameObject.transform.SetParent(m_Root);
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.ignoreListenerVolume = true;
                audioSource.ignoreListenerPause = true;
                audioSource.playOnAwake = false;
            }

            var playInfo = ReferencePool.Acquire<AudioClipSoundPlayInfo>();
            playInfo.SerialId = serialId;
            playInfo.AudioSource = audioSource;
            playInfo.SoundAssetName = soundAssetName;
            playInfo.SoundAsset = audioClip;
            playInfo.PlaySoundParams = playSoundParams;
            playInfo.ElapseSeconds = 0f;
            playInfo.Duration = audioClip.length;

            m_PlayInfoList.AddLast(playInfo);

            audioSource.volume = playSoundParams.Volume * m_MixedVolume;
            audioSource.loop = playSoundParams.Loop;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void StopSound(int serialId)
        {
            var node = m_PlayInfoList.First;
            while (node != null)
            {
                var nextNode = node.Next;
                var playInfo = node.Value;

                if (playInfo.SerialId == serialId)
                {
                    m_PlayInfoList.Remove(node);
                    StopPlayingSound(playInfo);
                    break;
                }

                node = nextNode;
            }
        }

        public void StopSound(string soundAssetName)
        {
            var node = m_PlayInfoList.First;
            while (node != null)
            {
                var nextNode = node.Next;
                var playInfo = node.Value;
                
                if (playInfo.SoundAssetName == soundAssetName)
                {
                    m_PlayInfoList.Remove(node);
                    StopPlayingSound(playInfo);
                }

                node = nextNode;
            }
        }

        public void StopAllSounds()
        {
            var node = m_PlayInfoList.First;
            while (node != null)
            {
                var nextNode = node.Next;
                var playInfo = node.Value;
                
                StopPlayingSound(playInfo);

                node = nextNode;
            }

            m_PlayInfoList.Clear();
        }

        public void PauseSound(int serialId)
        {
            var node = m_PlayInfoList.First;
            while (node != null)
            {
                var nextNode = node.Next;
                var playInfo = node.Value;

                if (playInfo.SerialId == serialId)
                {
                    playInfo.Pause();
                    break;
                }

                node = nextNode;
            }
        }

        public void ResumeSound(int serialId)
        {
            var node = m_PlayInfoList.First;
            while (node != null)
            {
                var nextNode = node.Next;
                var playInfo = node.Value;
                
                if (playInfo.SerialId == serialId)
                {
                    playInfo.Resume();
                    break;
                }

                node = nextNode;
            }
        }

        /// <summary>
        /// 检查声音数量限制。
        /// </summary>
        private void CheckSoundLimit(bool isNewComming)
        {
            var soundLimit = isNewComming ? SoundLimit - 1 : SoundLimit;
            if (soundLimit < 0)
            {
                return;
            }

            // 停止最早的声音。
            while (m_PlayInfoList.Count > soundLimit)
            {
                var info = m_PlayInfoList.First.Value;
                m_PlayInfoList.RemoveFirst();

                StopPlayingSound(info);
            }
        }

        /// <summary>
        /// 更新正在播放的声音音量。
        /// </summary>
        private void UpdatePlayingAudioSourceVolume()
        {
            if (m_PlayInfoList.Count == 0)
            {
                return;
            }

            foreach (var playInfo in m_PlayInfoList)
            {
                UpdateAudioSourceVolume(playInfo.AudioSource, playInfo.PlaySoundParams.Volume);
            }
        }

        /// <summary>
        /// 更新音频源音量。
        /// </summary>
        /// <param name="audioSource">音频源。</param>
        /// <param name="volume">音频文件指定的音量。</param>
        private void UpdateAudioSourceVolume(AudioSource audioSource, float volume)
        {
            if (audioSource == null)
            {
                return;
            }

            audioSource.volume = volume * m_MixedVolume;
        }

        /// <summary>
        /// 更新声音组辅助器。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_PlayInfoList.Count == 0)
            {
                return;
            }

            var node = m_PlayInfoList.First;
            while (node != null)
            {
                var nextNode = node.Next;
                var playInfo = node.Value;

                if (playInfo.IsPause)
                {
                    node = nextNode;
                    continue;
                }

                playInfo.ElapseSeconds += elapseSeconds;

                if (!playInfo.PlaySoundParams.Loop && playInfo.ElapseSeconds >= playInfo.Duration)
                {
                    m_PlayInfoList.Remove(node);
                    StopPlayingSound(playInfo);
                }

                node = nextNode;
            }
        }

        /// <summary>
        /// 停止播放声音。
        /// </summary>
        /// <param name="playInfo">播放信息。</param>
        private void StopPlayingSound(AudioClipSoundPlayInfo playInfo)
        {
            playInfo.AudioSource.Stop();
            playInfo.AudioSource.clip = null;
            m_AudioSourcePool.Enqueue(playInfo.AudioSource);

            m_SoundStopCallback?.Invoke(playInfo.SerialId, playInfo.SoundAssetName, playInfo.SoundAsset);
            
            playInfo.Dispose();
        }
    }
}
