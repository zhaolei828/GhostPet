#!/bin/bash

# Cursor Unity开发插件自动安装脚本
# 请先在Cursor中安装命令行工具: Cmd+Shift+P -> "Shell Command: Install 'cursor' command in PATH"

echo "🎮 开始为Cursor安装Unity开发插件..."

# 检查cursor命令是否可用
if ! command -v cursor &> /dev/null; then
    echo "❌ cursor命令未找到！"
    echo "请先在Cursor中安装命令行工具："
    echo "1. 打开Cursor"
    echo "2. 按Cmd+Shift+P"
    echo "3. 输入并执行: Shell Command: Install 'cursor' command in PATH"
    echo "4. 重启终端后再运行此脚本"
    exit 1
fi

# Unity开发核心插件
echo "📦 安装Unity开发核心插件..."
unity_plugins=(
    "visualstudiotoolsforunity.vstuc"
    "tobiah.unity-tools"
    "kleber-swf.unity-code-snippets"
)

for plugin in "${unity_plugins[@]}"; do
    echo "安装: $plugin"
    cursor --install-extension "$plugin"
done

# C#开发支持插件
echo "💻 安装C#开发支持插件..."
csharp_plugins=(
    "ms-dotnettools.csharp"
    "kreativ-software.csharpextensions"
    "formulahendry.dotnet-test-explorer"
)

for plugin in "${csharp_plugins[@]}"; do
    echo "安装: $plugin"
    cursor --install-extension "$plugin"
done

# 代码质量和调试工具
echo "🔍 安装代码质量工具..."
quality_plugins=(
    "streetsidesoftware.code-spell-checker"
    "esbenp.prettier-vscode"
    "aaron-bond.better-comments"
    "ms-vscode.hexeditor"
)

for plugin in "${quality_plugins[@]}"; do
    echo "安装: $plugin"
    cursor --install-extension "$plugin"
done

# 版本控制插件
echo "🔄 安装版本控制插件..."
git_plugins=(
    "eamodio.gitlens"
    "donjayamanne.githistory"
    "gruntfuggly.todo-tree"
)

for plugin in "${git_plugins[@]}"; do
    echo "安装: $plugin"
    cursor --install-extension "$plugin"
done

# UI增强插件
echo "🎨 安装UI增强插件..."
ui_plugins=(
    "pkief.material-icon-theme"
    "equinusocio.vsc-material-theme"
    "oderwat.indent-rainbow"
    "yzhang.markdown-all-in-one"
)

for plugin in "${ui_plugins[@]}"; do
    echo "安装: $plugin"
    cursor --install-extension "$plugin"
done

echo ""
echo "✅ 所有插件安装完成！"
echo ""
echo "📋 接下来的配置步骤："
echo "1. 重启Cursor编辑器"
echo "2. 在Unity中设置外部编辑器为Cursor"
echo "3. 查看详细配置指南: cursor-unity-plugins-manual-guide.md"
echo ""
echo "🎯 Unity编辑器配置："
echo "   Edit > Preferences > External Tools"
echo "   External Script Editor: /Applications/Cursor.app/Contents/MacOS/Cursor"
echo ""
echo "🚀 现在您可以享受增强的Unity开发体验了！"
