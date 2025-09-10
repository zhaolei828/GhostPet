# GhostPet Unity6 项目开发环境配置脚本
# 工欲善其事，必先利其器 

Write-Host "🎮 GhostPet 开发环境配置开始..." -ForegroundColor Green

# 1. 安装VS Code推荐扩展
Write-Host "`n📦 安装VS Code扩展..." -ForegroundColor Yellow
$extensions = @(
    "visualstudiotoolsforunity.vstuc",
    "ms-dotnettools.csharp", 
    "eamodio.gitlens",
    "gruntfuggly.todo-tree",
    "tobiah.unity-tools",
    "kleber-swf.unity-code-snippets"
)

foreach ($ext in $extensions) {
    Write-Host "安装扩展: $ext"
    code --install-extension $ext
}

# 2. 检查必要的开发工具
Write-Host "`n🔍 检查开发工具..." -ForegroundColor Yellow

# 检查Git配置
$gitUser = git config user.name
$gitEmail = git config user.email
if (-not $gitUser -or -not $gitEmail) {
    Write-Host "⚠️  建议配置Git用户信息:" -ForegroundColor Red
    Write-Host "git config --global user.name '你的姓名'"
    Write-Host "git config --global user.email '你的邮箱'"
}

# 3. Unity项目优化建议
Write-Host "`n⚙️  Unity编辑器配置建议:" -ForegroundColor Yellow
Write-Host "1. 打开 Edit > Project Settings > Player"
Write-Host "2. 设置 Company Name 和 Product Name"
Write-Host "3. 在 Edit > Preferences > External Tools 中设置外部编辑器为VS Code"
Write-Host "4. 启用 Edit > Project Settings > Editor > Asset Serialization Mode = Force Text"

# 4. 性能优化建议
Write-Host "`n🚀 性能优化建议:" -ForegroundColor Yellow
Write-Host "1. 启用 Edit > Project Settings > Editor > Compress Assets On Import"
Write-Host "2. 设置 Edit > Project Settings > Quality > Texture Quality = Full Res"
Write-Host "3. 配置 Window > Analysis > Profiler 进行性能分析"

Write-Host "`n✅ 开发环境配置完成！开始打造你的幽灵宠物游戏吧！" -ForegroundColor Green
Write-Host "💡 提示: 在Unity中打开项目，等待包管理器自动安装新包..." -ForegroundColor Cyan
