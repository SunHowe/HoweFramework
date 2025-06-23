# 行为树编辑器

基于 Unity GraphView 实现的可视化行为树编辑器，用于创建和编辑 HoweFramework 行为树资源。

## 功能特性

### 核心功能
- 🎨 **可视化编辑**：基于 GraphView 的直观节点编辑界面
- 📦 **节点系统**：支持行为节点、复合节点、装饰节点三大类型
- 🔗 **连接管理**：可视化的父子节点连接关系
- 💾 **资源管理**：支持新建、打开、保存、另存为行为树资源
- ✅ **验证系统**：实时验证行为树的完整性和正确性
- 🔍 **搜索创建**：快速搜索和创建所需节点类型

### 节点类型

#### 行为节点（Action Nodes）
- **Log**：输出日志信息
- **Success**：总是返回成功
- **Failure**：总是返回失败

#### 复合节点（Composite Nodes）
- **Sequence**：按顺序执行所有子节点，直到有一个失败
- **Selector**：按顺序执行子节点，直到有一个成功
- **Parallel**：同时执行所有子节点

#### 装饰节点（Decorator Nodes）
- **Inverter**：反转子节点的执行结果
- **Repeat**：重复执行子节点指定次数
- **Always Success**：总是返回成功
- **Always Failure**：总是返回失败
- **Root**：行为树的根节点

## 使用说明

### 打开编辑器
通过菜单 `HoweFramework > 行为树编辑器` 打开编辑器窗口。

### 基本操作

#### 创建节点
1. 在图视图空白区域右键点击
2. 选择"创建节点"
3. 在搜索窗口中选择所需的节点类型

#### 连接节点
- 从父节点的输出端口拖拽到子节点的输入端口
- 连接建立后会自动更新数据关系

#### 设置根节点
- 右键点击节点
- 选择"设为根节点"

#### 编辑属性
- 点击节点展开属性面板
- 修改节点的各种属性值

#### 节点操作
- **复制节点**：右键菜单选择"复制节点"
- **删除节点**：选中节点后按 Delete 键或右键选择"删除节点"

### 文件操作

#### 新建行为树
- 点击工具栏"新建"按钮
- 会创建一个空的行为树图

#### 打开现有行为树
- 点击工具栏"打开"按钮
- 选择 .asset 格式的行为树文件

#### 保存行为树
- 点击工具栏"保存"按钮
- 首次保存会提示选择保存位置

#### 验证行为树
- 点击工具栏"验证"按钮
- 检查行为树的完整性和正确性

## 架构设计

### 核心类结构

```
BehaviorGraphWindow          // 主编辑器窗口
├── BehaviorGraphView        // GraphView 主视图
│   ├── BehaviorGraphNode    // 节点视图
│   └── BehaviorNodeSearchWindow // 节点搜索窗口
└── BehaviorGraph            // 行为树数据资源
    └── BehaviorNode         // 节点数据
        └── BehaviorNodeProperty // 节点属性
```

### 模板系统

```
BehaviorNodeTemplate              // 抽象节点模板
├── BehaviorNodeTemplate_Action   // 行为节点模板基类
├── BehaviorNodeTemplate_Composite // 复合节点模板基类
└── BehaviorNodeTemplate_Decor    // 装饰节点模板基类
```

### 管理器系统

```
BehaviorNodeTemplateManager   // 节点模板管理器
├── 自动发现节点模板类型
├── 按类型分组管理
└── 提供搜索功能
```

## 扩展开发

### 创建自定义节点模板

1. 继承对应的节点模板基类：
```csharp
public class CustomActionTemplate : BehaviorNodeTemplate_Action
{
    public override string NodeName => "Custom Action";
    public override string NodeDescription => "自定义行为节点";
    public override string RuntimeTypeName => "YourNamespace.CustomActionNode";
    
    public override List<BehaviorNodePropertyTemplate> DefaultProperties =>
        new List<BehaviorNodePropertyTemplate>
        {
            new BehaviorNodePropertyTemplate("CustomProperty", typeof(string), "自定义属性", "默认值")
        };
}
```

2. 模板管理器会自动发现并加载新的节点模板

### 运行时类型映射

节点模板的 `RuntimeTypeName` 属性应该对应实际的运行时行为树节点类型：

```csharp
// 编辑器模板
public override string RuntimeTypeName => "HoweFramework.BehaviorLog";

// 对应的运行时类型
public class BehaviorLog : BehaviorActionNodeBase
{
    // 运行时实现
}
```

## 样式定制

编辑器使用 USS 样式表进行界面定制，样式文件位于：
`Assets/HoweFramework.Editor/BehaviorTree/Resources/BehaviorGraphView.uss`

可以修改节点颜色、连接线样式、字体等视觉效果。

## 注意事项

1. **数据持久化**：行为树数据以 ScriptableObject 形式保存为 .asset 文件
2. **验证规则**：编辑器会自动验证节点连接的合法性和循环引用
3. **性能考虑**：大型行为树建议分层管理，避免单个文件过于复杂
4. **版本兼容**：确保编辑器模板与运行时节点类型保持同步

## 故障排除

### 常见问题

**Q: 节点模板不显示？**
A: 检查节点模板类是否正确继承基类，并且程序集可以被编辑器访问。

**Q: 连接无法建立？**
A: 检查节点是否支持子节点，以及是否超过了子节点数量限制。

**Q: 保存失败？**
A: 确保有写入权限，并且文件路径有效。

**Q: 验证失败？**
A: 检查是否设置了根节点，以及所有连接关系是否完整。 