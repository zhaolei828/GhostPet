@echo off
chcp 65001 > nul
echo 🎮 启动Unity游戏测试服务器...
echo.

REM 检查Python是否可用
python --version > nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ 未找到Python，尝试使用Node.js...
    npx --version > nul 2>&1
    if %errorlevel% neq 0 (
        echo ❌ 未找到Node.js，请安装Python或Node.js
        echo 💡 或者双击WebGL-Test文件夹中的index.html直接打开浏览器测试
        pause
        exit
    ) else (
        echo ✅ 使用Node.js启动服务器...
        goto :nodejs
    )
) else (
    echo ✅ 使用Python启动服务器...
    goto :python
)

:python
echo 📱 手机访问地址：
for /f "tokens=2 delims=:" %%i in ('ipconfig ^| findstr /i "IPv4" ^| findstr /v "127.0.0.1"') do (
    for /f "tokens=1" %%j in ("%%i") do (
        echo    http://%%j:8000
    )
)
echo.
echo ⚠️  确保手机和电脑在同一WiFi
echo 💡 按Ctrl+C停止服务器
echo ==========================================
cd WebGL-Test
python -m http.server 8000
goto :end

:nodejs
echo 📱 手机访问地址：
for /f "tokens=2 delims=:" %%i in ('ipconfig ^| findstr /i "IPv4" ^| findstr /v "127.0.0.1"') do (
    for /f "tokens=1" %%j in ("%%i") do (
        echo    http://%%j:8000
    )
)
echo.
echo ⚠️  确保手机和电脑在同一WiFi
echo 💡 按Ctrl+C停止服务器
echo ==========================================
cd WebGL-Test
npx http-server . -p 8000
goto :end

:end
pause
