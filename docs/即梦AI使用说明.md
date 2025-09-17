# GhostPet即梦AI素材生成指南

## 🚀 快速开始

### 第一步：获取sessionid
1. 访问 [即梦AI官网](https://jimeng.jianying.com/)
2. 登录你的账户
3. 按 `F12` 打开开发者工具
4. 找到 `Application` > `Cookies`
5. 复制 `sessionid` 的值

### 第二步：配置和启动服务

1. **启动API服务**：
```bash
npm start
```
服务将在 http://localhost:8000 启动

2. **修改Python脚本**：
编辑 `test_generate_images.py` 文件，将 `SESSIONID` 替换为你的真实sessionid：
```python
SESSIONID = "你复制的sessionid值"
```

### 第三步：生成素材

运行Python脚本：
```bash
python test_generate_images.py
```

## 🎮 可生成的游戏素材

### UI界面元素
- ✅ 血量条框架 (health_bar_frame)
- ✅ 血量填充液体 (health_fill) 
- ✅ 分数面板 (score_panel)
- ✅ 飞剑状态面板 (sword_status_panel)
- ✅ 暂停菜单背景 (pause_menu_bg)

### 游戏角色
- ✅ 基础鬼怪敌人 (basic_ghost)
- ✅ 强力鬼怪敌人 (strong_ghost)
- ✅ 玩家角色 (player_character)

### 武器道具
- ✅ 飞剑主体 (flying_sword)
- ✅ 飞剑轨迹效果 (sword_trail)
- ✅ 命中特效 (hit_effect)

## 📁 文件输出结构

```
Assets/GeneratedAssets/
├── ui_elements/
│   ├── health_bar_frame_20250917_160000.jpg
│   ├── health_fill_20250917_160100.jpg
│   └── ...
├── characters/
│   ├── basic_ghost_20250917_160200.jpg
│   ├── strong_ghost_20250917_160300.jpg
│   └── ...
└── weapons/
    ├── flying_sword_20250917_160400.jpg
    ├── sword_trail_20250917_160500.jpg
    └── ...
```

## 🔧 脚本功能选项

运行脚本后，你可以选择：

1. **生成单张测试图片** - 快速测试API连接
2. **批量生成所有游戏素材** - 一次性生成全部素材
3. **只生成UI元素** - 仅生成界面相关素材
4. **只生成角色素材** - 仅生成游戏角色
5. **只生成武器素材** - 仅生成武器和特效

## ⚙️ 高级配置

### 修改生成参数
在 `test_generate_images.py` 中可以调整：

```python
# 图片尺寸
width=1024, height=1024

# 生成模型 
model="jimeng-3.0"  # 或 jimeng-2.1, jimeng-2.0-pro 等

# 精细度
sample_strength=0.5  # 范围 0-1

# 生成间隔（避免频率限制）
delay=2  # 秒
```

### 自定义提示词
你可以在 `GAME_PROMPTS` 字典中添加新的素材类型和提示词。

## 🚨 注意事项

1. **免费限制**：即梦AI每日赠送66积分，可生成66次
2. **请求频率**：建议在生成之间等待2-3秒，避免被限制
3. **仅供学习**：该API仅供测试学习使用，商用请使用官方API
4. **素材后处理**：生成的图片可能需要在Photoshop等工具中进一步处理以适配游戏

## 🔗 相关链接

- [即梦AI官网](https://jimeng.jianying.com/)
- [项目GitHub](https://github.com/LLM-Red-Team/jimeng-free-api)
- [火山引擎官方API](https://developer.volcengine.com/)

## 📞 问题排查

### API连接失败
- 检查服务是否启动：`npm start`
- 检查端口是否被占用：`netstat -an | findstr 8000`

### 生成失败
- 验证sessionid是否正确
- 检查即梦账户积分是否足够
- 确认网络连接正常

### 文件保存失败
- 检查输出目录权限
- 确保磁盘空间充足
