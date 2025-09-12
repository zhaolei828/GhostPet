# 🎨 GhostPet UI系统设置指南

完整的UI系统已创建！本指南将帮你在Unity中快速设置所有UI元素。

## 📋 **UI系统组件**

### ✅ **已完成的脚本**
- `UIManager.cs` - UI总管理器
- `PlayerHealthBar.cs` - 玩家血量条
- `ScoreUI.cs` - 分数显示系统
- `SwordStatusUI.cs` - 飞剑状态显示

## 🚀 **快速设置步骤**

### **第1步：创建UI Canvas**

1. **创建主Canvas**：
   - 在Hierarchy中右键 → `UI` → `Canvas`
   - 命名为 `GameUICanvas`
   - 添加 `UIManager` 脚本

2. **Canvas设置**（UIManager会自动配置）：
   - Render Mode: `Screen Space - Overlay`
   - Canvas Scaler: `Scale With Screen Size`
   - Reference Resolution: `1920 x 1080`

### **第2步：创建UI布局**

在GameUICanvas下创建以下结构：

```
GameUICanvas
├── GameUIPanel (Panel)
│   ├── TopPanel (Panel) - 顶部信息栏
│   │   ├── PlayerHealthBar (Panel)
│   │   └── ScorePanel (Panel)
│   ├── BottomPanel (Panel) - 底部状态栏
│   │   └── SwordStatusPanel (Panel)
│   └── DebugPanel (Panel) - 调试信息 (可选)
└── PauseMenuPanel (Panel) - 暂停菜单
```

### **第3步：设置玩家血量条**

1. **创建血量条容器**：
   - 在TopPanel下创建 `PlayerHealthBar` (Panel)
   - 添加 `PlayerHealthBar.cs` 脚本

2. **创建血量条元素**：
   ```
   PlayerHealthBar
   ├── HealthSlider (Slider)
   │   ├── Background (Image)
   │   ├── Fill Area
   │   │   └── Fill (Image) - 设置颜色为绿色
   │   └── Handle Slide Area (可删除)
   └── HealthText (TextMeshProUGUI)
   ```

3. **配置PlayerHealthBar脚本**：
   - Health Slider: 拖入HealthSlider
   - Fill Image: 拖入Fill
   - Health Text: 拖入HealthText
   - 勾选Show Numbers

### **第4步：设置分数显示**

1. **创建分数面板**：
   - 在TopPanel下创建 `ScorePanel` (Panel)
   - 添加 `ScoreUI.cs` 脚本

2. **创建分数元素**：
   ```
   ScorePanel
   ├── KillCountText (TextMeshProUGUI) - "击杀: 0"
   ├── SurvivalTimeText (TextMeshProUGUI) - "时间: 00:00"
   └── TotalScoreText (TextMeshProUGUI) - "总分: 0"
   ```

3. **配置ScoreUI脚本**：
   - Kill Count Text: 拖入KillCountText
   - Survival Time Text: 拖入SurvivalTimeText
   - Total Score Text: 拖入TotalScoreText

### **第5步：设置飞剑状态显示**

1. **创建飞剑状态面板**：
   - 在BottomPanel下创建 `SwordStatusPanel` (Panel)
   - 添加 `SwordStatusUI.cs` 脚本

2. **创建状态元素**：
   ```
   SwordStatusPanel
   ├── SwordCountText (TextMeshProUGUI) - "飞剑: 6/6"
   └── SwordIconContainer (Panel) - 用于放图标
   ```

3. **配置SwordStatusUI脚本**：
   - Sword Count Text: 拖入SwordCountText
   - Sword Icon Container: 拖入SwordIconContainer

### **第6步：配置UIManager**

1. **在UIManager脚本中设置引用**：
   - Game UI Canvas: 拖入GameUICanvas
   - Game UI Panel: 拖入GameUIPanel
   - Player Health Bar: 拖入PlayerHealthBar组件
   - Score UI: 拖入ScoreUI组件
   - Sword Status UI: 拖入SwordStatusUI组件

## 🎨 **推荐UI布局**

### **移动端友好布局**：

```
屏幕顶部（SafeArea）:
[❤️ 100/100] [击杀: 0] [时间: 00:00] [总分: 0]

屏幕底部:
[🗡️🗡️🗡️🗡️🗡️🗡️] 飞剑: 6/6
```

### **UI元素尺寸建议**：
- 血量条：宽300px，高30px
- 文本字体：24-32px（移动端友好）
- 飞剑图标：30x30px
- 面板间距：10-20px

## ⚙️ **样式建议**

### **颜色主题**：
- 背景：半透明黑色 (0,0,0,128)
- 血量条：绿色→红色渐变
- 文本：白色或淡青色
- 高亮：黄色或金色

### **字体设置**：
- 使用TextMeshPro
- 字体：LiberationSans SDF
- 加粗显示重要信息

## 🔗 **与游戏系统集成**

### **在相关脚本中添加UI更新调用**：

1. **在HealthSystem.cs中**：
   ```csharp
   private void UpdateHealth()
   {
       // 现有代码...
       
       // 更新UI
       if (UIManager.Instance != null)
       {
           UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
       }
   }
   ```

2. **在EnemyAI.cs中（敌人死亡时）**：
   ```csharp
   private void Die()
   {
       // 现有代码...
       
       // 更新击杀数
       if (UIManager.Instance != null && UIManager.Instance.scoreUI != null)
       {
           UIManager.Instance.scoreUI.AddKill();
       }
   }
   ```

3. **在FlyingSwordManager.cs中**：
   ```csharp
   private void UpdateSwordStatus()
   {
       // 计算飞剑状态
       int available = availableSwords.Count;
       int attacking = attackingSwords.Count;
       
       // 更新UI
       if (UIManager.Instance != null)
       {
           UIManager.Instance.UpdateSwordStatus(available, attacking);
       }
   }
   ```

## 🎯 **测试检查清单**

- [ ] 血量条正确显示玩家血量
- [ ] 分数系统正确计算击杀和时间
- [ ] 飞剑状态正确显示可用/攻击状态
- [ ] UI在不同分辨率下正常显示
- [ ] 暂停功能正常工作（ESC键）
- [ ] 所有文本清晰可读

## 🚀 **下一步优化**

1. **添加音效提示**（血量低、分数增加）
2. **创建游戏结束界面**
3. **添加设置菜单**
4. **实现成就系统**
5. **优化移动端触控体验**

---

**🎮 完成设置后，你将拥有一个专业级的游戏UI界面！**

有任何问题都可以随时问我！ ✨
