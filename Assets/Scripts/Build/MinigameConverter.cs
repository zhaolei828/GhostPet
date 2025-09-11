using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// 小游戏转换器 - 将WebGL构建转换为小游戏格式
/// </summary>
public class MinigameConverter : EditorWindow
{
    private string webglBuildPath = "";
    private string outputPath = "";
    
    [MenuItem("Build/小游戏转换器")]
    public static void ShowWindow()
    {
        GetWindow<MinigameConverter>("小游戏转换器");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("WebGL转小游戏工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // WebGL构建路径选择
        EditorGUILayout.LabelField("步骤1: 选择WebGL构建目录");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("WebGL路径:", webglBuildPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            webglBuildPath = EditorUtility.OpenFolderPanel("选择WebGL构建目录", "", "");
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 输出路径选择
        EditorGUILayout.LabelField("步骤2: 选择小游戏输出目录");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("输出路径:", outputPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            outputPath = EditorUtility.SaveFolderPanel("选择小游戏输出目录", "", "");
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 转换按钮
        GUI.enabled = !string.IsNullOrEmpty(webglBuildPath) && !string.IsNullOrEmpty(outputPath);
        if (GUILayout.Button("转换为微信小游戏", GUILayout.Height(40)))
        {
            ConvertToWeChatMinigame();
        }
        
        if (GUILayout.Button("转换为抖音小游戏", GUILayout.Height(40)))
        {
            ConvertToDouyinMinigame();
        }
        GUI.enabled = true;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "转换流程：\n" +
            "1. 先构建WebGL版本\n" +
            "2. 选择WebGL构建目录\n" +
            "3. 选择小游戏输出目录\n" +
            "4. 点击转换按钮", 
            MessageType.Info);
    }
    
    /// <summary>
    /// 转换为微信小游戏
    /// </summary>
    private void ConvertToWeChatMinigame()
    {
        try
        {
            Debug.Log("开始转换为微信小游戏...");
            
            // 创建小游戏目录结构
            CreateMinigameStructure(outputPath);
            
            // 复制WebGL文件
            CopyWebGLFiles(webglBuildPath, Path.Combine(outputPath, "webgl"));
            
            // 创建微信小游戏配置文件
            CreateWeChatConfigs(outputPath);
            
            Debug.Log("微信小游戏转换完成！");
            EditorUtility.DisplayDialog("转换完成", 
                $"微信小游戏转换完成！\n输出目录: {outputPath}\n\n下一步:\n1. 用微信开发者工具打开项目\n2. 预览和调试\n3. 上传发布", 
                "确定");
            
            EditorUtility.RevealInFinder(outputPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"转换失败: {e.Message}");
            EditorUtility.DisplayDialog("转换失败", $"转换过程中出现错误:\n{e.Message}", "确定");
        }
    }
    
    /// <summary>
    /// 转换为抖音小游戏
    /// </summary>
    private void ConvertToDouyinMinigame()
    {
        try
        {
            Debug.Log("开始转换为抖音小游戏...");
            
            // 创建小游戏目录结构
            CreateMinigameStructure(outputPath);
            
            // 复制WebGL文件
            CopyWebGLFiles(webglBuildPath, Path.Combine(outputPath, "webgl"));
            
            // 创建抖音小游戏配置文件
            CreateDouyinConfigs(outputPath);
            
            Debug.Log("抖音小游戏转换完成！");
            EditorUtility.DisplayDialog("转换完成", 
                $"抖音小游戏转换完成！\n输出目录: {outputPath}\n\n下一步:\n1. 用字节跳动开发者工具打开项目\n2. 预览和调试\n3. 上传发布", 
                "确定");
            
            EditorUtility.RevealInFinder(outputPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"转换失败: {e.Message}");
            EditorUtility.DisplayDialog("转换失败", $"转换过程中出现错误:\n{e.Message}", "确定");
        }
    }
    
    /// <summary>
    /// 创建小游戏目录结构
    /// </summary>
    private void CreateMinigameStructure(string basePath)
    {
        Directory.CreateDirectory(basePath);
        Directory.CreateDirectory(Path.Combine(basePath, "webgl"));
    }
    
    /// <summary>
    /// 复制WebGL文件
    /// </summary>
    private void CopyWebGLFiles(string sourcePath, string targetPath)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw new System.Exception($"WebGL构建目录不存在: {sourcePath}");
        }
        
        CopyDirectory(sourcePath, targetPath);
    }
    
    /// <summary>
    /// 递归复制目录
    /// </summary>
    private void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);
        
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string targetFile = Path.Combine(targetDir, fileName);
            File.Copy(file, targetFile, true);
        }
        
        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dir);
            string targetSubDir = Path.Combine(targetDir, dirName);
            CopyDirectory(dir, targetSubDir);
        }
    }
    
    /// <summary>
    /// 创建微信小游戏配置文件
    /// </summary>
    private void CreateWeChatConfigs(string basePath)
    {
        // game.json
        string gameJson = @"{
  ""deviceOrientation"": ""portrait"",
  ""showStatusBar"": false,
  ""networkTimeout"": {
    ""request"": 10000,
    ""downloadFile"": 10000
  },
  ""subpackages"": []
}";
        File.WriteAllText(Path.Combine(basePath, "game.json"), gameJson);
        
        // game.js
        string gameJs = @"// 微信小游戏入口文件
import './webgl/index.js';

// 适配微信小游戏环境
if (typeof wx !== 'undefined') {
    // 微信小游戏特定代码
    console.log('微信小游戏环境');
}";
        File.WriteAllText(Path.Combine(basePath, "game.js"), gameJs);
        
        // project.config.json
        string projectConfig = $@"{{
  ""description"": ""GhostPet微信小游戏"",
  ""setting"": {{
    ""urlCheck"": false,
    ""es6"": true,
    ""enhance"": true,
    ""postcss"": true,
    ""minified"": true,
    ""newFeature"": true
  }},
  ""compileType"": ""game"",
  ""libVersion"": ""game"",
  ""appid"": ""你的小游戏AppID"",
  ""projectname"": ""GhostPet"",
  ""isGameTourist"": false,
  ""condition"": {{}}
}}";
        File.WriteAllText(Path.Combine(basePath, "project.config.json"), projectConfig);
    }
    
    /// <summary>
    /// 创建抖音小游戏配置文件
    /// </summary>
    private void CreateDouyinConfigs(string basePath)
    {
        // game.json
        string gameJson = @"{
  ""deviceOrientation"": ""portrait"",
  ""showStatusBar"": false,
  ""networkTimeout"": {
    ""request"": 15000,
    ""downloadFile"": 15000
  }
}";
        File.WriteAllText(Path.Combine(basePath, "game.json"), gameJson);
        
        // game.js
        string gameJs = @"// 抖音小游戏入口文件
import './webgl/index.js';

// 适配抖音小游戏环境
if (typeof tt !== 'undefined') {
    // 抖音小游戏特定代码
    console.log('抖音小游戏环境');
}";
        File.WriteAllText(Path.Combine(basePath, "game.js"), gameJs);
        
        // project.config.json
        string projectConfig = $@"{{
  ""description"": ""GhostPet抖音小游戏"",
  ""setting"": {{
    ""urlCheck"": false,
    ""es6"": true,
    ""enhance"": true,
    ""postcss"": true,
    ""minified"": true
  }},
  ""compileType"": ""game"",
  ""libVersion"": ""game"",
  ""appid"": ""你的小游戏AppID"",
  ""projectname"": ""GhostPet""
}}";
        File.WriteAllText(Path.Combine(basePath, "project.config.json"), projectConfig);
    }
}
#endif
