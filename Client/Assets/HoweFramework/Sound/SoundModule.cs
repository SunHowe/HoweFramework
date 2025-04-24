namespace HoweFramework
{
    /// <summary>
    /// 声音模块。
    /// </summary>
    public sealed class SoundModule : ModuleBase<SoundModule>
    {
        private ISoundHelper m_SoundHelper;
        private IResLoader m_ResLoader;
        private float m_Volume;

        public float Volume
        {
            get => m_Volume;
            set
            {
                if (m_Volume == value)
                {
                    return;
                }

                m_Volume = value;
                SettingModule.Instance.SetFloat(FrameworkSettings.SoundVolume, m_Volume);

                if (m_SoundHelper != null)
                {
                    m_SoundHelper.SetVolume(m_Volume);
                }
            }
        }

        /// <summary>
        /// 设置声音辅助器。
        /// </summary>
        /// <param name="soundHelper">声音辅助器。</param>
        public void SetSoundHelper(ISoundHelper soundHelper)
        {
            m_SoundHelper = soundHelper;
            m_SoundHelper.SetResLoader(m_ResLoader);
            m_SoundHelper.SetVolume(m_Volume);
        }

        /// <summary>
        /// 创建声音组。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        /// <param name="groupName">声音组名称。</param>
        /// <param name="volume">声音组音量。实际音量=全局音量*声音组音量*播放时指定的音量。</param>
        /// <param name="soundLimit">声音组中声音数量限制。超过限制时，新的声音将覆盖最旧的声音。</param>
        public void CreateSoundGroup(int groupId, string groupName, float volume = SoundConstant.DefaultVolume, int soundLimit = SoundConstant.DefaultSoundLimit)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            m_SoundHelper.CreateSoundGroup(groupId, groupName, volume, soundLimit);
        }

        /// <summary>
        /// 销毁声音组。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        public void DestroySoundGroup(int groupId)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            m_SoundHelper.DestroySoundGroup(groupId);
        }

        /// <summary>
        /// 播放声音。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        /// <param name="soundAssetName">声音资源名称。</param>
        /// <param name="playSoundParams">播放声音参数。</param>
        /// <returns>声音编号。</returns>
        public int PlaySound(int groupId, string soundAssetName, PlaySoundParams playSoundParams)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            return m_SoundHelper.PlaySound(groupId, soundAssetName, playSoundParams);
        }

        /// <summary>
        /// 停止声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        public void StopSound(int serialId)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            m_SoundHelper.StopSound(serialId);
        }

        /// <summary>
        /// 停止声音。
        /// </summary>
        /// <param name="soundAssetName">声音资源名称。</param>
        /// <param name="groupId">声音组编号。若组件编号为0，则停止所有声音组中的声音。</param>
        public void StopSound(string soundAssetName, int groupId = 0)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            m_SoundHelper.StopSound(soundAssetName, groupId);
        }

        /// <summary>
        /// 停止所有声音。
        /// </summary>
        /// <param name="groupId">声音组编号。若组件编号为0，则停止所有声音组中的声音。</param>
        public void StopAllSounds(int groupId = 0)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            m_SoundHelper.StopAllSounds(groupId);
        }

        /// <summary>
        /// 暂停声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        public void PauseSound(int serialId)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            m_SoundHelper.PauseSound(serialId);
        }

        /// <summary>
        /// 恢复声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        public void ResumeSound(int serialId)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            m_SoundHelper.ResumeSound(serialId);
        }

        /// <summary>
        /// 是否正在加载声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        /// <returns>是否正在加载声音。</returns>
        public bool IsLoadingSound(int serialId)
        {
            if (m_SoundHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "SoundHelper is not set.");
            }

            return m_SoundHelper.IsLoadingSound(serialId);
        }

        protected override void OnInit()
        {
            m_ResLoader = ResModule.Instance.CreateResLoader();
            m_Volume = SettingModule.Instance.GetFloat(FrameworkSettings.SoundVolume, SoundConstant.DefaultVolume);
        }

        protected override void OnDestroy()
        {
            m_SoundHelper?.Dispose();
            m_SoundHelper = null;
            m_ResLoader.Dispose();
            m_ResLoader = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
