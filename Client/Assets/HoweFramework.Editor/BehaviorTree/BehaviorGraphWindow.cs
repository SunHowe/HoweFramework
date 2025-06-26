using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.IO;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树图编辑器窗口
    /// </summary>
    public class BehaviorGraphWindow : EditorWindow
    {
        /// <summary>
        /// 图视图
        /// </summary>
        private BehaviorGraphView m_GraphView;

        /// <summary>
        /// 工具栏
        /// </summary>
        private VisualElement m_Toolbar;

        /// <summary>
        /// 文件名标签
        /// </summary>
        private Label m_FileNameLabel;

        /// <summary>
        /// 验证按钮
        /// </summary>
        private Button m_ValidateButton;

        /// <summary>
        /// 撤销按钮
        /// </summary>
        private Button m_UndoButton;

        /// <summary>
        /// 重做按钮
        /// </summary>
        private Button m_RedoButton;

        /// <summary>
        /// 导出运行时配置按钮
        /// </summary>
        private Button m_ExportButton;

        /// <summary>
        /// 用户是否进行过实际操作
        /// </summary>
        private bool m_HasUserMadeChanges;

        /// <summary>
        /// 当前的工作副本。
        /// </summary>
        [SerializeField]
        private BehaviorGraph m_CurrentWorkingGraph;

        /// <summary>
        /// 当前的原始图。
        /// </summary>
        [SerializeField]
        private BehaviorGraph m_CurrentOriginalGraph;

        [MenuItem("Game Framework/行为树编辑器")]
        public static void OpenWindow()
        {
            var window = GetWindow<BehaviorGraphWindow>();
            window.titleContent = new GUIContent("行为树编辑器");
            window.Show();
        }

        /// <summary>
        /// 创建图形界面
        /// </summary>
        public void CreateGUI()
        {
            try
            {
                // 设置根容器为垂直布局
                rootVisualElement.style.flexDirection = FlexDirection.Column;

                // 先只创建工具栏
                ConstructToolbar();

                // 延迟创建图视图
                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        if (this != null) // 确保窗口还存在
                        {
                            ConstructGraphView();

                            if (m_CurrentWorkingGraph == null)
                            {
                                // 如果没有加载图，尝试加载或创建一个。
                                CreateNewGraph();
                            }
                            else
                            {
                                // 如果已经加载图，设置到GraphView中。
                                m_GraphView.SetGraph(m_CurrentWorkingGraph);
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"延迟创建图视图失败: {e.Message}\n{e.StackTrace}");
                    }
                };
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建行为树编辑器界面时发生错误: {e.Message}\n{e.StackTrace}");

                // 至少确保工具栏显示
                if (m_Toolbar == null)
                {
                    try
                    {
                        ConstructToolbar();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"创建工具栏也失败: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 构建工具栏
        /// </summary>
        private void ConstructToolbar()
        {
            m_Toolbar = new VisualElement();
            m_Toolbar.name = "toolbar";
            m_Toolbar.style.flexDirection = FlexDirection.Row;
            m_Toolbar.style.alignItems = Align.Center;
            m_Toolbar.style.height = 30;
            m_Toolbar.style.minHeight = 30;
            m_Toolbar.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 1f);
            m_Toolbar.style.borderBottomWidth = 1;
            m_Toolbar.style.borderBottomColor = new Color(0.13f, 0.13f, 0.13f, 1f);
            m_Toolbar.style.paddingLeft = 4;
            m_Toolbar.style.paddingRight = 4;

            // 新建按钮
            var newButton = new Button(CreateNewGraph) { text = "新建" };
            newButton.style.height = 22;
            newButton.style.marginLeft = 2;
            newButton.style.marginRight = 2;
            m_Toolbar.Add(newButton);

            // 打开按钮
            var openButton = new Button(OpenGraph) { text = "打开" };
            openButton.style.height = 22;
            openButton.style.marginLeft = 2;
            openButton.style.marginRight = 2;
            m_Toolbar.Add(openButton);

            // 保存按钮
            var saveButton = new Button(SaveGraph) { text = "保存" };
            saveButton.style.height = 22;
            saveButton.style.marginLeft = 2;
            saveButton.style.marginRight = 2;
            m_Toolbar.Add(saveButton);

            // 另存为按钮
            var saveAsButton = new Button(SaveAsGraph) { text = "另存为" };
            saveAsButton.style.height = 22;
            saveAsButton.style.marginLeft = 2;
            saveAsButton.style.marginRight = 2;
            m_Toolbar.Add(saveAsButton);

            // 导出运行时配置按钮
            m_ExportButton = new Button(ExportRuntimeConfig) { text = "导出运行时配置" };
            m_ExportButton.style.height = 22;
            m_ExportButton.style.marginLeft = 2;
            m_ExportButton.style.marginRight = 2;
            m_Toolbar.Add(m_ExportButton);

            // 分隔符
            var spacer1 = new VisualElement();
            spacer1.style.width = 1;
            spacer1.style.height = 18;
            spacer1.style.backgroundColor = new Color(0.13f, 0.13f, 0.13f, 1f);
            spacer1.style.marginLeft = 4;
            spacer1.style.marginRight = 4;
            m_Toolbar.Add(spacer1);

            // 撤销按钮
            m_UndoButton = new Button(UndoCommand) { text = "撤销" };
            m_UndoButton.style.height = 22;
            m_UndoButton.style.marginLeft = 2;
            m_UndoButton.style.marginRight = 2;
            m_UndoButton.tooltip = "撤销";
            m_Toolbar.Add(m_UndoButton);

            // 重做按钮
            m_RedoButton = new Button(RedoCommand) { text = "重做" };
            m_RedoButton.style.height = 22;
            m_RedoButton.style.marginLeft = 2;
            m_RedoButton.style.marginRight = 2;
            m_RedoButton.tooltip = "重做";
            m_Toolbar.Add(m_RedoButton);

            // 分隔符
            var spacer1_5 = new VisualElement();
            spacer1_5.style.width = 1;
            spacer1_5.style.height = 18;
            spacer1_5.style.backgroundColor = new Color(0.13f, 0.13f, 0.13f, 1f);
            spacer1_5.style.marginLeft = 4;
            spacer1_5.style.marginRight = 4;
            m_Toolbar.Add(spacer1_5);

            // 验证按钮
            m_ValidateButton = new Button(ValidateGraph) { text = "验证" };
            m_ValidateButton.style.height = 22;
            m_ValidateButton.style.marginLeft = 2;
            m_ValidateButton.style.marginRight = 2;
            m_Toolbar.Add(m_ValidateButton);

            // 分隔符
            var spacer2 = new VisualElement();
            spacer2.style.width = 1;
            spacer2.style.height = 18;
            spacer2.style.backgroundColor = new Color(0.13f, 0.13f, 0.13f, 1f);
            spacer2.style.marginLeft = 4;
            spacer2.style.marginRight = 4;
            m_Toolbar.Add(spacer2);

            // 文件名标签
            m_FileNameLabel = new Label("未保存的行为树");
            m_FileNameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            m_FileNameLabel.style.color = Color.white;
            m_FileNameLabel.style.marginLeft = 8;
            m_FileNameLabel.style.fontSize = 12;
            m_Toolbar.Add(m_FileNameLabel);

            // 弹性空间
            var flexibleSpace = new VisualElement();
            flexibleSpace.style.flexGrow = 1;
            m_Toolbar.Add(flexibleSpace);

            rootVisualElement.Add(m_Toolbar);
        }

        /// <summary>
        /// 构建图视图
        /// </summary>
        private void ConstructGraphView()
        {
            try
            {
                m_GraphView = new BehaviorGraphView
                {
                    name = "Behavior Graph"
                };

                // 不要使用StretchToParentSize，而是使用flexGrow占用剩余空间
                m_GraphView.style.flexGrow = 1;
                m_GraphView.style.width = Length.Percent(100);
                m_GraphView.style.height = Length.Auto();

                rootVisualElement.Add(m_GraphView);

                // 订阅命令管理器的事件
                var commandManager = m_GraphView.GetCommandManager();
                if (commandManager != null)
                {
                    commandManager.OnCommandExecuted += OnCommandManagerEvent;
                    commandManager.OnCommandUndone += OnCommandManagerEvent;
                    commandManager.OnCommandRedone += OnCommandManagerEvent;
                }

                // 更新撤销重做按钮状态
                UpdateUndoRedoButtonTooltips();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建图视图失败: {e.Message}\n{e.StackTrace}");

                // 创建一个占位的视图
                var placeholder = new VisualElement();
                placeholder.style.flexGrow = 1;
                placeholder.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

                var label = new Label("图视图创建失败，请查看控制台");
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                label.style.color = Color.white;
                placeholder.Add(label);

                rootVisualElement.Add(placeholder);
            }
        }

        /// <summary>
        /// 创建新图
        /// </summary>
        private void CreateNewGraph()
        {
            try
            {
                if (ShouldSaveChanges())
                {
                    SaveGraph();
                }

                var graph = CreateInstance<BehaviorGraph>();
                graph.GraphName = BehaviorEditorSettings.DEFAULT_GRAPH_NAME;

                m_CurrentOriginalGraph = graph;
                m_CurrentWorkingGraph = CreateWorkingCopy(graph);

                m_GraphView?.SetGraph(m_CurrentWorkingGraph);
                UpdateFileNameLabel("新行为树*");

                // 重置用户操作标记
                m_HasUserMadeChanges = false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建新行为树失败: {e.Message}\n{e.StackTrace}");

                // 确保至少有一个空图
                try
                {
                    var graph = CreateInstance<BehaviorGraph>();
                    graph.GraphName = BehaviorEditorSettings.DEFAULT_GRAPH_NAME;

                    m_CurrentOriginalGraph = graph;
                    m_CurrentWorkingGraph = CreateWorkingCopy(graph);

                    m_GraphView?.SetGraph(m_CurrentWorkingGraph);
                    UpdateFileNameLabel("空行为树*");

                    // 重置用户操作标记
                    m_HasUserMadeChanges = false;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"创建空行为树也失败: {ex.Message}");
                    UpdateFileNameLabel("创建失败");
                }
            }
        }

        /// <summary>
        /// 打开图
        /// </summary>
        private void OpenGraph()
        {
            if (ShouldSaveChanges())
            {
                SaveGraph();
            }

            var path = EditorUtility.OpenFilePanel("打开行为树", BehaviorEditorSettings.DefaultDirectory, "asset");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            // 转换为相对路径
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            var graph = AssetDatabase.LoadAssetAtPath<BehaviorGraph>(path);
            if (graph != null)
            {
                // 设置当前原始图和当前工作副本。
                m_CurrentOriginalGraph = graph;
                m_CurrentWorkingGraph = CreateWorkingCopy(graph);

                m_GraphView.SetGraph(m_CurrentWorkingGraph);

                UpdateFileNameLabel(graph.name);

                // 更新目录设置
                BehaviorEditorSettings.UpdateDirectory(path);

                // 重置用户操作标记（打开文件时重置）
                m_HasUserMadeChanges = false;
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "无法加载行为树文件", "确定");
            }
        }

        /// <summary>
        /// 保存图
        /// </summary>
        private void SaveGraph()
        {
            if (m_CurrentWorkingGraph == null)
            {
                return;
            }

            m_GraphView?.SaveGraphState();

            var path = m_CurrentOriginalGraph != null ? AssetDatabase.GetAssetPath(m_CurrentOriginalGraph) : null;
            if (string.IsNullOrEmpty(path))
            {
                SaveAsGraph();
                return;
            }

            // 应用工作副本的更改到原始图
            ApplyChangesToOriginal();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UpdateFileNameLabel(m_CurrentWorkingGraph.name);

            // 保存后重置标记（已保存，无需再提示）
            m_HasUserMadeChanges = false;
        }

        /// <summary>
        /// 另存为图
        /// </summary>
        private void SaveAsGraph()
        {
            if (m_CurrentWorkingGraph == null)
            {
                return;
            }

            m_GraphView?.SaveGraphState();

            var path = EditorUtility.SaveFilePanel("保存行为树", BehaviorEditorSettings.DefaultDirectory, m_CurrentWorkingGraph.GraphName ?? BehaviorEditorSettings.DefaultFileName, "asset");
            if (string.IsNullOrEmpty(path))
                return;

            // 转换为相对路径
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            // 使用文件名作为图名称。
            m_GraphView.Graph.GraphName = Path.GetFileNameWithoutExtension(path);

            // 如果是新图或者路径改变了，创建新资产
            var existingPath = m_CurrentOriginalGraph != null ? AssetDatabase.GetAssetPath(m_CurrentOriginalGraph) : null;
            if (string.IsNullOrEmpty(existingPath) || existingPath != path)
            {
                AssetDatabase.CreateAsset(m_CurrentWorkingGraph, path);

                // 更新索引的原始图。
                m_CurrentOriginalGraph = m_CurrentWorkingGraph;
                m_CurrentWorkingGraph = CreateWorkingCopy(m_CurrentOriginalGraph);

                m_GraphView.SetGraph(m_CurrentWorkingGraph);
            }
            else
            {
                // 应用工作副本的更改到原始图
                ApplyChangesToOriginal();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UpdateFileNameLabel(m_CurrentWorkingGraph.name);

            // 更新目录设置
            BehaviorEditorSettings.UpdateDirectory(path);

            // 保存后重置标记（已保存，无需再提示）
            m_HasUserMadeChanges = false;
        }

        /// <summary>
        /// 验证图
        /// </summary>
        private void ValidateGraph()
        {
            if (m_GraphView == null)
                return;

            var result = m_GraphView.ValidateGraph();

            if (result.IsValid)
            {
                EditorUtility.DisplayDialog("验证结果", "行为树验证通过！", "确定");
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
        /// 更新文件名标签
        /// </summary>
        /// <param name="fileName">文件名</param>
        private void UpdateFileNameLabel(string fileName)
        {
            if (m_FileNameLabel != null)
            {
                m_FileNameLabel.text = fileName;
            }
        }

        /// <summary>
        /// 是否应该保存更改
        /// </summary>
        /// <returns>是否保存</returns>
        private bool ShouldSaveChanges()
        {
            if (m_GraphView?.Graph == null)
                return false;

            // 如果用户未进行过实际操作，不需要提示保存
            if (!m_HasUserMadeChanges)
                return false;

            if (EditorUtility.IsDirty(m_GraphView.Graph))
            {
                var result = EditorUtility.DisplayDialogComplex(
                    "保存更改",
                    $"行为树 '{m_GraphView.Graph.GraphName}' 已修改，是否保存更改？",
                    "保存", "不保存", "取消"
                );

                switch (result)
                {
                    case 0: // 保存
                        return true;
                    case 1: // 不保存
                        return false;
                    case 2: // 取消
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 窗口失去焦点时
        /// </summary>
        private void OnLostFocus()
        {
            // 只保存视图状态（如位置、缩放），不保存到资产文件
            if (m_GraphView != null && m_GraphView.Graph != null)
            {
                m_GraphView.Graph.GraphOffset = m_GraphView.viewTransform.position;
                m_GraphView.Graph.GraphScale = m_GraphView.viewTransform.scale;
            }
        }

        /// <summary>
        /// 命令管理器事件处理
        /// </summary>
        /// <param name="command">命令</param>
        private void OnCommandManagerEvent(IBehaviorGraphCommand command)
        {
            // 用户进行了操作，标记已做更改
            m_HasUserMadeChanges = true;

            UpdateUndoRedoButtonTooltips();
        }

        /// <summary>
        /// 窗口销毁时
        /// </summary>
        private void OnDestroy()
        {
            // 取消订阅事件
            if (m_GraphView != null)
            {
                var commandManager = m_GraphView.GetCommandManager();
                if (commandManager != null)
                {
                    commandManager.OnCommandExecuted -= OnCommandManagerEvent;
                    commandManager.OnCommandUndone -= OnCommandManagerEvent;
                    commandManager.OnCommandRedone -= OnCommandManagerEvent;
                }

                // 只保存视图状态，不保存到资产文件
                if (m_GraphView.Graph != null)
                {
                    m_GraphView.Graph.GraphOffset = m_GraphView.viewTransform.position;
                    m_GraphView.Graph.GraphScale = m_GraphView.viewTransform.scale;
                }
            }
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        private void UndoCommand()
        {
            if (m_GraphView != null)
            {
                m_GraphView.Undo();
            }
        }

        /// <summary>
        /// 重做命令
        /// </summary>
        private void RedoCommand()
        {
            if (m_GraphView != null)
            {
                m_GraphView.Redo();
            }
        }

        /// <summary>
        /// 更新撤销重做按钮的提示信息
        /// </summary>
        private void UpdateUndoRedoButtonTooltips()
        {
            if (m_GraphView == null || m_UndoButton == null || m_RedoButton == null)
                return;

            var commandManager = m_GraphView.GetCommandManager();
            if (commandManager == null)
                return;

            // 更新撤销按钮提示
            if (commandManager.CanUndo)
            {
                var undoDescription = commandManager.GetUndoDescription();
                m_UndoButton.tooltip = $"撤销: {undoDescription}";
                m_UndoButton.SetEnabled(true);
            }
            else
            {
                m_UndoButton.tooltip = "撤销";
                m_UndoButton.SetEnabled(false);
            }

            // 更新重做按钮提示
            if (commandManager.CanRedo)
            {
                var redoDescription = commandManager.GetRedoDescription();
                m_RedoButton.tooltip = $"重做: {redoDescription}";
                m_RedoButton.SetEnabled(true);
            }
            else
            {
                m_RedoButton.tooltip = "重做";
                m_RedoButton.SetEnabled(false);
            }
        }

        /// <summary>
        /// 导出运行时配置为二进制.bytes文件
        /// </summary>
        private void ExportRuntimeConfig()
        {
            if (m_GraphView?.Graph == null)
            {
                EditorUtility.DisplayDialog("错误", "未加载任何行为树，无法导出。", "确定");
                return;
            }
            try
            {
                // 生成运行时配置
                var runtimeConfig = m_GraphView.Graph.ToRuntimeConfig();
                if (runtimeConfig == null)
                {
                    EditorUtility.DisplayDialog("错误", "生成运行时配置失败。", "确定");
                    return;
                }

                // 选择保存路径
                string defaultName = m_GraphView.Graph.GraphName;
                if (string.IsNullOrEmpty(defaultName))
                {
                    defaultName = "BehaviorTree";
                }

                string path = EditorUtility.SaveFilePanel(
                    "导出行为树运行时配置",
                    BehaviorEditorSettings.DefaultRuntimeConfigDirectory,
                    defaultName,
                    "bytes");

                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                // 序列化为二进制
                using var writer = DefaultBufferWriter.Create();
                runtimeConfig.Serialize(writer);

                // 获取有效字节
                byte[] data = writer.WrittenBuffer.ToArray();

                // 写入文件
                File.WriteAllBytes(path, data);
                Debug.Log($"行为树运行时配置已导出到：\n{path}");

                // 更新默认目录。
                BehaviorEditorSettings.UpdateRuntimeConfigDirectory(path);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"导出运行时配置失败：{ex.Message}");
            }
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
        /// 应用更改到原始图
        /// </summary>
        public void ApplyChangesToOriginal()
        {
            if (m_CurrentOriginalGraph == null || m_CurrentWorkingGraph == null)
                return;

            // 清空原始图的节点
            m_CurrentOriginalGraph.Clear();

            // 复制工作副本的数据到原始图
            m_CurrentOriginalGraph.GraphName = m_CurrentWorkingGraph.GraphName;
            m_CurrentOriginalGraph.Description = m_CurrentWorkingGraph.Description;
            m_CurrentOriginalGraph.GraphOffset = m_CurrentWorkingGraph.GraphOffset;
            m_CurrentOriginalGraph.GraphScale = m_CurrentWorkingGraph.GraphScale;
            m_CurrentOriginalGraph.RootNodeId = m_CurrentWorkingGraph.RootNodeId;

            // 复制所有节点（ID保持一致）
            foreach (var node in m_CurrentWorkingGraph.Nodes)
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

                m_CurrentOriginalGraph.AddNode(originalNode);
            }

            EditorUtility.SetDirty(m_CurrentOriginalGraph);
        }
    }
}