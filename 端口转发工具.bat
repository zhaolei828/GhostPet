@echo off
echo 🔧 Unity端口转发工具
echo ==========================================
echo.
echo 此工具将49408端口转发到8080端口供手机访问
echo.
echo ⚠️  需要管理员权限运行
echo 📱 转发后手机访问: http://你的IP:8080
echo.
pause

echo 正在设置端口转发...
netsh interface portproxy add v4tov4 listenport=8080 listenaddress=0.0.0.0 connectport=49408 connectaddress=127.0.0.1

if %errorlevel% equ 0 (
    echo ✅ 端口转发设置成功！
    echo 📱 手机访问地址:
    for /f "tokens=2 delims=:" %%i in ('ipconfig ^| findstr /i "IPv4" ^| findstr /v "127.0.0.1"') do (
        for /f "tokens=1" %%j in ("%%i") do (
            echo    http://%%j:8080
        )
    )
    echo.
    echo 🛑 要停止转发，请运行:
    echo    netsh interface portproxy delete v4tov4 listenport=8080 listenaddress=0.0.0.0
) else (
    echo ❌ 设置失败，请以管理员身份运行此脚本
)

echo.
pause
