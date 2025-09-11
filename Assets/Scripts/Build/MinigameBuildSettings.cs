using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

/// <summary>
/// 小游戏构建设置 - 一键配置WebGL构建参数
/// </summary>
public class MinigameBuildSettings : EditorWindow
{
    [MenuItem("Build/小游戏构建设置")]
    public static void ShowWindow()
    {
        GetWindow<MinigameBuildSettings>("小游戏构建设置");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("小游戏平台构建设置", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("配置微信小游戏设置", GUILayout.Height(40)))
        {
            ConfigureWeChatMinigame();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("配置抖音小游戏设置", GUILayout.Height(40)))
        {
            ConfigureDouyinMinigame();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("立即构建WebGL", GUILayout.Height(40)))
        {
            BuildWebGL();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "构建前请确保：\n" +
            "1. 已安装WebGL构建支持\n" +
            "2. 场景已添加到Build Settings\n" +
            "3. 已配置移动端分辨率", 
            MessageType.Info);
    }
    
    /// <summary>
    /// 配置微信小游戏设置
    /// </summary>
    private void ConfigureWeChatMinigame()
    {
        Debug.Log("正在配置微信小游戏设置...");
        
        // 切换到WebGL平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        
        // 设置玩家设置
        PlayerSettings.productName = "GhostPet";
        PlayerSettings.companyName = "YourCompany";
        
        // WebGL设置
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.memorySize = 32; // 32MB内存
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.dataCaching = false;
        PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
        
        // 分辨率设置（微信小游戏推荐）
        PlayerSettings.defaultScreenWidth = 360;
        PlayerSettings.defaultScreenHeight = 640;
        PlayerSettings.runInBackground = true;
        
        // 优化设置
        PlayerSettings.stripEngineCode = true;
        // 使用正确的API设置脚本后端
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);
        
        // 质量设置
        QualitySettings.SetQualityLevel(0); // 最低质量
        
        Debug.Log("微信小游戏设置配置完成！");
        EditorUtility.DisplayDialog("配置完成", "微信小游戏设置已配置完成！\n分辨率: 360x640\n压缩: Gzip\n内存: 32MB", "确定");
    }
    
    /// <summary>
    /// 配置抖音小游戏设置
    /// </summary>
    private void ConfigureDouyinMinigame()
    {
        Debug.Log("正在配置抖音小游戏设置...");
        
        // 切换到WebGL平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        
        // 设置玩家设置
        PlayerSettings.productName = "GhostPet";
        PlayerSettings.companyName = "YourCompany";
        
        // WebGL设置
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.memorySize = 64; // 64MB内存（抖音限制更宽松）
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.dataCaching = false;
        PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
        
        // 分辨率设置（抖音小游戏推荐）
        PlayerSettings.defaultScreenWidth = 375;
        PlayerSettings.defaultScreenHeight = 667;
        PlayerSettings.runInBackground = true;
        
        // 优化设置
        PlayerSettings.stripEngineCode = true;
        // 使用正确的API设置脚本后端
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);
        
        // 质量设置
        QualitySettings.SetQualityLevel(1); // 中等质量
        
        Debug.Log("抖音小游戏设置配置完成！");
        EditorUtility.DisplayDialog("配置完成", "抖音小游戏设置已配置完成！\n分辨率: 375x667\n压缩: Brotli\n内存: 64MB", "确定");
    }
    
    /// <summary>
    /// 构建WebGL
    /// </summary>
    private void BuildWebGL()
    {
        string buildPath = EditorUtility.SaveFolderPanel("选择构建输出目录", "", "");
        
        if (string.IsNullOrEmpty(buildPath))
        {
            Debug.Log("构建取消");
            return;
        }
        
        Debug.Log($"开始构建WebGL到: {buildPath}");
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;
        
        var result = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        if (result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("WebGL构建成功！");
            EditorUtility.DisplayDialog("构建成功", $"WebGL构建成功！\n输出目录: {buildPath}", "确定");
            
            // 打开构建目录
            EditorUtility.RevealInFinder(buildPath);
        }
        else
        {
            Debug.LogError("WebGL构建失败！");
            EditorUtility.DisplayDialog("构建失败", "WebGL构建失败，请查看Console窗口了解详情。", "确定");
        }
    }
}
#endif