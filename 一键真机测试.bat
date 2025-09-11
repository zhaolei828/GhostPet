@echo off
echo ========================================
echo    Unity游戏快速真机测试工具
echo ========================================
echo.

echo 第1步: 请先在Unity中构建WebGL版本
echo   1. File → Build Settings
echo   2. 选择WebGL平台
echo   3. 点击Build，选择当前目录下的"WebGL-Build"文件夹
echo.
pause

echo 第2步: 启动本地服务器...
echo.

REM 检查是否存在WebGL构建
if not exist "WebGL-Build" (
    echo ❌ 没有找到WebGL-Build文件夹！
    echo 请先在Unity中构建WebGL版本到此目录
    pause
    exit
)

echo ✅ 发现WebGL构建文件
echo.

REM 获取本机IP地址
for /f "tokens=2 delims=:" %%i in ('ipconfig ^| findstr /i "IPv4"') do (
    for /f "tokens=1" %%j in ("%%i") do (
        set LOCAL_IP=%%j
        goto :found_ip
    )
)
:found_ip

echo 🌐 启动本地服务器...
echo 📱 请在手机浏览器中访问：
echo.
echo    http://%LOCAL_IP%:8000
echo.
echo ⚠️  确保手机和电脑在同一WiFi网络
echo 💡 按 Ctrl+C 停止服务器
echo.

cd WebGL-Build
python -m http.server 8000
