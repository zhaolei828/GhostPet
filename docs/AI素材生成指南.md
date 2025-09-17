# GhostPet AI素材生成指南

## 📋 目录
- [概述](#概述)
- [即梦AI使用流程](#即梦ai使用流程)
- [环境准备](#环境准备)
- [素材生成工具](#素材生成工具)
- [Unity集成方案](#unity集成方案)
- [常见问题解决](#常见问题解决)
- [素材质量优化](#素材质量优化)

## 🎯 概述

本指南介绍如何使用即梦AI为GhostPet项目生成游戏素材，包括角色、武器、敌人和UI元素的创建流程。

### 项目架构
```
GhostPet/
├── jimeng-free-api/           # 即梦AI本地API服务
│   ├── test_generate_images.py
│   └── generate_transparent_assets.py
├── Assets/GeneratedAssets/    # AI生成的素材目录
│   ├── characters/           # 角色素材
│   ├── weapons/             # 武器素材
│   └── ui_elements/         # UI素材
└── docs/                    # 文档目录
    ├── 即梦AI使用说明.md
    └── 即梦AI-SESSIONID获取教程.md
```

## 🚀 即梦AI使用流程

### 1. 获取SESSIONID
详细步骤请参考：[即梦AI-SESSIONID获取教程.md](./即梦AI-SESSIONID获取教程.md)

**快速步骤：**
1. 访问 https://jimeng.jianying.com/ai-tool/generate
2. 登录账户
3. 打开浏览器开发者工具 (F12)
4. 在Network/网络标签页中找到请求
5. 复制Cookie中的sessionid值

### 2. 启动本地API服务
```bash
cd jimeng-free-api
npm install
npm start
```

### 3. 配置SESSIONID
修改生成脚本中的SESSIONID变量：
```python
SESSIONID = "你的sessionid值"
```

## ⚙️ 环境准备

### 系统要求
- Node.js 16+
- Python 3.7+
- Git
- Unity 2022.3+

### 依赖安装
```bash
# Node.js依赖
cd jimeng-free-api
npm install

# Python依赖（如需要）
pip install requests
```

## 🎨 素材生成工具

### 基础素材生成
使用 `test_generate_images.py` 生成基础游戏素材：

```bash
cd jimeng-free-api
python test_generate_images.py
```

**生成内容：**
- 角色：玩家、基础鬼怪、强力鬼怪
- 武器：飞剑、剑迹、打击效果
- UI：血条框、分数面板、暂停菜单

### 透明背景素材生成
使用 `generate_transparent_assets.py` 生成优化的透明背景素材：

```bash
cd jimeng-free-api
python generate_transparent_assets.py
```

**优化特点：**
- 强调透明背景
- 清洁的边缘轮廓
- 适合游戏使用的尺寸

### 提示词模板

#### 角色类素材
```
mystical warrior, ancient chinese robes, floating meditation pose, 
ethereal aura, controlling flying swords, 2D game sprite, 
transparent background, no background, cutout style, PNG format
```

#### 武器类素材
```
elegant chinese flying sword, silver blade, gold hilt, 
mystical runes, glowing edge, weapon asset, 2D game sprite, 
transparent background, no background, cutout style, PNG format
```

#### UI类素材
```
ancient chinese style health bar frame, wooden texture with gold inlay, 
jade corners, empty health container, traditional border design, 
transparent background, game UI asset, PNG format
```

## 🎮 Unity集成方案

### 自动化素材更新
项目包含 `RuntimeAssetUpdater.cs` 脚本，可在运行时自动更新游戏对象的素材。

**使用方法：**
1. 将脚本添加到场景中的任意GameObject
2. 在Inspector中分配生成的素材
3. 运行游戏时自动应用到相关对象

### 手动素材替换
1. 将生成的图片导入Unity
2. 设置Texture Type为 "Sprite (2D and UI)"
3. 调整Pixels Per Unit适配游戏尺寸
4. 替换Prefab或GameObject的Sprite属性

### 素材命名规范
```
角色素材：player_character_YYYYMMDD_HHMMSS.jpg
武器素材：flying_sword_YYYYMMDD_HHMMSS.jpg
敌人素材：basic_ghost_YYYYMMDD_HHMMSS.jpg
UI素材：health_bar_frame_YYYYMMDD_HHMMSS.jpg
```

## 🔧 常见问题解决

### API连接问题
```bash
# 检查服务状态
curl http://localhost:8000/ping

# 重启服务
npm start
```

### SESSIONID过期
- 重新登录即梦AI网站
- 获取新的sessionid
- 更新脚本配置

### 素材质量问题
- **背景不透明**：使用`generate_transparent_assets.py`
- **尺寸不合适**：在Unity中调整localScale
- **风格不统一**：使用一致的提示词模板

### Unity导入问题
- 检查图片格式设置
- 确认Sprite模式正确
- 验证像素密度配置

## 🎨 素材质量优化

### 提示词优化技巧
1. **明确背景要求**：`transparent background`, `no background`, `cutout style`
2. **统一艺术风格**：`ancient chinese style`, `2D game sprite`, `mystical`
3. **指定格式**：`PNG format`, `game asset`
4. **避免复杂场景**：使用负面提示词排除背景元素

### 后期处理建议
1. **背景移除**：使用图像编辑软件去除残留背景
2. **尺寸调整**：统一素材尺寸规格
3. **格式转换**：JPG转PNG保持透明度
4. **压缩优化**：平衡文件大小和质量

### 批量处理工具
```python
# 示例：批量转换和优化
from PIL import Image
import os

def optimize_sprite(input_path, output_path):
    img = Image.open(input_path)
    img = img.convert("RGBA")  # 确保透明通道
    img.save(output_path, "PNG", optimize=True)
```

## 📚 相关文档

- [即梦AI使用说明.md](./即梦AI使用说明.md) - 详细的即梦AI使用指南
- [即梦AI-SESSIONID获取教程.md](./即梦AI-SESSIONID获取教程.md) - SESSIONID获取的图文教程
- [Unity编辑器设置指南.md](./Unity编辑器设置指南.md) - Unity项目配置
- [项目开发规则.md](./项目开发规则.md) - 开发规范和测试要求

## 🔄 更新日志

### 2025-09-17
- ✅ 创建基础素材生成工具
- ✅ 实现透明背景优化
- ✅ 添加Unity自动更新脚本
- ✅ 完善文档体系

### 下一步计划
- 🔲 PNG格式素材生成
- 🔲 批量素材处理工具
- 🔲 AI素材质量评估
- 🔲 自动化测试集成

---

> 💡 **提示**: 生成高质量游戏素材需要反复调试提示词和参数。建议先从小批量测试开始，逐步优化生成效果。
