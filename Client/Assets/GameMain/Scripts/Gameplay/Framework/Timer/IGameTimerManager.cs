using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏定时器管理器。
    /// </summary>
    [GameManager(GameManagerType.Timer)]
    public interface IGameTimerManager : IGameManager, ITimerCore
    {
    }
}