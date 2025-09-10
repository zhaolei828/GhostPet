# 🎮 GhostPet Unity编辑器设置指南
*手把手教你在Unity中设置游戏*

## 📚 第一步：打开Unity项目

1. **启动Unity Hub**
   - 双击桌面上的Unity Hub图标
   - 如果没有，从开始菜单找到Unity Hub

2. **打开GhostPet项目**
   - 在Unity Hub中点击"Open"（打开）
   - 导航到：`D:\Program Files\Unity\Hub\Project\GhostPet`
   - 点击"Select Folder"（选择文件夹）
   - 等待Unity加载项目（可能需要几分钟）

## 🎯 第二步：了解Unity界面

Unity编辑器主要有几个窗口：
- **Scene视图** (左上)：3D/2D场景编辑器
- **Game视图** (左上，标签页)：游戏运行预览
- **Hierarchy** (左下)：场景中所有对象的层级列表
- **Inspector** (右侧)：选中对象的属性面板
- **Project** (底部)：项目文件浏览器
- **Console** (底部，标签页)：日志和错误信息

## 🔧 第三步：创建游戏管理器

### 3.1 创建空对象
1. 在**Hierarchy**窗口中**右键点击**空白处
2. 选择 **"Create Empty"**（创建空对象）
3. 新对象会出现，默认名为"GameObject"
4. **右键点击**这个对象，选择**"Rename"**（重命名）
5. 输入 **"GameManager"**，按回车确认

### 3.2 添加GameManager脚本
1. 确保GameManager对象已选中（在Hierarchy中点击它）
2. 在**Inspector**窗口中，点击底部的**"Add Component"**按钮
3. 在搜索框中输入 **"GameManager"**
4. 点击出现的"GameManager"脚本

## 👤 第四步：创建玩家对象

### 4.1 创建玩家对象
1. 在Hierarchy中右键，选择**"Create Empty"**
2. 重命名为 **"Player"**
3. 确保Player对象已选中

### 4.2 设置玩家标签
1. 在Inspector窗口顶部，找到**"Tag"**下拉菜单
2. 点击下拉菜单，选择**"Player"**
   - 如果没有Player标签，点击"Add Tag..."创建一个

### 4.3 添加玩家组件
依次添加以下组件（每次都点击"Add Component"）：

**1. Rigidbody2D**
- 搜索并添加"Rigidbody2D"
- 在Inspector中找到"Gravity Scale"，设置为 **0**（关闭重力）

**2. CircleCollider2D** 
- 搜索并添加"CircleCollider2D"
- 设置"Radius"为 **0.5**

**3. SpriteRenderer**
- 搜索并添加"SpriteRenderer"
- 暂时不用设置精灵图片

**4. PlayerController**
- 搜索并添加"PlayerController"
- 设置"Move Speed"为 **5**

**5. HealthSystem**
- 搜索并添加"HealthSystem"  
- 设置"Max Health"为 **100**

**6. FlyingSwordManager**
- 搜索并添加"FlyingSwordManager"
- 暂时保持默认设置

## 📷 第五步：设置摄像机

### 5.1 选择主摄像机
1. 在Hierarchy中找到**"Main Camera"**并点击选中

### 5.2 添加摄像机跟随脚本
1. 在Inspector中点击**"Add Component"**
2. 搜索并添加**"CameraFollow"**
3. 找到"Target"字段，点击右侧的**小圆圈**图标
4. 在弹出窗口中选择**"Player"**对象

### 5.3 调整摄像机设置
- 设置"Smooth Speed"为 **0.125**
- 设置"Offset"为 **(0, 0, -10)**

## 👻 第六步：创建敌人预制体

### 📖 什么是预制体（Prefab）？

**预制体就像是一个"模板"或"蓝图"**：

🏭 **生活中的类比**：
- 就像**饼干模具** - 用一个模具可以做出无数个相同的饼干
- 就像**建筑图纸** - 用一张图纸可以建造无数个相同的房子
- 就像**汽车生产线** - 用一个设计可以生产无数辆相同的汽车

🎮 **在Unity中**：
- **预制体** = 敌人的"模板"文件
- **实例** = 场景中具体的敌人对象

### 🎯 预制体的好处：

**1. 批量创建**
- 一次设计，无限使用
- EnemySpawner可以用Ghost预制体生成无数个敌人

**2. 统一管理** 
- 修改预制体 = 所有敌人同时更新
- 比如：增加敌人血量，所有Ghost都会变强

**3. 节省时间**
- 不用每次都重新添加组件和设置参数
- 新敌人直接"复制粘贴"就行

### 🔄 预制体工作流程：
```
1. 在Hierarchy创建对象 → 2. 配置组件和参数 → 3. 拖到Project做成预制体
4. 删除原对象 → 5. 用代码生成无数个实例
```

### 6.1 创建敌人对象
1. 在Hierarchy中右键，选择**"Create Empty"**
2. 重命名为 **"Ghost"**

### 6.2 设置敌人标签
1. 在Inspector窗口顶部，点击**"Tag"**下拉菜单
2. 点击**"Add Tag..."**
3. 点击**"+"**号，输入 **"Enemy"**，点击"Save"
4. 回到Ghost对象，设置Tag为**"Enemy"**

### 6.3 添加敌人组件
**1. Rigidbody2D**
- 添加"Rigidbody2D"
- 设置"Gravity Scale"为 **0**

**2. CircleCollider2D**
- 添加"CircleCollider2D"
- 设置"Radius"为 **0.4**

**3. SpriteRenderer**
- 添加"SpriteRenderer"

**4. EnemyAI**
- 添加"EnemyAI"脚本
- 设置"Move Speed"为 **3**
- 设置"Attack Damage"为 **10**

**5. HealthSystem**
- 添加"HealthSystem"
- 设置"Max Health"为 **50**

### 6.4 创建预制体
1. 确保Ghost对象设置完成
2. 在Project窗口中，**右键点击Assets文件夹**（根目录），选择**"Create > Folder"**
3. 新文件夹命名为**"Prefabs"**
   - 创建后的结构应该是：`Assets/Prefabs/`
   - 与Scripts、Scenes文件夹同级
4. 将Hierarchy中的Ghost对象**拖拽**到Prefabs文件夹中
   - 🎯 此时Unity会创建Ghost.prefab预制体
5. **处理Hierarchy中的Ghost对象** - 你有两种选择：

   **🗑️ 选择A：删除Ghost对象（推荐）**
   - 右键点击Ghost对象 → 选择"Delete"
   - ✅ 场景更干净，只有EnemySpawner生成的敌人
   - ✅ 符合游戏设计逻辑
   
   **👁️ 选择B：隐藏Ghost对象（方便调试）**
   - 在Inspector窗口中，**取消勾选**左上角的✅复选框
   - 或者点击Hierarchy中Ghost对象名字前的👁️图标
   - ✅ 保留了原对象，方便以后修改预制体
   - ✅ 对象隐藏，不会影响游戏运行
   - ⚠️ Hierarchy看起来稍微杂乱一些

### 🔧 以后如何修改预制体？

**方法1：直接编辑预制体文件**
1. 在Project窗口中**双击Ghost.prefab**
2. 进入预制体编辑模式
3. 修改组件、添加新功能
4. 点击"Save"保存

**方法2：通过实例修改（如果你选择了隐藏）**
1. 重新激活隐藏的Ghost对象
2. 修改它的组件和属性
3. 在Inspector顶部点击"Overrides"下拉菜单
4. 选择"Apply All"应用到预制体

**方法3：创建临时实例**
1. 将Ghost.prefab从Project拖拽到Hierarchy
2. 修改这个临时实例
3. 应用修改到预制体
4. 删除临时实例

## 🏭 第七步：设置敌人生成器

### 7.1 创建生成器对象
1. 在Hierarchy中创建空对象，命名为**"EnemySpawner"**

### 7.2 配置生成器
1. 添加**"EnemySpawner"**脚本
2. 在Inspector中找到"Enemy Prefabs"
3. 设置"Size"为 **1**
4. 点击"Element 0"右侧的小圆圈，选择Ghost预制体
5. 设置其他参数：
   - "Spawn Interval": **3**
   - "Max Enemies": **10**
   - "Min Spawn Radius": **6**
   - "Max Spawn Radius": **12**

## ⚔️ 第八步：创建飞剑预制体

### 8.1 创建飞剑对象
1. 创建空对象，命名为**"FlyingSword"**

### 8.2 添加飞剑组件
**1. SpriteRenderer**
- 添加"SpriteRenderer"

**2. TrailRenderer**
- 添加"TrailRenderer"
- 设置"Time"为 **0.3**

**3. CircleCollider2D**
- 添加"CircleCollider2D"
- 勾选**"Is Trigger"**
- 设置"Radius"为 **0.2**

**4. FlyingSword脚本**
- 添加"FlyingSword"脚本（注意：是FlyingSword，不是FlyingSwordManager）
- FlyingSword：控制单个飞剑的行为
- FlyingSwordManager：管理多个飞剑，已经添加到Player上了

### 8.3 创建飞剑预制体
1. 将Hierarchy中的FlyingSword对象**拖拽**到**Assets/Prefabs/**文件夹中
2. **处理原FlyingSword对象**：
   - **推荐**：删除原对象（右键Delete）
   - **或者**：隐藏原对象（取消勾选Inspector中的✅）
   - 原理同Ghost对象，选择适合你的方式

### 8.4 配置玩家的飞剑管理器
1. 选择Player对象
2. 在FlyingSwordManager组件中：
   - 将"Sword Prefab"设置为FlyingSword预制体
   - "Max Swords"设置为 **6**
   - "Orbit Radius"设置为 **2**

## 📋 第九步：检查Hierarchy层级结构

在运行游戏前，请确认你的**Hierarchy**窗口中的对象结构如下：

```
SampleScene
├── Main Camera                    [Camera, Audio Listener, CameraFollow]
├── Directional Light             [Light]
├── GameManager                   [GameManager]
├── Player                        [Transform, Rigidbody2D, CircleCollider2D, 
│                                  SpriteRenderer, PlayerController, 
│                                  HealthSystem, FlyingSwordManager]
└── EnemySpawner                  [EnemySpawner]
```

### 🔍 详细检查每个对象：

**📷 Main Camera**
- ✅ 应该有 **CameraFollow** 脚本
- ✅ CameraFollow的 **Target** 字段应该指向 **Player** 对象
- ✅ Position应该是 (0, 0, -10)

**🎮 GameManager** 
- ✅ 空对象，只有 **GameManager** 脚本
- ✅ Position应该是 (0, 0, 0)

**👤 Player**
- ✅ Tag设置为 **"Player"**
- ✅ 包含所有必要组件（见上面列表）
- ✅ PlayerController的 **Input Actions** 字段应该指向 InputSystem_Actions 资源
- ✅ FlyingSwordManager的 **Sword Prefab** 应该指向 FlyingSword 预制体

**🏭 EnemySpawner**
- ✅ 只有 **EnemySpawner** 脚本
- ✅ **Enemy Prefabs** 数组应该包含 Ghost 预制体
- ✅ Position应该是 (0, 0, 0)

### 📦 Project窗口中的预制体：
在 **Assets/Prefabs/** 文件夹中应该有：
- 🗂️ **Prefabs** 文件夹
  - 👻 **Ghost.prefab** (敌人预制体)
  - ⚔️ **FlyingSword.prefab** (飞剑预制体)

> **💡 提示**：如果你的Hierarchy结构不匹配，回到相应的步骤重新检查设置！

## 🎮 第十步：测试游戏

### 10.1 保存场景

你有两个选择：

**选择A：保持SampleScene名称（推荐）**
1. 按 **Ctrl+S** 保存场景
2. 保持默认的"SampleScene"名称不变
3. ✅ 简单快捷，适合学习和测试

**选择B：重命名为GameScene**
1. 在Project窗口中找到 **SampleScene.unity** 文件
2. 右键点击 → 选择**"Rename"**
3. 输入 **"GameScene"**，按回车确认
4. 按 **Ctrl+S** 保存场景修改
5. ✅ 名称更有意义，适合正式项目

> 💡 **提示**：对于学习阶段，保持SampleScene名称就够了！

### 10.2 运行游戏
1. 点击编辑器上方的**播放按钮**（▶️）
2. 游戏应该开始运行！

### 10.3 测试控制
- 使用**WASD**或**方向键**移动玩家
- 按**空格键**或**鼠标左键**进行攻击
- 观察敌人是否生成和追击
- 观察飞剑是否环绕和攻击

## 🔧 第十一步：调试和优化

### 如果遇到问题：

**1. 检查Console窗口**
- 点击底部的"Console"标签
- 查看是否有红色错误信息

**2. 常见问题解决：**
- **玩家不动**：检查PlayerController脚本是否添加
- **摄像机不跟随**：检查CameraFollow的Target是否设置
- **敌人不生成**：检查EnemySpawner的Enemy Prefabs是否设置
- **飞剑不出现**：检查FlyingSwordManager的Sword Prefab是否设置

**3. 如果脚本报错**：
- 检查所有脚本是否正确添加到对象上
- 确保预制体引用正确设置

## 🎉 完成！

恭喜！你已经成功设置了GhostPet游戏的基础系统。现在你可以：
- 控制玩家移动
- 看到敌人生成和追击
- 体验飞剑自动攻击
- 感受完整的游戏循环

## 📋 最终检查清单

### Hierarchy结构检查：
- [ ] **Main Camera** - 包含CameraFollow脚本，Target指向Player
- [ ] **Directional Light** - Unity默认光源
- [ ] **GameManager** - 空对象 + GameManager脚本
- [ ] **Player** - 包含所有必要组件（Tag设为"Player"）
- [ ] **EnemySpawner** - EnemySpawner脚本 + Ghost预制体引用

### 组件配置检查：
- [ ] Player的PlayerController输入系统已设置
- [ ] Player的FlyingSwordManager飞剑预制体已设置
- [ ] 摄像机CameraFollow目标已设置
- [ ] EnemySpawner的敌人预制体数组已配置

### 预制体检查：
- [ ] **Assets/Prefabs/Ghost.prefab** 已创建
- [ ] **Assets/Prefabs/FlyingSword.prefab** 已创建
- [ ] Ghost预制体Tag设为"Enemy"
- [ ] 所有预制体组件配置正确

### 运行测试：
- [ ] 游戏可以运行并控制玩家
- [ ] 敌人会自动生成和追击
- [ ] 飞剑环绕玩家并自动攻击敌人
- [ ] 摄像机跟随玩家移动

如果某一步有问题，随时告诉我具体在哪里卡住了！

