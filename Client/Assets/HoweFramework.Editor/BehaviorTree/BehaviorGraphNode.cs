using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树图节点视图
    /// </summary>
    public class BehaviorGraphNode : Node
    {
        /// <summary>
        /// 关联的数据节点
        /// </summary>
        public BehaviorNode DataNode { get; private set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public BehaviorNodeType NodeType { get; private set; }

        /// <summary>
        /// 输入端口
        /// </summary>
        public Port InputPort { get; private set; }

        /// <summary>
        /// 输出端口
        /// </summary>
        public Port OutputPort { get; private set; }

        /// <summary>
        /// 属性容器
        /// </summary>
        private VisualElement m_PropertiesContainer;

        /// <summary>
        /// 错误指示器
        /// </summary>
        private VisualElement m_ErrorIndicator;

        /// <summary>
        /// 节点事件委托
        /// </summary>
        public System.Action<BehaviorGraphEventType, BehaviorGraphNode> OnNodeEvent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataNode">数据节点</param>
        public BehaviorGraphNode(BehaviorNode dataNode)
        {
            DataNode = dataNode;
            NodeType = dataNode.NodeType;
            
            Initialize();
            UpdateNodeAppearance();
            CreatePorts();
            CreatePropertiesUI();
            
            RefreshExpandedState();
            RefreshPorts();
        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        private void Initialize()
        {
            title = DataNode.Name;
            name = DataNode.Id;
            
            // 设置节点位置
            SetPosition(new Rect(DataNode.GraphPosition, Vector2.zero));
            
            // 添加样式类
            AddToClassList("behavior-node");
            AddToClassList($"behavior-node--{NodeType.ToString().ToLower()}");
            
            // 使标题可编辑
            MakeTitleEditable();
        }

        /// <summary>
        /// 更新节点外观
        /// </summary>
        private void UpdateNodeAppearance()
        {
            // 检查是否为Root节点
            bool isRootNode = DataNode.NodeType == BehaviorNodeType.Root;
            
            // 根据节点类型设置颜色
            var color = GetNodeColor(NodeType);
            style.backgroundColor = color;
            
            // 设置边框样式
            var borderWidth = isRootNode ? 3f : 1f; // Root节点使用更粗的边框
            var borderColor = isRootNode ? new Color(1f, 0.8f, 0.2f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f); // Root节点使用金色边框
            
            style.borderTopWidth = borderWidth;
            style.borderBottomWidth = borderWidth;
            style.borderLeftWidth = borderWidth;
            style.borderRightWidth = borderWidth;
            style.borderTopColor = borderColor;
            style.borderBottomColor = borderColor;
            style.borderLeftColor = borderColor;
            style.borderRightColor = borderColor;
            style.borderTopLeftRadius = isRootNode ? 10 : 6; // Root节点使用更大的圆角
            style.borderTopRightRadius = isRootNode ? 10 : 6;
            style.borderBottomLeftRadius = isRootNode ? 10 : 6;
            style.borderBottomRightRadius = isRootNode ? 10 : 6;
            
            // 设置标题颜色
            var titleLabel = this.Q<Label>("title-label");
            if (titleLabel != null)
            {
                titleLabel.style.color = isRootNode ? new Color(1f, 0.9f, 0.6f, 1f) : Color.white; // Root节点使用金色文字
                titleLabel.style.fontSize = isRootNode ? 14 : 12; // Root节点使用更大字体
                titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            }
            
            // 添加类型标签
            var typeLabel = new Label(NodeType.ToString());
            typeLabel.AddToClassList("node-type-label");
            typeLabel.style.fontSize = isRootNode ? 10 : 9;
            typeLabel.style.color = isRootNode ? new Color(1f, 0.8f, 0.2f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f);
            typeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            typeLabel.style.unityFontStyleAndWeight = isRootNode ? FontStyle.Bold : FontStyle.Normal;
            
            titleContainer.Add(typeLabel);

            // 创建错误指示器
            CreateErrorIndicator();
        }

        /// <summary>
        /// 使标题可编辑
        /// </summary>
        private void MakeTitleEditable()
        {
            // 检查是否为Root节点，Root节点不允许重命名
            if (DataNode.NodeType == BehaviorNodeType.Root)
                return;

            // 获取标题标签
            var titleLabel = this.Q<Label>("title-label");
            if (titleLabel == null)
                return;

            // 添加双击事件来编辑标题
            titleLabel.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.clickCount == 2 && evt.button == 0) // 双击左键
                {
                    StartEditingTitle();
                    evt.StopPropagation();
                }
            });

            // 添加右键菜单选项
            titleLabel.RegisterCallback<ContextualMenuPopulateEvent>(evt =>
            {
                evt.menu.AppendAction("重命名节点", (a) => StartEditingTitle());
            });
        }

        /// <summary>
        /// 开始重命名节点（公共方法）
        /// </summary>
        public void StartRenaming()
        {
            // 检查是否为Root节点，Root节点不允许重命名
            if (DataNode.NodeType == BehaviorNodeType.Root)
            {
                UnityEngine.Debug.LogWarning("Root节点不允许重命名");
                return;
            }

            StartEditingTitle();
        }

        /// <summary>
        /// 开始编辑标题
        /// </summary>
        private void StartEditingTitle()
        {
            // 获取标题标签
            var titleLabel = this.Q<Label>("title-label");
            if (titleLabel == null)
                return;

            // 创建文本输入框
            var textField = new TextField
            {
                value = DataNode.Name
                // 移除 isDelayed = true，让值变化事件立即触发
            };

            // 设置样式
            textField.style.position = Position.Absolute;
            textField.style.left = titleLabel.worldBound.x - worldBound.x;
            textField.style.top = titleLabel.worldBound.y - worldBound.y;
            textField.style.width = titleLabel.worldBound.width;
            textField.style.height = titleLabel.worldBound.height;
            textField.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            textField.style.borderTopWidth = 1;
            textField.style.borderBottomWidth = 1;
            textField.style.borderLeftWidth = 1;
            textField.style.borderRightWidth = 1;
            textField.style.borderTopColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            textField.style.borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            textField.style.borderLeftColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            textField.style.borderRightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

            // 隐藏原始标题
            titleLabel.style.display = DisplayStyle.None;

            // 添加文本输入框到节点
            Add(textField);

            // 聚焦到输入框并选中所有文本
            textField.Focus();
            textField.SelectAll();

            // 注册回车键事件（优先处理）
            textField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    FinishEditingTitle(textField, titleLabel, textField.value);
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    CancelEditingTitle(textField, titleLabel);
                    evt.StopPropagation();
                }
            });

            // 注册失去焦点事件
            textField.RegisterCallback<FocusOutEvent>(evt =>
            {
                FinishEditingTitle(textField, titleLabel, textField.value);
            });
        }

        /// <summary>
        /// 完成编辑标题
        /// </summary>
        /// <param name="textField">文本输入框</param>
        /// <param name="titleLabel">标题标签</param>
        /// <param name="newName">新名称</param>
        private void FinishEditingTitle(TextField textField, Label titleLabel, string newName)
        {
            // 验证名称
            if (string.IsNullOrWhiteSpace(newName))
            {
                newName = DataNode.Name; // 如果为空，保持原名称
            }

            // 更新数据
            if (newName != DataNode.Name)
            {
                DataNode.Name = newName;
                title = newName;
                
                // 直接更新标题标签的文本
                if (titleLabel != null)
                {
                    titleLabel.text = newName;
                }
                
                // 标记图为脏
                MarkDirtyRepaint();
                
                // 通知图视图数据已更改
                if (OnNodeEvent != null)
                {
                    // 这里可以添加一个专门的事件类型来通知数据更改
                    // 暂时使用MarkDirtyRepaint来触发重绘
                }
            }

            // 清理UI
            CleanupEditingUI(textField, titleLabel);
        }

        /// <summary>
        /// 取消编辑标题
        /// </summary>
        /// <param name="textField">文本输入框</param>
        /// <param name="titleLabel">标题标签</param>
        private void CancelEditingTitle(TextField textField, Label titleLabel)
        {
            CleanupEditingUI(textField, titleLabel);
        }

        /// <summary>
        /// 清理编辑UI
        /// </summary>
        /// <param name="textField">文本输入框</param>
        /// <param name="titleLabel">标题标签</param>
        private void CleanupEditingUI(TextField textField, Label titleLabel)
        {
            // 移除文本输入框
            if (textField != null && textField.parent != null)
            {
                textField.parent.Remove(textField);
            }

            // 显示原始标题
            if (titleLabel != null)
            {
                titleLabel.style.display = DisplayStyle.Flex;
            }
        }

        /// <summary>
        /// 创建端口
        /// </summary>
        private void CreatePorts()
        {
            // 创建输入端口（所有节点都有输入端口，除非明确标记为根节点）
            // 注意：这里我们总是创建输入端口，因为节点在创建时可能还不知道自己是否是根节点
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            InputPort.portName = "";
            inputContainer.Add(InputPort);

            // 创建输出端口（如果支持子节点）
            if (DataNode.SupportChildren)
            {
                var capacity = DataNode.MaxChildrenCount == 1 ? Port.Capacity.Single : Port.Capacity.Multi;
                OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(bool));
                OutputPort.portName = "";
                outputContainer.Add(OutputPort);
            }
        }

        /// <summary>
        /// 创建属性UI
        /// </summary>
        private void CreatePropertiesUI()
        {
            if (DataNode.Properties.Count == 0)
                return;

            // 创建属性容器
            m_PropertiesContainer = new VisualElement();
            m_PropertiesContainer.AddToClassList("properties-container");
            
            var foldout = new Foldout { text = "属性", value = false };
            foldout.AddToClassList("properties-foldout");
            
            // 为每个属性创建编辑字段
            foreach (var property in DataNode.Properties)
            {
                var propertyElement = CreatePropertyElement(property);
                if (propertyElement != null)
                {
                    foldout.Add(propertyElement);
                }
            }
            
            m_PropertiesContainer.Add(foldout);
            extensionContainer.Add(m_PropertiesContainer);
        }

        /// <summary>
        /// 创建属性编辑元素
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>属性编辑元素</returns>
        private VisualElement CreatePropertyElement(BehaviorNodeProperty property)
        {
            var container = new VisualElement();
            container.AddToClassList("property-element");
            
            var label = new Label(property.Name);
            label.AddToClassList("property-label");
            container.Add(label);

            // 根据属性值类型创建不同的编辑控件
            VisualElement field = null;
            
            switch (property.ValueType)
            {
                case BehaviorNodePropertyValueType.Bool:
                    var toggle = new Toggle { value = property.BoolValue };
                    toggle.RegisterValueChangedCallback(evt => 
                    {
                        property.BoolValue = evt.newValue;
                        MarkDirtyRepaint();
                    });
                    field = toggle;
                    break;

                case BehaviorNodePropertyValueType.Int:
                    var intField = new IntegerField { value = property.IntValue };
                    intField.RegisterValueChangedCallback(evt => 
                    {
                        property.IntValue = evt.newValue;
                        MarkDirtyRepaint();
                    });
                    field = intField;
                    break;

                case BehaviorNodePropertyValueType.Long:
                    var longField = new LongField { value = property.LongValue };
                    longField.RegisterValueChangedCallback(evt => 
                    {
                        property.LongValue = evt.newValue;
                        MarkDirtyRepaint();
                    });
                    field = longField;
                    break;

                case BehaviorNodePropertyValueType.Float:
                    var floatField = new FloatField { value = property.FloatValue };
                    floatField.RegisterValueChangedCallback(evt => 
                    {
                        property.FloatValue = evt.newValue;
                        MarkDirtyRepaint();
                    });
                    field = floatField;
                    break;

                case BehaviorNodePropertyValueType.Double:
                    var doubleField = new DoubleField { value = property.DoubleValue };
                    doubleField.RegisterValueChangedCallback(evt => 
                    {
                        property.DoubleValue = evt.newValue;
                        MarkDirtyRepaint();
                    });
                    field = doubleField;
                    break;

                case BehaviorNodePropertyValueType.String:
                    var textField = new TextField { value = property.StringValue };
                    textField.RegisterValueChangedCallback(evt => 
                    {
                        property.StringValue = evt.newValue;
                        MarkDirtyRepaint();
                    });
                    field = textField;
                    break;
            }

            if (field != null)
            {
                field.AddToClassList("property-field");
                container.Add(field);
            }

            return container;
        }

        /// <summary>
        /// 获取节点颜色
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <returns>颜色</returns>
        private Color GetNodeColor(BehaviorNodeType nodeType)
        {
            switch (nodeType)
            {
                case BehaviorNodeType.Action:
                    return new Color(0.25f, 0.35f, 0.25f, 1f); // 深绿色
                case BehaviorNodeType.Composite:
                    return new Color(0.25f, 0.3f, 0.45f, 1f); // 深蓝色
                case BehaviorNodeType.Decor:
                    return new Color(0.45f, 0.35f, 0.25f, 1f); // 深橙色
                case BehaviorNodeType.Root:
                    return new Color(0.6f, 0.4f, 0.1f, 1f); // 深金色背景
                default:
                    return new Color(0.35f, 0.35f, 0.35f, 1f); // 深灰色
            }
        }

        /// <summary>
        /// 设置节点位置
        /// </summary>
        /// <param name="newPos">新位置</param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            DataNode.GraphPosition = newPos.position;
        }

        /// <summary>
        /// 构建右键菜单
        /// </summary>
        /// <param name="evt">菜单事件</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // 检查是否为Root节点
            bool isRootNode = DataNode.NodeType == BehaviorNodeType.Root;
            
            // 只有非Root节点才显示重命名选项
            if (!isRootNode)
            {
                evt.menu.AppendAction("重命名节点", (a) => StartEditingTitle());
                evt.menu.AppendSeparator();
            }
            
            // Root节点不显示复制和删除选项
            if (!isRootNode)
            {
                evt.menu.AppendAction("复制节点", (a) => DuplicateNode());
                
                // 根据选择状态显示不同的删除选项
                if (selected)
                {
                    evt.menu.AppendAction("删除选中节点", (a) => DeleteNode());
                }
                else
                {
                    evt.menu.AppendAction("删除节点", (a) => DeleteNode());
                }
            }
        }

        /// <summary>
        /// 复制节点
        /// </summary>
        private void DuplicateNode()
        {
            OnNodeEvent?.Invoke(BehaviorGraphEventType.DuplicateNode, this);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        private void DeleteNode()
        {
            // 如果当前节点被选中，删除所有选中的节点；否则只删除当前节点
            if (selected)
            {
                OnNodeEvent?.Invoke(BehaviorGraphEventType.DeleteSelectedNodes, this);
            }
            else
            {
                OnNodeEvent?.Invoke(BehaviorGraphEventType.DeleteNode, this);
            }
        }

        /// <summary>
        /// 创建错误指示器
        /// </summary>
        private void CreateErrorIndicator()
        {
            // 创建错误图标
            m_ErrorIndicator = new VisualElement();
            m_ErrorIndicator.AddToClassList("error-indicator");
            m_ErrorIndicator.style.position = Position.Absolute;
            m_ErrorIndicator.style.top = 2;
            m_ErrorIndicator.style.right = 2;
            m_ErrorIndicator.style.width = 16;
            m_ErrorIndicator.style.height = 16;
            m_ErrorIndicator.style.backgroundColor = new Color(1f, 0.2f, 0.2f, 0.9f); // 红色错误
            m_ErrorIndicator.style.borderTopLeftRadius = 8;
            m_ErrorIndicator.style.borderTopRightRadius = 8;
            m_ErrorIndicator.style.borderBottomLeftRadius = 8;
            m_ErrorIndicator.style.borderBottomRightRadius = 8;
            m_ErrorIndicator.style.display = DisplayStyle.None; // 默认隐藏

            // 添加错误符号
            var errorLabel = new Label("!");
            errorLabel.style.color = Color.white;
            errorLabel.style.fontSize = 10;
            errorLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            errorLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            errorLabel.style.flexGrow = 1;
            m_ErrorIndicator.Add(errorLabel);

            // 添加到节点
            Add(m_ErrorIndicator);
            
            // 更新错误状态
            UpdateErrorState();
        }

        /// <summary>
        /// 更新错误状态
        /// </summary>
        private void UpdateErrorState()
        {
            if (m_ErrorIndicator == null)
                return;

            // 检查是否需要显示错误
            bool showError = DataNode.SupportChildren && DataNode.ChildrenIds.Count == 0;
            
            m_ErrorIndicator.style.display = showError ? DisplayStyle.Flex : DisplayStyle.None;
            
            if (showError)
            {
                m_ErrorIndicator.tooltip = "此节点支持子节点但没有连接任何子节点";
            }
        }

        /// <summary>
        /// 刷新节点显示
        /// </summary>
        public void RefreshNode()
        {
            title = DataNode.Name;
            
            // 刷新属性显示
            if (m_PropertiesContainer != null)
            {
                extensionContainer.Remove(m_PropertiesContainer);
                CreatePropertiesUI();
            }
            
            // 更新错误状态
            UpdateErrorState();
            
            MarkDirtyRepaint();
        }

        /// <summary>
        /// 设置为根节点显示
        /// </summary>
        /// <param name="isRoot">是否为根节点</param>
        public void SetAsRoot(bool isRoot)
        {
            if (isRoot)
            {
                // 隐藏输入端口
                if (InputPort != null)
                {
                    InputPort.style.display = DisplayStyle.None;
                }
                
                // 添加根节点样式
                AddToClassList("root-node");
            }
            else
            {
                // 显示输入端口
                if (InputPort != null)
                {
                    InputPort.style.display = DisplayStyle.Flex;
                }
                
                // 移除根节点样式
                RemoveFromClassList("root-node");
            }
        }
    }

    /// <summary>
    /// 行为树图事件类型
    /// </summary>
    public enum BehaviorGraphEventType
    {
        DuplicateNode,
        DeleteNode,
        DeleteSelectedNodes
    }

    /// <summary>
    /// 行为树图事件
    /// </summary>
    public class BehaviorGraphEvent : EventBase<BehaviorGraphEvent>
    {
        public BehaviorGraphEventType EventType { get; private set; }
        public BehaviorGraphNode Node { get; private set; }

        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            bubbles = true;
            tricklesDown = true;
        }

        public static BehaviorGraphEvent GetPooled(BehaviorGraphEventType eventType, BehaviorGraphNode node)
        {
            var evt = GetPooled();
            evt.EventType = eventType;
            evt.Node = node;
            return evt;
        }
    }
} 