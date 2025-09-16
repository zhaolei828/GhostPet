# Unity编辑器设置指南

## 🎯 **需要在Unity编辑器中检查和设置的内容**

### **第一步：检查UIManager组件**

1. **打开主场景** (`Assets/Scenes/SampleScene.unity`)

2. **找到UIManager对象**：
   - 在Hierarchy窗口中查找包含UIManager组件的GameObject
   - 如果找不到，创建一个新的空GameObject命名为"UIManager"

3. **检查UIManager组件**：
   - 选中UIManager GameObject
   - 在Inspector中检查是否有UIManager组件
   - 如果没有，点击"Add Component" → 搜索"UIManager" → 添加

### **第二步：设置UI组件引用**

在UIManager的Inspector面板中设置以下引用：

#### **UI面板引用**
- **Game UI Canvas**: 拖拽主Canvas到此处
- **Game UI Panel**: 拖拽游戏UI面板到此处（如果有的话）
- **Pause Menu Panel**: 拖拽暂停菜单面板到此处（如果有的话）

#### **玩家UI引用**
- **Player Health Bar**: 拖拽血量条UI组件到此处
- **Score UI**: 拖拽分数显示UI组件到此处
- **Sword Status UI**: 拖拽飞剑状态UI组件到此处

#### **调试设置**
- **Show Debug Info**: 勾选此项可以显示调试信息
- **Debug Text**: 拖拽一个Text组件来显示调试信息（可选）

### **第三步：检查Canvas设置**

1. **选择主Canvas**
2. **检查Canvas组件设置**：
   - Render Mode: Screen Space - Overlay ✅
   - Sorting Order: 0 ✅

3. **检查Canvas Scaler组件**：
   - UI Scale Mode: Scale With Screen Size ✅
   - Reference Resolution: 1920 x 1080 ✅
   - Screen Match Mode: Match Width Or Height ✅
   - Match: 0.5 ✅

### **第四步：如果UI组件不存在，创建基本UI**

如果场景中缺少UI组件，可以快速创建：

#### **创建血量条**
1. 右键Canvas → UI → Slider
2. 重命名为"PlayerHealthBar"
3. 添加PlayerHealthBar脚本组件

#### **创建分数UI**
1. 右键Canvas → UI → Text - TextMeshPro
2. 重命名为"ScoreUI"
3. 添加ScoreUI脚本组件

#### **创建飞剑状态UI**
1. 右键Canvas → UI → Text - TextMeshPro
2. 重命名为"SwordStatusUI"
3. 添加SwordStatusUI脚本组件

### **第五步：测试设置**

1. **运行游戏**：点击Play按钮
2. **观察控制台**：应该看到UI初始化的日志
3. **检查UI位置**：UI元素应该在正确位置显示

### **第六步：测试UI稳定性**

1. **右键点击UIManager组件** → 选择"测试UI稳定性"
2. **改变Game窗口分辨率**：测试不同分辨率下的显示效果
3. **观察控制台输出**：查看位置偏差报告

## ⚠️ **常见问题解决**

### **如果UIManager组件显示为"Missing"**
- 删除原组件，重新添加UIManager组件

### **如果UI引用为空**
- 手动拖拽对应的UI对象到相应字段

### **如果UI位置不正确**
- 运行"测试UI稳定性"功能
- 检查RectTransform的锚点设置

### **如果看不到UI**
- 检查Canvas的Sorting Order
- 确保UI对象是Canvas的子对象

## 🎯 **成功验证标准**

✅ **控制台输出包含**：
- "[UIManager] 开始自动查找UI组件..."
- "[UIManager] UI锚点设置完成"
- "[UIManager] UI初始化完成"

✅ **UI显示正常**：
- 血量条在左上角
- 分数UI在右上角  
- 飞剑状态在底部中心

✅ **无错误信息**：
- 控制台没有红色错误信息
- 所有UI组件引用都不为空

## 🚀 **准备好测试对象池系统**

基础UI测试完成后，我们可以逐步启用：
1. DamageNumber对象池
2. Enemy对象池
3. SwordAfterimage对象池
