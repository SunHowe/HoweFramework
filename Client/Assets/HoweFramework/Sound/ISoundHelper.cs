using System;

namespace HoweFramework
{
    /// <summary>
    /// 声音辅助器接口。
    /// </summary>
    public interface ISoundHelper : IDisposable
    {
        /// <summary>
        /// 设置资源加载器。
        /// </summary>
        /// <param name="resLoader">资源加载器。</param>
        void SetResLoader(IResLoader resLoader);

        /// <summary>
        /// 设置全局音量。
        /// </summary>
        /// <param name="volume">全局音量。</param>
        void SetVolume(float volume);

        /// <summary>
        /// 创建声音组。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        /// <param name="groupName">声音组名称。</param>
        /// <param name="volume">声音组音量。实际音量=全局音量*声音组音量*播放时指定的音量。</param>
        /// <param name="soundLimit">声音组中声音数量限制。超过限制时，新的声音将覆盖最旧的声音。</param>
        void CreateSoundGroup(int groupId, string groupName, float volume, int soundLimit);

        /// <summary>
        /// 销毁声音组。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        void DestroySoundGroup(int groupId);

        /// <summary>
        /// 播放声音。
        /// </summary>
        /// <param name="groupId">声音组编号。</param>
        /// <param name="soundAssetName">声音资源名称。</param>
        /// <param name="playSoundParams">播放声音参数。</param>
        /// <returns>声音编号。</returns>
        int PlaySound(int groupId, string soundAssetName, PlaySoundParams playSoundParams);

        /// <summary>
        /// 停止声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        void StopSound(int serialId);
        
        /// <summary>
        /// 停止声音。
        /// </summary>
        /// <param name="soundAssetName">声音资源名称。</param>
        /// <param name="groupId">声音组编号。若组件编号为0，则停止所有声音组中的声音。</param>
        void StopSound(string soundAssetName, int groupId);

        /// <summary>
        /// 停止所有声音。
        /// </summary>
        /// <param name="groupId">声音组编号。若组件编号为0，则停止所有声音组中的声音。</param>
        void StopAllSounds(int groupId);

        /// <summary>
        /// 暂停声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        void PauseSound(int serialId);

        /// <summary>
        /// 恢复声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        void ResumeSound(int serialId);

        /// <summary>
        /// 是否正在加载声音。
        /// </summary>
        /// <param name="serialId">声音编号。</param>
        /// <returns>是否正在加载声音。</returns>
        bool IsLoadingSound(int serialId);

        /// <summary>
        /// 更新声音辅助器。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds, float realElapseSeconds);
    }
}
