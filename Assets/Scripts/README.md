# GhostPet 游戏核心系统

## 🎮 游戏概述
GhostPet是一个2D动作游戏，玩家控制角色在地图中移动，周围环绕着飞剑自动攻击靠近的鬼怪敌人。

## 📁 脚本结构

### 🎯 Player（玩家系统）
- **PlayerController.cs** - 玩家移动控制和基础行为
  - 方向键/WASD控制移动
  - 基于新输入系统
  - 支持死亡和重生

### 📷 Camera（摄像机系统）  
- **CameraFollow.cs** - 摄像机跟随玩家
  - 平滑跟随
  - 可设置边界限制
  - 支持瞬移到目标（重生时使用）

### 🎮 Managers（管理器）
- **GameManager.cs** - 游戏状态管理
  - 单例模式
  - 玩家重生管理
  - 游戏生命周期控制

### ❤️ Health（血量系统）
- **HealthSystem.cs** - 通用血量组件
  - 支持伤害、治疗
  - 死亡事件处理
  - 血量变化事件

### ⚔️ Combat（战斗系统）
- **FlyingSwordManager.cs** - 飞剑管理器
  - 管理玩家周围环绕的飞剑
  - 自动检测敌人并发射攻击
  - 可配置攻击范围和伤害

- **FlyingSword.cs** - 单个飞剑控制
  - 环绕轨道移动
  - 攻击目标追踪
  - 自动返回轨道

### 👻 Enemy（敌人系统）
- **EnemyAI.cs** - 敌人AI控制
  - 状态机：空闲/追击/攻击
  - 自动寻找和追击玩家
  - 攻击冷却和伤害系统

- **EnemySpawner.cs** - 敌人生成器
  - 随机位置生成敌人
  - 难度递增系统
  - 最大敌人数量限制

### 🎮 Input（输入系统）
- **PlayerInputActions.cs** - 输入动作映射
  - 支持键盘和鼠标输入
  - WASD和方向键移动
  - 空格键和鼠标左键攻击

## 🔧 使用说明

### 1. 基础设置
1. 在场景中创建空游戏对象并添加 `GameManager` 组件
2. 创建玩家对象并添加以下组件：
   - `PlayerController`
   - `HealthSystem`  
   - `FlyingSwordManager`
   - `Rigidbody2D`
   - `SpriteRenderer`
   - `Collider2D`

### 2. 摄像机设置
1. 在主摄像机上添加 `CameraFollow` 组件
2. 设置目标为玩家对象

### 3. 敌人设置
1. 创建敌人预制体，添加组件：
   - `EnemyAI`
   - `HealthSystem`
   - `Rigidbody2D`
   - `SpriteRenderer`
   - `Collider2D`
2. 设置敌人标签为 "Enemy"
3. 创建空对象添加 `EnemySpawner`，配置敌人预制体

### 4. 飞剑设置
1. 创建飞剑预制体，添加组件：
   - `FlyingSword`
   - `SpriteRenderer`
   - `TrailRenderer`（可选）
   - `Collider2D`（设为Trigger）

## ⭐ 核心特性

### ✅ 已实现功能
- [x] 玩家移动控制（方向键/WASD）
- [x] 摄像机跟随玩家移动
- [x] 敌人随机生成系统
- [x] 敌人AI追击玩家
- [x] 飞剑环绕和自动攻击
- [x] 血量系统（玩家和敌人）
- [x] 玩家死亡重生机制
- [x] 难度递增系统

### 🔄 待扩展功能
- [ ] 粒子特效和动画
- [ ] 音效和背景音乐
- [ ] UI界面（血量条、分数等）
- [ ] 更多敌人类型
- [ ] 道具和升级系统
- [ ] 关卡和波次系统

## 🎯 标签约定
- **Player** - 玩家对象
- **Enemy** - 敌人对象
- **Sword** - 飞剑对象（可选）

## 🚀 性能优化建议
1. 使用对象池管理敌人和飞剑
2. 限制同时存在的敌人数量
3. 定期清理已销毁的引用
4. 使用事件系统减少Update调用

## 🐛 已知问题
- 需要在Unity编辑器中手动配置预制体
- 输入动作需要重新生成以匹配项目设置
- 某些组件可能需要在Inspector中配置引用
