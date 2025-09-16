using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// UI分辨率测试工具 - 验证UI在不同分辨率下的稳定性
/// </summary>
public class UIResolutionTester : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableAutoTest = false;
    [SerializeField] private bool enableDebugLog = true;
    [SerializeField] private float testDuration = 2f; // 每个分辨率的测试时长
    
    [Header("测试分辨率列表")]
    [SerializeField] private List<Vector2Int> testResolutions = new List<Vector2Int>
    {
        new Vector2Int(640, 480),   // 4:3 经典
        new Vector2Int(800, 600),   // 4:3 SVGA
        new Vector2Int(1024, 768),  // 4:3 XGA
        new Vector2Int(1280, 720),  // 16:9 HD
        new Vector2Int(1366, 768),  // 16:9 常见笔记本
        new Vector2Int(1920, 1080), // 16:9 Full HD
        new Vector2Int(2560, 1440), // 16:9 2K
        new Vector2Int(360, 640),   // 9:16 手机竖屏
        new Vector2Int(414, 896),   // iPhone 11 竖屏
        new Vector2Int(768, 1024),  // iPad 竖屏
    };
    
    // 测试状态
    private bool testInProgress = false;
    private int currentTestIndex = 0;
    private Vector2Int originalResolution;
    private bool originalFullscreen;
    
    // 测试结果
    private Dictionary<Vector2Int, UITestResult> testResults = new Dictionary<Vector2Int, UITestResult>();
    
    /// <summary>
    /// UI测试结果
    /// </summary>
    [System.Serializable]
    public class UITestResult
    {
        public Vector2Int resolution;
        public bool layoutValid;
        public bool componentsVisible;
        public bool noOverlap;
        public bool withinBounds;
        public string errorMessage;
        public float testTime;
        
        public bool IsSuccess => layoutValid && componentsVisible && noOverlap && withinBounds && string.IsNullOrEmpty(errorMessage);
    }
    
    private void Start()
    {
        // 记录原始分辨率
        originalResolution = new Vector2Int(Screen.width, Screen.height);
        originalFullscreen = Screen.fullScreen;
        
        if (enableAutoTest)
        {
            StartCoroutine(RunAutoTest());
        }
    }
    
    /// <summary>
    /// 开始自动测试
    /// </summary>
    public void StartAutoTest()
    {
        if (!testInProgress)
        {
            StartCoroutine(RunAutoTest());
        }
    }
    
    /// <summary>
    /// 运行自动测试协程
    /// </summary>
    private IEnumerator RunAutoTest()
    {
        testInProgress = true;
        testResults.Clear();
        
        Debug.Log("[UIResolutionTester] 开始UI分辨率稳定性测试...");
        
        for (int i = 0; i < testResolutions.Count; i++)
        {
            currentTestIndex = i;
            Vector2Int resolution = testResolutions[i];
            
            Debug.Log($"[UIResolutionTester] 测试分辨率: {resolution.x}x{resolution.y} ({i + 1}/{testResolutions.Count})");
            
            // 应用分辨率
            SetResolution(resolution);
            
            // 等待UI适配
            yield return new WaitForSeconds(0.5f);
            
            // 执行测试
            UITestResult result = TestCurrentResolution(resolution);
            testResults[resolution] = result;
            
            // 等待观察时间
            yield return new WaitForSeconds(testDuration);
        }
        
        // 恢复原始分辨率
        RestoreOriginalResolution();
        
        // 生成测试报告
        GenerateTestReport();
        
        testInProgress = false;
        Debug.Log("[UIResolutionTester] UI分辨率测试完成");
    }
    
    /// <summary>
    /// 设置分辨率
    /// </summary>
    private void SetResolution(Vector2Int resolution)
    {
        Screen.SetResolution(resolution.x, resolution.y, false);
        
        if (enableDebugLog)
            Debug.Log($"[UIResolutionTester] 已设置分辨率: {resolution.x}x{resolution.y}");
    }
    
    /// <summary>
    /// 恢复原始分辨率
    /// </summary>
    private void RestoreOriginalResolution()
    {
        Screen.SetResolution(originalResolution.x, originalResolution.y, originalFullscreen);
        
        if (enableDebugLog)
            Debug.Log($"[UIResolutionTester] 已恢复原始分辨率: {originalResolution.x}x{originalResolution.y}");
    }
    
    /// <summary>
    /// 测试当前分辨率下的UI
    /// </summary>
    private UITestResult TestCurrentResolution(Vector2Int resolution)
    {
        float startTime = Time.time;
        UITestResult result = new UITestResult
        {
            resolution = resolution,
            layoutValid = true,
            componentsVisible = true,
            noOverlap = true,
            withinBounds = true,
            errorMessage = ""
        };
        
        try
        {
            // 获取UIManager实例
            UIManager uiManager = UIManager.Instance;
            if (uiManager == null)
            {
                result.errorMessage = "UIManager实例未找到";
                result.layoutValid = false;
                return result;
            }
            
            // 检查UI基本组件是否存在
            if (uiManager.gameObject == null)
            {
                result.errorMessage = "UI管理器未正确初始化";
                result.layoutValid = false;
                return result;
            }
            
            // 测试UI组件的可见性和位置
            TestUIComponents(result);
            
            // 测试Canvas缩放
            TestCanvasScaling(result);
            
            // 测试UI边界
            TestUIBounds(result);
            
        }
        catch (System.Exception ex)
        {
            result.errorMessage = $"测试过程中发生异常: {ex.Message}";
            result.layoutValid = false;
        }
        
        result.testTime = Time.time - startTime;
        
        if (enableDebugLog)
        {
            string status = result.IsSuccess ? "通过" : "失败";
            Debug.Log($"[UIResolutionTester] {resolution.x}x{resolution.y} 测试{status}");
            if (!result.IsSuccess)
            {
                Debug.LogWarning($"[UIResolutionTester] 失败原因: {result.errorMessage}");
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// 测试UI组件
    /// </summary>
    private void TestUIComponents(UITestResult result)
    {
        // 查找关键UI组件
        PlayerHealthBar healthBar = FindFirstObjectByType<PlayerHealthBar>();
        ScoreUI scoreUI = FindFirstObjectByType<ScoreUI>();
        SwordStatusUI swordStatusUI = FindFirstObjectByType<SwordStatusUI>();
        
        // 测试血量条
        if (healthBar != null)
        {
            RectTransform healthRect = healthBar.GetComponent<RectTransform>();
            if (healthRect != null)
            {
                if (!IsRectTransformValid(healthRect))
                {
                    result.componentsVisible = false;
                    result.errorMessage += "血量条位置异常; ";
                }
            }
        }
        
        // 测试分数UI
        if (scoreUI != null)
        {
            RectTransform scoreRect = scoreUI.GetComponent<RectTransform>();
            if (scoreRect != null)
            {
                if (!IsRectTransformValid(scoreRect))
                {
                    result.componentsVisible = false;
                    result.errorMessage += "分数UI位置异常; ";
                }
            }
        }
        
        // 测试飞剑状态UI
        if (swordStatusUI != null)
        {
            RectTransform swordRect = swordStatusUI.GetComponent<RectTransform>();
            if (swordRect != null)
            {
                if (!IsRectTransformValid(swordRect))
                {
                    result.componentsVisible = false;
                    result.errorMessage += "飞剑状态UI位置异常; ";
                }
            }
        }
    }
    
    /// <summary>
    /// 检查RectTransform是否有效
    /// </summary>
    private bool IsRectTransformValid(RectTransform rectTransform)
    {
        if (rectTransform == null) return false;
        
        // 检查位置是否为NaN或无穷大
        Vector3 position = rectTransform.anchoredPosition;
        if (float.IsNaN(position.x) || float.IsNaN(position.y) || 
            float.IsInfinity(position.x) || float.IsInfinity(position.y))
        {
            return false;
        }
        
        // 检查尺寸是否合理
        Vector2 size = rectTransform.sizeDelta;
        if (size.x <= 0 || size.y <= 0 || size.x > 5000 || size.y > 5000)
        {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 测试Canvas缩放
    /// </summary>
    private void TestCanvasScaling(UITestResult result)
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        foreach (Canvas canvas in canvases)
        {
            if (canvas.isRootCanvas)
            {
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    // 检查缩放因子是否合理
                    if (scaler.scaleFactor <= 0 || scaler.scaleFactor > 10)
                    {
                        result.layoutValid = false;
                        result.errorMessage += $"Canvas缩放因子异常: {scaler.scaleFactor}; ";
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 测试UI边界
    /// </summary>
    private void TestUIBounds(UITestResult result)
    {
        // 获取屏幕边界
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // 查找所有UI元素
        RectTransform[] allRects = FindObjectsByType<RectTransform>(FindObjectsSortMode.None);
        
        foreach (RectTransform rect in allRects)
        {
            if (rect.gameObject.activeInHierarchy && IsUIElement(rect))
            {
                // 转换到屏幕坐标
                Vector3[] corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                
                // 检查是否超出屏幕边界（允许一定容差）
                for (int i = 0; i < corners.Length; i++)
                {
                    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);
                    
                    if (screenPoint.x < -100 || screenPoint.x > screenWidth + 100 ||
                        screenPoint.y < -100 || screenPoint.y > screenHeight + 100)
                    {
                        result.withinBounds = false;
                        result.errorMessage += $"UI元素 {rect.name} 超出屏幕边界; ";
                        break;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 判断是否为UI元素
    /// </summary>
    private bool IsUIElement(RectTransform rectTransform)
    {
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        return canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay;
    }
    
    /// <summary>
    /// 生成测试报告
    /// </summary>
    private void GenerateTestReport()
    {
        Debug.Log("=== UI分辨率测试报告 ===");
        
        int passCount = 0;
        int totalCount = testResults.Count;
        
        foreach (var kvp in testResults)
        {
            Vector2Int resolution = kvp.Key;
            UITestResult result = kvp.Value;
            
            string status = result.IsSuccess ? "✓ 通过" : "✗ 失败";
            Debug.Log($"{resolution.x}x{resolution.y}: {status} (耗时: {result.testTime:F2}s)");
            
            if (!result.IsSuccess)
            {
                Debug.LogWarning($"  失败原因: {result.errorMessage}");
            }
            
            if (result.IsSuccess) passCount++;
        }
        
        float successRate = totalCount > 0 ? (float)passCount / totalCount * 100f : 0f;
        
        Debug.Log($"=== 测试总结 ===");
        Debug.Log($"测试总数: {totalCount}");
        Debug.Log($"通过数量: {passCount}");
        Debug.Log($"失败数量: {totalCount - passCount}");
        Debug.Log($"成功率: {successRate:F1}%");
        
        if (successRate >= 90f)
        {
            Debug.Log("✓ UI分辨率兼容性测试评估: 优秀");
        }
        else if (successRate >= 70f)
        {
            Debug.Log("⚠ UI分辨率兼容性测试评估: 良好");
        }
        else
        {
            Debug.LogWarning("✗ UI分辨率兼容性测试评估: 需要改进");
        }
    }
    
    /// <summary>
    /// 手动测试指定分辨率
    /// </summary>
    [ContextMenu("测试当前分辨率")]
    public void TestCurrentResolution()
    {
        Vector2Int currentRes = new Vector2Int(Screen.width, Screen.height);
        UITestResult result = TestCurrentResolution(currentRes);
        
        Debug.Log($"当前分辨率 {currentRes.x}x{currentRes.y} 测试结果: {(result.IsSuccess ? "通过" : "失败")}");
        if (!result.IsSuccess)
        {
            Debug.LogWarning($"失败原因: {result.errorMessage}");
        }
    }
    
    /// <summary>
    /// 获取测试结果
    /// </summary>
    public Dictionary<Vector2Int, UITestResult> GetTestResults()
    {
        return new Dictionary<Vector2Int, UITestResult>(testResults);
    }
    
    /// <summary>
    /// 是否正在测试
    /// </summary>
    public bool IsTestInProgress => testInProgress;
    
    private void OnDestroy()
    {
        // 确保恢复原始分辨率
        if (testInProgress)
        {
            RestoreOriginalResolution();
        }
    }
}
