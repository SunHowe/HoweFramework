using System.Collections.Generic;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树图命令管理器
    /// </summary>
    public class BehaviorGraphCommandManager
    {
        /// <summary>
        /// 命令历史栈
        /// </summary>
        private Stack<IBehaviorGraphCommand> m_UndoStack = new Stack<IBehaviorGraphCommand>();
        
        /// <summary>
        /// 重做命令栈
        /// </summary>
        private Stack<IBehaviorGraphCommand> m_RedoStack = new Stack<IBehaviorGraphCommand>();
        
        /// <summary>
        /// 最大历史记录数量
        /// </summary>
        private const int MAX_HISTORY_COUNT = 50;
        
        /// <summary>
        /// 命令执行事件
        /// </summary>
        public System.Action<IBehaviorGraphCommand> OnCommandExecuted;
        
        /// <summary>
        /// 命令撤销事件
        /// </summary>
        public System.Action<IBehaviorGraphCommand> OnCommandUndone;
        
        /// <summary>
        /// 命令重做事件
        /// </summary>
        public System.Action<IBehaviorGraphCommand> OnCommandRedone;
        
        /// <summary>
        /// 是否可以撤销
        /// </summary>
        public bool CanUndo => m_UndoStack.Count > 0;
        
        /// <summary>
        /// 是否可以重做
        /// </summary>
        public bool CanRedo => m_RedoStack.Count > 0;
        
        /// <summary>
        /// 执行命令并添加到历史记录
        /// </summary>
        /// <param name="command">命令</param>
        public void ExecuteCommand(IBehaviorGraphCommand command)
        {
            // 执行命令
            command.Execute();
            
            // 添加到撤销栈
            m_UndoStack.Push(command);
            
            // 清空重做栈（执行新命令后，之前的重做历史失效）
            m_RedoStack.Clear();
            
            // 限制历史记录数量
            while (m_UndoStack.Count > MAX_HISTORY_COUNT)
            {
                var oldCommands = new IBehaviorGraphCommand[m_UndoStack.Count];
                m_UndoStack.CopyTo(oldCommands, 0);
                m_UndoStack.Clear();
                
                for (int i = 1; i < oldCommands.Length; i++)
                {
                    m_UndoStack.Push(oldCommands[i]);
                }
            }
            
            // 触发命令执行事件
            OnCommandExecuted?.Invoke(command);
        }
        
        /// <summary>
        /// 撤销上一个命令
        /// </summary>
        public void Undo()
        {
            if (!CanUndo)
                return;
                
            var command = m_UndoStack.Pop();
            command.Undo();
            m_RedoStack.Push(command);
            
            // 触发命令撤销事件
            OnCommandUndone?.Invoke(command);
        }
        
        /// <summary>
        /// 重做下一个命令
        /// </summary>
        public void Redo()
        {
            if (!CanRedo)
                return;
                
            var command = m_RedoStack.Pop();
            command.Execute();
            m_UndoStack.Push(command);
            
            // 触发命令重做事件
            OnCommandRedone?.Invoke(command);
        }
        
        /// <summary>
        /// 清空历史记录
        /// </summary>
        public void Clear()
        {
            m_UndoStack.Clear();
            m_RedoStack.Clear();
        }
        
        /// <summary>
        /// 获取撤销命令描述
        /// </summary>
        /// <returns>命令描述</returns>
        public string GetUndoDescription()
        {
            return CanUndo ? m_UndoStack.Peek().Description : "";
        }
        
        /// <summary>
        /// 获取重做命令描述
        /// </summary>
        /// <returns>命令描述</returns>
        public string GetRedoDescription()
        {
            return CanRedo ? m_RedoStack.Peek().Description : "";
        }
    }
} 