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
            NodeType = GetNodeTypeFromTypeName(dataNode.TypeName);
            
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
            var typeLabel = new Label(isRootNode ? "ROOT" : NodeType.ToString());
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
        /// 从类型名获取节点类型
        /// </summary>
        /// <param name="typeName">类型名</param>
        /// <returns>节点类型</returns>
        private BehaviorNodeType GetNodeTypeFromTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return BehaviorNodeType.Action;

            if (typeName.Contains("Action"))
                return BehaviorNodeType.Action;
            else if (typeName.Contains("Composite"))
                return BehaviorNodeType.Composite;
            else if (typeName.Contains("Decor"))
                return BehaviorNodeType.Decor;

            return BehaviorNodeType.Action;
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
        /// 更新节点标题
        /// </summary>
        /// <param name="newTitle">新标题</param>
        public void UpdateTitle(string newTitle)
        {
            title = newTitle;
            DataNode.Name = newTitle;
        }

        /// <summary>
        /// 构建右键菜单
        /// </summary>
        /// <param name="evt">菜单事件</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // 检查是否为Root节点
            bool isRootNode = DataNode.NodeType == BehaviorNodeType.Root;
            
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
                
                evt.menu.AppendSeparator();
            }
            
            // Root节点不显示"设为根节点"选项，其他支持子节点的节点显示
            if (!isRootNode && DataNode.SupportChildren)
            {
                evt.menu.AppendAction("设为根节点", (a) => SetAsRootNode());
            }
        }

        /// <summary>
        /// 设为根节点
        /// </summary>
        private void SetAsRootNode()
        {
            // 通过委托通知图视图
            OnNodeEvent?.Invoke(BehaviorGraphEventType.SetRootNode, this);
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
        SetRootNode,
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