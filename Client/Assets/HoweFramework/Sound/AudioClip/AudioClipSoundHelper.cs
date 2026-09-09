using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoweFramework
{
    /// <summary>
    /// 使用AudioClip的声音辅助器。
    /// </summary>
    internal sealed class AudioClipSoundHelper : ISoundHelper
    {
        private AudioListener m_AudioListener = null;
        private Transform m_Root = null;
        private readonly Dictionary<int, AudioClipSoundGroupHelper> m_SoundGroupHelperDict = new();
        private float m_GlobalVolume;
        private IResLoader m_ResLoader = null;

        private readonly Dictionary<int, AudioClipSoundLoadInfo> m_LoadInfoDict = new();
        private readonly Dictionary<int, int> m_SoundGroupIdDict = new();

        private int m_SerialId = 0;

        public AudioClipSoundHelper()
        {
            var gameObject = new GameObject("Audio Helper");
            Object.DontDestroyOnLoad(gameObject);

            m_Root = gameObject.transform;
            m_AudioListener = gameObject.AddComponent<AudioListener>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void Dispose()
        {
            foreach (var loadInfo in m_LoadInfoDict.Values)
            {
                loadInfo.Dispose();
            }

            m_LoadInfoDict.Clear();

            foreach (var soundGroupHelper in m_SoundGroupHelperDict.Values)
            {
                soundGroupHelper.Dispose();
            }

            m_SoundGroupHelperDict.Clear();
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            if (m_Root != null)
            {
                Object.Destroy(m_Root.gameObject);
            }

            m_Root = null;
            m_AudioListener = null;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var soundGroupHelper in m_SoundGroupHelperDict.Values)
            {
                soundGroupHelper.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public void CreateSoundGroup(int groupId, string groupName, float volume, int soundLimit)
        {
            if (m_SoundGroupHelperDict.ContainsKey(groupId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundGroupAlreadyExists, $"Sound group '{groupId}' has been created.");
            }

            var soundGroupHelper = new AudioClipSoundGroupHelper(groupId, groupName, volume, m_GlobalVolume, soundLimit, m_Root);
            soundGroupHelper.SetSoundStopCallback(OnSoundStop);
            m_SoundGroupHelperDict.Add(groupId, soundGroupHelper);
        }

        public void DestroySoundGroup(int groupId)
        {
            if (!m_SoundGroupHelperDict.TryGetValue(groupId, out var soundGroupHelper))
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
            }

            m_SoundGroupHelperDict.Remove(groupId);
            CancelLoadingSounds(groupId);
            soundGroupHelper.Dispose();
        }

        public bool IsLoadingSound(int serialId)
        {
            return m_LoadInfoDict.ContainsKey(serialId);
        }

        public void PauseSound(int serialId)
        {
            if (m_LoadInfoDict.TryGetValue(serialId, out var loadInfo))
            {
                loadInfo.IsPause = true;
            }
            else if (m_SoundGroupIdDict.TryGetValue(serialId, out var groupId))
            {
                if (m_SoundGroupHelperDict.TryGetValue(groupId, out var soundGroupHelper))
                {
                    soundGroupHelper.PauseSound(serialId);
                }
                else
                {
                    throw new ErrorCodeException(FrameworkErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
                }
            }
            else
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundNotExist, $"Sound '{serialId}' not exist.");
            }
        }

        public int PlaySound(int groupId, string soundAssetName, PlaySoundParams playSoundParams)
        {
            if (!m_SoundGroupHelperDict.ContainsKey(groupId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
            }

            var serialId = ++m_SerialId;

            var loadInfo = ReferencePool.Acquire<AudioClipSoundLoadInfo>();
            loadInfo.SerialId = serialId;
            loadInfo.GroupId = groupId;
            loadInfo.SoundAssetName = soundAssetName;
            loadInfo.PlaySoundParams = playSoundParams;
            loadInfo.CancellationTokenSource = new CancellationTokenSource();
            loadInfo.IsPause = false;

            m_LoadInfoDict.Add(serialId, loadInfo);

            LoadSoundAsync(loadInfo).Forget();

            return serialId;
        }

        /// <summary>
        /// 加载声音。
        /// </summary>
        private async UniTask LoadSoundAsync(AudioClipSoundLoadInfo loadInfo)
        {
            var token = loadInfo.CancellationTokenSource.Token;

            // 缓存关键字段：取消路径下 loadInfo 可能已被回收复用，await 之后不可再访问其字段。
            var serialId = loadInfo.SerialId;
            var groupId = loadInfo.GroupId;
            var soundAssetName = loadInfo.SoundAssetName;

            var soundAsset = await m_ResLoader.LoadAssetAsync<AudioClip>(soundAssetName, token);

            // 加载结束后（无论成功/失败/取消）先移除加载记录，避免残留导致 Stop/Pause/Resume 误命中已回收对象。
            m_LoadInfoDict.Remove(serialId);

            if (token.IsCancellationRequested)
            {
                if (soundAsset != null)
                {
                    m_ResLoader.UnloadAsset(soundAssetName);
                }

                return;
            }

            if (soundAsset == null)
            {
                // 加载失败：卸载空引用并清理，避免字典残留。
                m_ResLoader.UnloadAsset(soundAssetName);
                loadInfo.Dispose();
                return;
            }

            if (!m_SoundGroupHelperDict.TryGetValue(groupId, out var soundGroupHelper))
            {
                m_ResLoader.UnloadAsset(soundAssetName);
                loadInfo.Dispose();
                Log.Error($"Sound group '{groupId}' not exist.");
                return;
            }

            m_SoundGroupIdDict.Add(serialId, groupId);
            soundGroupHelper.PlaySound(serialId, soundAssetName, soundAsset, loadInfo.PlaySoundParams);

            // 加载期间被暂停的声音，加载完成后保持暂停状态。
            if (loadInfo.IsPause)
            {
                soundGroupHelper.PauseSound(serialId);
            }

            loadInfo.PlaySoundParams = null;
            loadInfo.Dispose();
        }

        public void ResumeSound(int serialId)
        {
            if (m_LoadInfoDict.TryGetValue(serialId, out var loadInfo))
            {
                loadInfo.IsPause = false;
            }
            else if (m_SoundGroupIdDict.TryGetValue(serialId, out var groupId))
            {
                if (m_SoundGroupHelperDict.TryGetValue(groupId, out var soundGroupHelper))
                {
                    soundGroupHelper.ResumeSound(serialId);
                }
                else
                {
                    throw new ErrorCodeException(FrameworkErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
                }
            }
            else
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundNotExist, $"Sound '{serialId}' not exist.");
            }
        }

        public void SetResLoader(IResLoader resLoader)
        {
            m_ResLoader = resLoader;
        }

        public void SetVolume(float volume)
        {
            m_GlobalVolume = volume;

            foreach (var soundGroupHelper in m_SoundGroupHelperDict.Values)
            {
                // 设置全局音量，不能覆盖各组独立音量（实际音量 = 全局 × 组 × 播放）。
                soundGroupHelper.SetGlobalVolume(volume);
            }
        }

        public void StopAllSounds(int groupId)
        {
            CancelLoadingSounds(groupId);

            if (groupId == 0)
            {
                // 组编号为 0 时停止所有声音组中的声音。
                foreach (var groupHelper in m_SoundGroupHelperDict.Values)
                {
                    groupHelper.StopAllSounds();
                }

                return;
            }

            if (m_SoundGroupHelperDict.TryGetValue(groupId, out var soundGroupHelper))
            {
                soundGroupHelper.StopAllSounds();
            }
            else
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
            }
        }

        public void StopSound(int serialId)
        {
            if (m_LoadInfoDict.TryGetValue(serialId, out var loadInfo))
            {
                loadInfo.Dispose();
                m_LoadInfoDict.Remove(serialId);
            }
            else if (m_SoundGroupIdDict.TryGetValue(serialId, out var groupId))
            {
                if (m_SoundGroupHelperDict.TryGetValue(groupId, out var soundGroupHelper))
                {
                    soundGroupHelper.StopSound(serialId);
                }
                else
                {
                    throw new ErrorCodeException(FrameworkErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
                }
            }
            else
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundNotExist, $"Sound '{serialId}' not exist.");
            }
        }

        public void StopSound(string soundAssetName, int groupId)
        {
            CancelLoadingSounds(groupId, soundAssetName);

            if (groupId == 0)
            {
                // 组编号为 0 时停止所有声音组中匹配的声音。
                foreach (var groupHelper in m_SoundGroupHelperDict.Values)
                {
                    groupHelper.StopSound(soundAssetName);
                }

                return;
            }

            if (m_SoundGroupHelperDict.TryGetValue(groupId, out var soundGroupHelper))
            {
                soundGroupHelper.StopSound(soundAssetName);
            }
            else
            {
                throw new ErrorCodeException(FrameworkErrorCode.SoundGroupNotExist, $"Sound group '{groupId}' not exist.");
            }
        }

        private void CancelLoadingSounds(int groupId, string soundAssetName = null)
        {
            using var toRemove = ReusableList<int>.Create();
            foreach (var pair in m_LoadInfoDict)
            {
                if (groupId != 0 && pair.Value.GroupId != groupId)
                {
                    continue;
                }

                if (soundAssetName != null && pair.Value.SoundAssetName != soundAssetName)
                {
                    continue;
                }

                toRemove.Add(pair.Key);
            }

            foreach (var serialId in toRemove)
            {
                if (m_LoadInfoDict.Remove(serialId, out var loadInfo))
                {
                    loadInfo.Dispose();
                }
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            RefreshAudioListener();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            RefreshAudioListener();
        }

        private void RefreshAudioListener()
        {
            m_AudioListener.enabled = Object.FindObjectsOfType<AudioListener>().Length <= 1;
        }

        private void OnSoundStop(int serialId, string soundAssetName, object soundAsset)
        {
            m_SoundGroupIdDict.Remove(serialId);
            m_ResLoader.UnloadAsset(soundAssetName);
        }
    }
}
