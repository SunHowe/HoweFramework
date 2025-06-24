namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树图编辑命令接口
    /// </summary>
    public interface IBehaviorGraphCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Execute();
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        void Undo();
        
        /// <summary>
        /// 命令描述
        /// </summary>
        string Description { get; }
    }
} 