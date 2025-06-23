using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树图视图
    /// </summary>
    public class BehaviorGraphView : GraphView
    {
        /// <summary>
        /// 关联的行为树图数据（工作副本）
        /// </summary>
        public BehaviorGraph Graph { get; private set; }

        /// <summary>
        /// 原始行为树图数据
        /// </summary>
        public BehaviorGraph OriginalGraph { get; private set; }

        /// <summary>
        /// 节点搜索窗口
        /// </summary>
        private BehaviorNodeSearchWindow m_SearchWindow;

        /// <summary>
        /// 节点视图字典（ID -> 节点视图）
        /// </summary>
        private Dictionary<string, BehaviorGraphNode> m_NodeViews = new Dictionary<string, BehaviorGraphNode>();

        /// <summary>
        /// 节点模板管理器
        /// </summary>
        private BehaviorNodeTemplateManager m_TemplateManager;

        /// <summary>
        /// 右键菜单打开时的本地坐标
        /// </summary>
        private Vector2 m_ContextMenuPosition;

        /// <summary>
        /// 命令管理器
        /// </summary>
        private BehaviorGraphCommandManager m_CommandManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BehaviorGraphView()
        {
            Initialize();
            m_CommandManager = new BehaviorGraphCommandManager();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            try
            {
                // 初始化节点模板管理器
                m_TemplateManager = new BehaviorNodeTemplateManager();

                // 尝试设置样式表
                var styleSheet = Resources.Load<StyleSheet>("BehaviorGraphView");
                if (styleSheet != null)
                {
                    styleSheets.Add(styleSheet);
                }

                // 设置操作器
                this.AddManipulator(new ContentZoomer());
                this.AddManipulator(new ContentDragger());
                this.AddManipulator(new SelectionDragger());
                this.AddManipulator(new RectangleSelector());

                // 添加网格背景
                var grid = new GridBackground();
                Insert(0, grid);
                grid.StretchToParentSize();



                // 设置节点创建回调
                nodeCreationRequest = OnNodeCreationRequest;

                // 设置连接兼容性回调
                graphViewChanged = OnGraphViewChanged;

                // 注册键盘事件
                RegisterCallback<KeyDownEvent>(OnKeyDown);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"初始化BehaviorGraphView失败: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 设置行为树图数据
        /// </summary>
        /// <param name="graph">行为树图数据</param>
        public void SetGraph(BehaviorGraph graph)
        {
            OriginalGraph = graph;
            
            // 创建工作副本，避免直接修改原始数据
            if (graph != null)
            {
                Graph = CreateWorkingCopy(graph);
            }
            else
            {
                Graph = null;
            }
            
            PopulateView();
        }

        /// <summary>
        /// 创建工作副本
        /// </summary>
        /// <param name="original">原始图</param>
        /// <returns>工作副本</returns>
        private BehaviorGraph CreateWorkingCopy(BehaviorGraph original)
        {
            var workingCopy = ScriptableObject.CreateInstance<BehaviorGraph>();
            
            // 复制基本属性
            workingCopy.GraphId = original.GraphId;
            workingCopy.GraphName = original.GraphName;
            workingCopy.Description = original.Description;
            workingCopy.GraphOffset = original.GraphOffset;
            workingCopy.GraphScale = original.GraphScale;
            workingCopy.RootNodeId = original.RootNodeId;

            // 深度复制节点，保持ID一致
            foreach (var originalNode in original.Nodes)
            {
                var workingNode = new BehaviorNode(originalNode.Id, originalNode.Name, originalNode.TypeName)
                {
                    NodeType = originalNode.NodeType,
                    SupportChildren = originalNode.SupportChildren,
                    MaxChildrenCount = originalNode.MaxChildrenCount,
                    GraphPosition = originalNode.GraphPosition,
                    ParentId = originalNode.ParentId
                };

                // 复制属性
                foreach (var property in originalNode.Properties)
                {
                    workingNode.Properties.Add(property.Clone());
                }

                // 复制子节点ID列表
                workingNode.ChildrenIds.AddRange(originalNode.ChildrenIds);

                workingCopy.AddNode(workingNode);
            }

            return workingCopy;
        }

        /// <summary>
        /// 填充视图
        /// </summary>
        private void PopulateView()
        {
            if (Graph == null)
                return;

            // 清空现有视图
            ClearView();

            // 创建节点视图
            foreach (var node in Graph.Nodes)
            {
                CreateNodeView(node);
            }

            // 创建连接
            CreateEdges();

            // 刷新根节点显示状态
            RefreshRootNodeDisplay();

            // 刷新所有节点的错误状态
            RefreshAllNodesErrorState();

            // 设置视图变换
            UpdateViewTransform();
        }

        /// <summary>
        /// 清空视图
        /// </summary>
        private void ClearView()
        {
            graphViewChanged = null;
            DeleteElements(graphElements.ToList());
            graphViewChanged = OnGraphViewChanged;
            m_NodeViews.Clear();
        }

        /// <summary>
        /// 创建节点视图
        /// </summary>
        /// <param name="dataNode">数据节点</param>
        /// <returns>节点视图</returns>
        private BehaviorGraphNode CreateNodeView(BehaviorNode dataNode)
        {
            var nodeView = new BehaviorGraphNode(dataNode);
            
            // 设置事件委托
            nodeView.OnNodeEvent = OnNodeEvent;
            
            AddElement(nodeView);
            m_NodeViews[dataNode.Id] = nodeView;
            return nodeView;
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        private void CreateEdges()
        {
            foreach (var nodeView in m_NodeViews.Values)
            {
                var dataNode = nodeView.DataNode;

                // 为每个子节点创建连接
                foreach (var childId in dataNode.ChildrenIds)
                {
                    if (m_NodeViews.TryGetValue(childId, out var childView))
                    {
                        var edge = nodeView.OutputPort.ConnectTo(childView.InputPort);
                        AddElement(edge);
                    }
                }
            }
        }

        /// <summary>
        /// 刷新边显示
        /// </summary>
        private void RefreshEdges()
        {
            // 移除所有现有的边
            var existingEdges = edges.ToList();
            foreach (var edge in existingEdges)
            {
                RemoveElement(edge);
            }
            
            // 重新创建边
            CreateEdges();
        }

        /// <summary>
        /// 更新视图变换
        /// </summary>
        private void UpdateViewTransform()
        {
            if (Graph == null)
                return;

            viewTransform.position = Graph.GraphOffset;
            viewTransform.scale = Graph.GraphScale;
        }

        /// <summary>
        /// 节点创建请求处理
        /// </summary>
        /// <param name="context">上下文</param>
        private void OnNodeCreationRequest(NodeCreationContext context)
        {
            if (m_SearchWindow == null)
            {
                m_SearchWindow = ScriptableObject.CreateInstance<BehaviorNodeSearchWindow>();
                m_SearchWindow.Initialize(this, m_TemplateManager);
            }

            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_SearchWindow);
        }

        /// <summary>
        /// 图视图变化处理
        /// </summary>
        /// <param name="changes">变化信息</param>
        /// <returns>变化结果</returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
        {
            if (Graph == null)
                return changes;

            // 处理删除的元素
            if (changes.elementsToRemove != null)
            {
                var nodesToRemove = new List<BehaviorNode>();
                
                foreach (var element in changes.elementsToRemove)
                {
                    if (element is BehaviorGraphNode nodeView)
                    {
                        nodesToRemove.Add(nodeView.DataNode);
                        // 从视图字典中移除
                        m_NodeViews.Remove(nodeView.DataNode.Id);
                    }
                    else if (element is Edge edge)
                    {
                        RemoveEdgeFromGraph(edge);
                    }
                }
                
                // 使用命令系统删除节点
                if (nodesToRemove.Count > 0)
                {
                    IBehaviorGraphCommand command;
                    if (nodesToRemove.Count == 1)
                    {
                        command = new RemoveNodeCommand(Graph, nodesToRemove[0]);
                    }
                    else
                    {
                        command = new RemoveMultipleNodesCommand(Graph, nodesToRemove);
                    }
                    ExecuteCommand(command);
                    
                    // 删除节点后，需要刷新视图以清理相关的边
                    RefreshEdges();
                    
                    // 刷新所有节点的错误状态
                    RefreshAllNodesErrorState();
                }
            }

            // 处理创建的边
            if (changes.edgesToCreate != null)
            {
                foreach (var edge in changes.edgesToCreate)
                {
                    CreateEdgeInGraph(edge);
                }
            }

            // 注意：现在操作的是工作副本，不自动设置dirty
            // 只有在保存时才应用更改到原始图

            return changes;
        }



        /// <summary>
        /// 从图中移除边
        /// </summary>
        /// <param name="edge">边</param>
        private void RemoveEdgeFromGraph(Edge edge)
        {
            var outputNode = edge.output.node as BehaviorGraphNode;
            var inputNode = edge.input.node as BehaviorGraphNode;

            if (outputNode != null && inputNode != null)
            {
                // 使用命令系统断开连接
                var command = new DisconnectNodesCommand(Graph, outputNode.DataNode.Id, inputNode.DataNode.Id);
                ExecuteCommand(command);
                
                // 刷新父节点的错误状态
                outputNode.RefreshNode();
            }
        }

        /// <summary>
        /// 在图中创建边
        /// </summary>
        /// <param name="edge">边</param>
        private void CreateEdgeInGraph(Edge edge)
        {
            var outputNode = edge.output.node as BehaviorGraphNode;
            var inputNode = edge.input.node as BehaviorGraphNode;

            if (outputNode != null && inputNode != null)
            {
                // 使用命令系统连接节点
                var command = new ConnectNodesCommand(Graph, outputNode.DataNode.Id, inputNode.DataNode.Id);
                ExecuteCommand(command);
                
                // 刷新父节点的错误状态
                outputNode.RefreshNode();
            }
        }

        /// <summary>
        /// 节点事件处理
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="node">节点</param>
        private void OnNodeEvent(BehaviorGraphEventType eventType, BehaviorGraphNode node)
        {
            switch (eventType)
            {
                case BehaviorGraphEventType.SetRootNode:
                    SetRootNode(node);
                    break;
                case BehaviorGraphEventType.DuplicateNode:
                    DuplicateNode(node);
                    break;
                case BehaviorGraphEventType.DeleteNode:
                    DeleteNode(node);
                    break;
                case BehaviorGraphEventType.DeleteSelectedNodes:
                    DeleteSelectedNodes();
                    break;
            }
        }

        /// <summary>
        /// 设置根节点
        /// </summary>
        /// <param name="nodeView">节点视图</param>
        private void SetRootNode(BehaviorGraphNode nodeView)
        {
            if (Graph != null)
            {
                // 使用命令系统设置根节点
                var command = new SetRootNodeCommand(Graph, nodeView.DataNode.Id);
                ExecuteCommand(command);
                
                EditorUtility.SetDirty(Graph);

                // 刷新视图显示根节点状态
                RefreshRootNodeDisplay();
            }
        }

        /// <summary>
        /// 复制节点
        /// </summary>
        /// <param name="nodeView">节点视图</param>
        private void DuplicateNode(BehaviorGraphNode nodeView)
        {
            if (Graph != null)
            {
                // 不允许复制Root节点
                if (Graph.IsRootNode(nodeView.DataNode.Id))
                    return;
                
                // 使用命令系统复制节点
                var command = new DuplicateNodeCommand(Graph, nodeView.DataNode);
                ExecuteCommand(command);
                
                // 获取复制后的节点并创建视图
                var allNodes = Graph.Nodes;
                var duplicatedNode = allNodes.LastOrDefault(n => n.Name == nodeView.DataNode.Name && n.Id != nodeView.DataNode.Id);
                if (duplicatedNode != null)
                {
                    var newNodeView = CreateNodeView(duplicatedNode);
                    
                    // 刷新新节点的错误状态
                    newNodeView.RefreshNode();
                    
                    // 选中新创建的节点
                    ClearSelection();
                    AddToSelection(newNodeView);
                }

                EditorUtility.SetDirty(Graph);
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="nodeView">节点视图</param>
        private void DeleteNode(BehaviorGraphNode nodeView)
        {
            if (Graph == null)
                return;
                
            // 不允许删除Root节点
            if (Graph.IsRootNode(nodeView.DataNode.Id))
            {
                UnityEngine.Debug.LogWarning("Root节点不允许删除");
                return;
            }
            
            DeleteElements(new[] { nodeView });
        }

        /// <summary>
        /// 删除所有选中的节点
        /// </summary>
        private void DeleteSelectedNodes()
        {
            if (Graph == null)
                return;
                
            var selectedNodes = selection.OfType<BehaviorGraphNode>().ToList();
            if (selectedNodes.Count == 0)
                return;
                
            // 过滤掉Root节点
            var nodesToDelete = selectedNodes.Where(node => !Graph.IsRootNode(node.DataNode.Id)).ToList();
            
            // 如果包含Root节点，给出提示
            if (nodesToDelete.Count < selectedNodes.Count)
            {
                var rootNodesCount = selectedNodes.Count - nodesToDelete.Count;
                UnityEngine.Debug.LogWarning($"已过滤掉 {rootNodesCount} 个Root节点，Root节点不允许删除");
            }
            
            // 删除可删除的节点
            if (nodesToDelete.Count > 0)
            {
                DeleteElements(nodesToDelete);
            }
        }

        /// <summary>
        /// 键盘事件处理
        /// </summary>
        /// <param name="evt">键盘事件</param>
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace)
            {
                DeleteSelectedNodes();
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// 刷新根节点显示
        /// </summary>
        private void RefreshRootNodeDisplay()
        {
            if (Graph == null)
                return;

            foreach (var nodeView in m_NodeViews.Values)
            {
                var isRoot = nodeView.DataNode.Id == Graph.RootNodeId;
                nodeView.SetAsRoot(isRoot);
            }
        }

        /// <summary>
        /// 刷新所有节点的错误状态
        /// </summary>
        private void RefreshAllNodesErrorState()
        {
            if (Graph == null)
                return;

            foreach (var nodeView in m_NodeViews.Values)
            {
                nodeView.RefreshNode();
            }
        }

        /// <summary>
        /// 通过模板创建节点
        /// </summary>
        /// <param name="template">节点模板</param>
        /// <param name="position">位置（如果为Vector2.zero则使用右键菜单位置）</param>
        public void CreateNodeFromTemplate(BehaviorNodeTemplate template, Vector2 position = default)
        {
            if (Graph == null)
                return;

            var dataNode = template.CreateNode();
            
            // 如果没有指定位置，使用右键菜单打开时的位置
            if (position == Vector2.zero)
            {
                dataNode.GraphPosition = m_ContextMenuPosition;
            }
            else
            {
                dataNode.GraphPosition = position;
            }

            // 使用命令系统创建节点
            var command = new AddNodeCommand(Graph, dataNode);
            ExecuteCommand(command);
            
            // 创建节点视图
            CreateNodeView(dataNode);

            EditorUtility.SetDirty(Graph);
        }

        /// <summary>
        /// 将屏幕坐标转换为GraphView本地坐标
        /// </summary>
        /// <param name="screenPosition">屏幕坐标</param>
        /// <returns>GraphView本地坐标</returns>
        public Vector2 ScreenToGraphPosition(Vector2 screenPosition)
        {
            var editorWindow = EditorWindow.focusedWindow;
            if (editorWindow == null)
                return Vector2.zero;

            // 将屏幕坐标转换为窗口坐标
            var windowMousePosition = screenPosition - editorWindow.position.position;
            
            // 将窗口坐标转换为GraphView的本地坐标
            // 考虑视图变换（缩放和平移）
            var viewTransform = this.viewTransform;
            var localMousePosition = (windowMousePosition - (Vector2)viewTransform.position) / viewTransform.scale.x;
            
            return localMousePosition;
        }

        /// <summary>
        /// 获取端口兼容性
        /// </summary>
        /// <param name="startPort">起始端口</param>
        /// <param name="nodeAdapter">节点适配器</param>
        /// <returns>兼容端口列表</returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port &&
                    startPort.direction != port.direction &&
                    startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        /// <summary>
        /// 保存图状态
        /// </summary>
        public void SaveGraphState()
        {
            if (Graph == null)
                return;

            Graph.GraphOffset = viewTransform.position;
            Graph.GraphScale = viewTransform.scale;

            // 应用更改到原始图
            ApplyChangesToOriginal();
        }

        /// <summary>
        /// 应用更改到原始图
        /// </summary>
        public void ApplyChangesToOriginal()
        {
            if (OriginalGraph == null || Graph == null)
                return;

            // 清空原始图的节点
            OriginalGraph.Clear();

            // 复制工作副本的数据到原始图
            OriginalGraph.GraphName = Graph.GraphName;
            OriginalGraph.Description = Graph.Description;
            OriginalGraph.GraphOffset = Graph.GraphOffset;
            OriginalGraph.GraphScale = Graph.GraphScale;
            OriginalGraph.RootNodeId = Graph.RootNodeId;

            // 复制所有节点（ID保持一致）
            foreach (var node in Graph.Nodes)
            {
                var originalNode = new BehaviorNode(node.Id, node.Name, node.TypeName)
                {
                    NodeType = node.NodeType,
                    SupportChildren = node.SupportChildren,
                    MaxChildrenCount = node.MaxChildrenCount,
                    GraphPosition = node.GraphPosition,
                    ParentId = node.ParentId
                };

                // 复制属性
                foreach (var property in node.Properties)
                {
                    originalNode.Properties.Add(property.Clone());
                }

                // 复制子节点ID列表
                originalNode.ChildrenIds.AddRange(node.ChildrenIds);

                OriginalGraph.AddNode(originalNode);
            }

            EditorUtility.SetDirty(OriginalGraph);
        }

        /// <summary>
        /// 验证图
        /// </summary>
        /// <returns>验证结果</returns>
        public BehaviorGraphValidationResult ValidateGraph()
        {
            if (Graph == null)
            {
                var result = new BehaviorGraphValidationResult();
                result.AddError("没有加载行为树图数据");
                return result;
            }

            return Graph.ValidateGraph();
        }

        /// <summary>
        /// 构建上下文菜单
        /// </summary>
        /// <param name="evt">事件</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is BehaviorGraphView && Graph != null)
            {
                evt.menu.AppendAction("新建节点", OnContextMenuNodeCreate);
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("验证图", (a) => ShowValidationResults());
                evt.menu.AppendAction("保存状态", (a) => SaveGraphState());
            }
        }

        /// <summary>
        /// 新建节点
        /// </summary>
        private void OnContextMenuNodeCreate(DropdownMenuAction a)
        {
            // 获取当前编辑器窗口
            var editorWindow = EditorWindow.focusedWindow;
            if (editorWindow == null)
                return;
                
            // 将事件坐标转换为屏幕坐标
            var screenMousePosition = GUIUtility.GUIToScreenPoint(a.eventInfo.mousePosition);
            
            // 记录右键菜单打开时的位置（转换为GraphView本地坐标）
            m_ContextMenuPosition = ScreenToGraphPosition(screenMousePosition);
            
            OnNodeCreationRequest(new NodeCreationContext { screenMousePosition = screenMousePosition });
        }

        /// <summary>
        /// 显示验证结果
        /// </summary>
        private void ShowValidationResults()
        {
            var result = ValidateGraph();

            if (result.IsValid)
            {
                EditorUtility.DisplayDialog("验证结果", "行为树图验证通过！", "确定");
            }
            else
            {
                var message = "验证失败：\n";
                foreach (var error in result.Errors)
                {
                    message += $"• {error}\n";
                }

                if (result.Warnings.Count > 0)
                {
                    message += "\n警告：\n";
                    foreach (var warning in result.Warnings)
                    {
                        message += $"• {warning}\n";
                    }
                }

                EditorUtility.DisplayDialog("验证结果", message, "确定");
            }
        }

        /// <summary>
        /// 撤销上一个操作
        /// </summary>
        public void Undo()
        {
            if (m_CommandManager != null && m_CommandManager.CanUndo)
            {
                m_CommandManager.Undo();
                
                // 重新填充视图以反映撤销的更改
                PopulateView();
                
                // 标记图为脏
                if (Graph != null)
                {
                    EditorUtility.SetDirty(Graph);
                }
            }
        }

        /// <summary>
        /// 重做下一个操作
        /// </summary>
        public void Redo()
        {
            if (m_CommandManager != null && m_CommandManager.CanRedo)
            {
                m_CommandManager.Redo();
                
                // 重新填充视图以反映重做的更改
                PopulateView();
                
                // 标记图为脏
                if (Graph != null)
                {
                    EditorUtility.SetDirty(Graph);
                }
            }
        }

        /// <summary>
        /// 获取命令管理器
        /// </summary>
        /// <returns>命令管理器</returns>
        public BehaviorGraphCommandManager GetCommandManager()
        {
            return m_CommandManager;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">命令</param>
        private void ExecuteCommand(IBehaviorGraphCommand command)
        {
            m_CommandManager.ExecuteCommand(command);
        }

    }
}