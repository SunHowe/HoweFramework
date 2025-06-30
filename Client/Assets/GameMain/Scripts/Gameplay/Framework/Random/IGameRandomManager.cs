using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 随机数管理器接口。
    /// </summary>
    [GameManager(GameManagerType.Random)]
    public interface IGameRandomManager : IGameManager, IRandom
    {
        /// <summary>
        /// 设置随机数种子。
        /// </summary>
        /// <param name="seed">随机数种子。</param>
        void SetSeed(int seed);
    }
}