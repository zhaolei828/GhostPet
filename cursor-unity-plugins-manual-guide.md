# Cursor Unity开发插件安装指南

## 方法一：安装Cursor命令行工具（推荐）

### 步骤1：安装Cursor命令行工具
1. 打开Cursor编辑器
2. 按 `Cmd+Shift+P` 打开命令面板
3. 输入 "Shell Command: Install 'cursor' command in PATH"
4. 选择并执行该命令
5. 重启终端

### 步骤2：运行自动安装脚本
安装完命令行工具后，在终端中运行：
```bash
cd /Users/zhaolei/Unity/Projects/GhostPet
./setup-cursor-unity-plugins.ps1
```

## 方法二：手动安装插件

### Unity开发核心插件
在Cursor中按 `Cmd+Shift+X` 打开插件市场，搜索并安装以下插件：

#### 🎮 Unity核心插件
- **Unity Tools for Visual Studio Code** (`visualstudiotoolsforunity.vstuc`)
  - Unity官方插件，提供调试和智能感知
- **Unity Tools** (`tobiah.unity-tools`)
  - 快速创建Unity脚本和组件
- **Unity Code Snippets** (`kleber-swf.unity-code-snippets`)
  - Unity常用代码片段

#### 💻 C#开发支持
- **C#** (`ms-dotnettools.csharp`)
  - C#官方语言支持
- **C# Extensions** (`kreativ-software.csharpextensions`)
  - C#扩展功能
- **.NET Core Test Explorer** (`formulahendry.dotnet-test-explorer`)
  - .NET测试资源管理器

#### 🔍 代码质量工具
- **Spell Checker** (`streetsidesoftware.code-spell-checker`)
  - 英语拼写检查
- **Prettier** (`esbenp.prettier-vscode`)
  - 代码格式化
- **Better Comments** (`aaron-bond.better-comments`)
  - 增强注释显示

#### 🔄 版本控制
- **GitLens** (`eamodio.gitlens`)
  - Git增强功能
- **Git History** (`donjayamanne.githistory`)
  - Git历史查看器
- **TODO Tree** (`gruntfuggly.todo-tree`)
  - TODO任务树状图

#### 🎨 UI增强
- **Material Icon Theme** (`pkief.material-icon-theme`)
  - 美观的文件图标
- **Material Theme** (`equinusocio.vsc-material-theme`)
  - 材质设计主题
- **Indent Rainbow** (`oderwat.indent-rainbow`)
  - 缩进彩虹显示

## Unity编辑器配置

### 设置Cursor为Unity外部编辑器
1. 在Unity中打开 `Edit > Preferences > External Tools`
2. 将 `External Script Editor` 设置为Cursor的路径：
   - macOS: `/Applications/Cursor.app/Contents/MacOS/Cursor`
3. 确保选中 `Regenerate project files`
4. 点击 `Regenerate project files` 按钮

### 推荐的Unity项目设置
1. `Edit > Project Settings > Player`
   - 设置 Company Name 和 Product Name
2. `Edit > Project Settings > Editor`
   - Asset Serialization Mode = Force Text
   - Version Control Mode = Visible Meta Files
3. `Edit > Preferences > GI Cache`
   - 清理旧的光照缓存

## Cursor设置优化

### 推荐的settings.json配置
在Cursor中按 `Cmd+,` 打开设置，添加以下配置：

```json
{
    "editor.formatOnSave": true,
    "editor.codeActionsOnSave": {
        "source.organizeImports": true
    },
    "files.exclude": {
        "**/Library/**": true,
        "**/Temp/**": true,
        "**/Obj/**": true,
        "**/Build/**": true,
        "**/Builds/**": true,
        "**/Logs/**": true,
        "**/.DS_Store": true,
        "**/Thumbs.db": true
    },
    "omnisharp.useGlobalMono": "never",
    "omnisharp.useModernNet": true,
    "dotnet.completion.showCompletionItemsFromUnimportedNamespaces": true
}
```

## AI编程助手使用技巧

### Cursor AI功能
- **Ctrl+K**: AI代码生成
- **Ctrl+L**: AI聊天助手
- **Tab**: 接受AI建议
- **Cmd+I**: 内联AI编辑

### Unity特定的AI提示词示例
```
// 生成一个简单的移动脚本
"Create a Unity PlayerController script with WASD movement"

// 生成UI管理器
"Create a Unity UI manager for main menu with play, settings, and quit buttons"

// 生成对象池
"Create a Unity object pool system for bullets"
```

## 故障排除

### 常见问题
1. **IntelliSense不工作**
   - 确保安装了C#插件
   - 重启Cursor
   - 在Unity中点击 `Assets > Reimport All`

2. **调试器无法连接**
   - 确保Unity和Cursor都在运行
   - 检查Unity Console是否有错误
   - 重新生成项目文件

3. **代码补全缓慢**
   - 排除Unity的临时文件夹
   - 增加OmniSharp的内存限制
   - 清理Unity缓存

## 完成安装后的验证

1. 在Unity中创建一个新的C#脚本
2. 双击脚本，应该在Cursor中打开
3. 输入 `public class` 应该看到智能提示
4. 尝试使用 `Ctrl+K` 生成一些Unity代码

🎉 恭喜！您现在拥有了一个功能完整的Unity-Cursor开发环境！
