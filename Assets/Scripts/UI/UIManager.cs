using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// UI管理器 - 统一管理游戏中的所有UI元素
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI面板引用")]
    [SerializeField] private Canvas gameUICanvas;
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    
    [Header("玩家UI")]
    [SerializeField] private PlayerHealthBar playerHealthBar;
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private SwordStatusUI swordStatusUI;
    
    [Header("设置")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private TextMeshProUGUI debugText;
    
    // 单例模式
    public static UIManager Instance { get; private set; }
    
    // 事件
    public static System.Action OnGamePaused;
    public static System.Action OnGameResumed;
    
    private bool isPaused = false;
    
    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 确保Canvas设置正确
        SetupCanvas();
        
        // 初始化UI状态
        InitializeUI();
    }
    
    private void Start()
    {
        // 订阅游戏事件
        SubscribeToEvents();
        
        // 显示游戏UI
        ShowGameUI();
        
        // 立即强制设置一次UI位置
        ForceUIPositions();
    }
    
    private void Update()
    {
        // 检测暂停输入 (使用新的Input System)
        if (UnityEngine.InputSystem.Keyboard.current != null && 
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
        
        // 每隔一段时间检查并修复UI位置
        if (Time.time % 2f < Time.deltaTime) // 每2秒检查一次
        {
            ForceUIPositions();
        }
        
        // 更新调试信息
        if (showDebugInfo && debugText != null)
        {
            UpdateDebugInfo();
        }
    }
    
    /// <summary>
    /// 强制修正UI位置
    /// </summary>
    private void ForceUIPositions()
    {
        // 如果组件为空，重新查找
        if (playerHealthBar == null || scoreUI == null || swordStatusUI == null)
        {
            AutoFindUIComponents();
        }
        
        // 强制设置血量条位置
        if (playerHealthBar != null)
        {
            RectTransform healthBarRect = playerHealthBar.GetComponent<RectTransform>();
            if (healthBarRect != null)
            {
                // 检查位置是否正确，如果不正确则强制修正
                Vector2 expectedPos = new Vector2(100, -50);
                if (Vector2.Distance(healthBarRect.anchoredPosition, expectedPos) > 10f)
                {
                    healthBarRect.anchorMin = new Vector2(0, 1);
                    healthBarRect.anchorMax = new Vector2(0, 1);
                    healthBarRect.anchoredPosition = expectedPos;
                    Debug.Log("强制修正血量条位置");
                }
            }
        }
        
        // 强制设置分数UI位置
        if (scoreUI != null)
        {
            RectTransform scoreRect = scoreUI.GetComponent<RectTransform>();
            if (scoreRect != null)
            {
                Vector2 expectedPos = new Vector2(-100, -50);
                if (Vector2.Distance(scoreRect.anchoredPosition, expectedPos) > 10f)
                {
                    scoreRect.anchorMin = new Vector2(1, 1);
                    scoreRect.anchorMax = new Vector2(1, 1);
                    scoreRect.anchoredPosition = expectedPos;
                    Debug.Log("强制修正分数UI位置");
                }
            }
        }
        
        // 强制设置飞剑状态UI位置
        if (swordStatusUI != null)
        {
            RectTransform swordRect = swordStatusUI.GetComponent<RectTransform>();
            if (swordRect != null)
            {
                Vector2 expectedPos = new Vector2(0, 80);
                if (Vector2.Distance(swordRect.anchoredPosition, expectedPos) > 10f)
                {
                    swordRect.anchorMin = new Vector2(0.5f, 0);
                    swordRect.anchorMax = new Vector2(0.5f, 0);
                    swordRect.anchoredPosition = expectedPos;
                    Debug.Log("强制修正飞剑状态UI位置");
                }
            }
        }
    }
    
    /// <summary>
    /// 设置Canvas
    /// </summary>
    private void SetupCanvas()
    {
        if (gameUICanvas == null)
        {
            gameUICanvas = GetComponent<Canvas>();
        }
        
        if (gameUICanvas != null)
        {
            // 设置为Screen Space - Overlay
            gameUICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameUICanvas.sortingOrder = 10;
            
            // 添加Canvas Scaler用于适配不同分辨率
            CanvasScaler scaler = gameUICanvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameUICanvas.gameObject.AddComponent<CanvasScaler>();
            }
            
            // 设置适配参数
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f; // 平衡宽高适配
            
            // 添加GraphicRaycaster用于UI交互
            GraphicRaycaster raycaster = gameUICanvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                gameUICanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
    
    /// <summary>
    /// 设置UI元素的锚点和位置
    /// </summary>
    private void SetupUIAnchors()
    {
        Debug.Log("开始设置UI锚点...");
        
        // 设置血量条位置（左上角）
        if (playerHealthBar != null)
        {
            Debug.Log("设置血量条锚点...");
            RectTransform healthBarRect = playerHealthBar.GetComponent<RectTransform>();
            if (healthBarRect != null)
            {
                healthBarRect.anchorMin = new Vector2(0, 1); // 左上
                healthBarRect.anchorMax = new Vector2(0, 1); // 左上
                healthBarRect.anchoredPosition = new Vector2(100, -50); // 距离左上角的偏移
                healthBarRect.sizeDelta = new Vector2(250, 40); // 血量条大小
                Debug.Log($"血量条位置设置为: {healthBarRect.anchoredPosition}");
            }
            else
            {
                Debug.LogError("PlayerHealthBar没有RectTransform组件！");
            }
        }
        else
        {
            Debug.LogError("PlayerHealthBar组件为null！");
        }
        
        // 设置分数UI位置（右上角）
        if (scoreUI != null)
        {
            Debug.Log("设置分数UI锚点...");
            RectTransform scoreRect = scoreUI.GetComponent<RectTransform>();
            if (scoreRect != null)
            {
                scoreRect.anchorMin = new Vector2(1, 1); // 右上
                scoreRect.anchorMax = new Vector2(1, 1); // 右上
                scoreRect.anchoredPosition = new Vector2(-100, -50); // 距离右上角的偏移
                scoreRect.sizeDelta = new Vector2(300, 40); // 分数显示大小
                Debug.Log($"分数UI位置设置为: {scoreRect.anchoredPosition}");
            }
            else
            {
                Debug.LogError("ScoreUI没有RectTransform组件！");
            }
        }
        else
        {
            Debug.LogError("ScoreUI组件为null！");
        }
        
        // 设置飞剑状态UI位置（底部中心）
        if (swordStatusUI != null)
        {
            Debug.Log("设置飞剑状态UI锚点...");
            RectTransform swordRect = swordStatusUI.GetComponent<RectTransform>();
            if (swordRect != null)
            {
                swordRect.anchorMin = new Vector2(0.5f, 0); // 底部中心
                swordRect.anchorMax = new Vector2(0.5f, 0); // 底部中心
                swordRect.anchoredPosition = new Vector2(0, 80); // 距离底部的偏移
                swordRect.sizeDelta = new Vector2(250, 50); // 飞剑状态大小
                Debug.Log($"飞剑状态UI位置设置为: {swordRect.anchoredPosition}");
            }
            else
            {
                Debug.LogError("SwordStatusUI没有RectTransform组件！");
            }
        }
        else
        {
            Debug.LogError("SwordStatusUI组件为null！");
        }
        
        Debug.Log("UI锚点设置完成！");
    }
    
    /// <summary>
    /// 自动查找UI组件
    /// </summary>
    private void AutoFindUIComponents()
    {
        Debug.Log("开始自动查找UI组件...");
        
        // 如果Canvas为空，尝试获取自身组件
        if (gameUICanvas == null)
        {
            gameUICanvas = GetComponent<Canvas>();
            Debug.Log($"自动找到Canvas: {gameUICanvas != null}");
        }
        
        // 查找PlayerHealthBar
        if (playerHealthBar == null)
        {
            playerHealthBar = FindFirstObjectByType<PlayerHealthBar>();
            Debug.Log($"自动找到PlayerHealthBar: {playerHealthBar != null}");
        }
        
        // 查找ScoreUI
        if (scoreUI == null)
        {
            scoreUI = FindFirstObjectByType<ScoreUI>();
            Debug.Log($"自动找到ScoreUI: {scoreUI != null}");
        }
        
        // 查找SwordStatusUI
        if (swordStatusUI == null)
        {
            swordStatusUI = FindFirstObjectByType<SwordStatusUI>();
            Debug.Log($"自动找到SwordStatusUI: {swordStatusUI != null}");
        }
        
        // 查找gameUIPanel（通过名称查找）
        if (gameUIPanel == null)
        {
            GameObject foundPanel = GameObject.Find("GameUIPanel");
            if (foundPanel == null)
            {
                // 如果没有GameUIPanel，使用整个Canvas作为gameUIPanel
                gameUIPanel = gameObject;
                Debug.Log("使用Canvas作为GameUIPanel");
            }
            else
            {
                gameUIPanel = foundPanel;
                Debug.Log("找到GameUIPanel");
            }
        }
        
        Debug.Log("UI组件查找完成！");
    }
    
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        // 自动查找UI组件（如果引用为空）
        AutoFindUIComponents();
        
        // 默认显示游戏UI，隐藏暂停菜单
        if (gameUIPanel != null)
            gameUIPanel.SetActive(true);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        // 确保UI元素有正确的锚点设置
        SetupUIAnchors();
        
        // 初始化各个UI组件
        if (playerHealthBar != null)
            playerHealthBar.Initialize();
        
        if (scoreUI != null)
            scoreUI.Initialize();
        
        if (swordStatusUI != null)
            swordStatusUI.Initialize();
    }
    
    /// <summary>
    /// 订阅游戏事件
    /// </summary>
    private void SubscribeToEvents()
    {
        // 查找玩家血量系统并订阅事件
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdatePlayerHealth;
                Debug.Log("UI已连接到玩家血量系统");
                
                // 立即更新一次血量显示
                UpdatePlayerHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
        }
        
        // 启动分数更新
        if (scoreUI != null)
        {
            scoreUI.Initialize();
        }
        
        // 启动飞剑状态更新
        if (swordStatusUI != null)
        {
            swordStatusUI.Initialize();
        }
    }
    
    /// <summary>
    /// 显示游戏UI
    /// </summary>
    public void ShowGameUI()
    {
        if (gameUIPanel != null)
            gameUIPanel.SetActive(true);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }
    
    /// <summary>
    /// 切换暂停状态
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
        
        OnGamePaused?.Invoke();
        Debug.Log("游戏已暂停");
    }
    
    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        OnGameResumed?.Invoke();
        Debug.Log("游戏已恢复");
    }
    
    /// <summary>
    /// 更新玩家血量显示
    /// </summary>
    public void UpdatePlayerHealth(float currentHealth, float maxHealth)
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }
    
    /// <summary>
    /// 更新分数显示
    /// </summary>
    public void UpdateScore(int kills, float survivalTime)
    {
        if (scoreUI != null)
        {
            scoreUI.UpdateScore(kills, survivalTime);
        }
    }
    
    /// <summary>
    /// 更新飞剑状态
    /// </summary>
    public void UpdateSwordStatus(int availableSwords, int attackingSwords)
    {
        if (swordStatusUI != null)
        {
            swordStatusUI.UpdateStatus(availableSwords, attackingSwords);
        }
    }
    
    /// <summary>
    /// 增加击杀数
    /// </summary>
    public void AddKill()
    {
        if (scoreUI != null)
        {
            scoreUI.AddKill();
        }
    }
    
    /// <summary>
    /// 更新调试信息
    /// </summary>
    private void UpdateDebugInfo()
    {
        if (debugText != null)
        {
            debugText.text = $"FPS: {1f / Time.unscaledDeltaTime:F0}\n" +
                           $"Time Scale: {Time.timeScale}\n" +
                           $"Paused: {isPaused}";
        }
    }
    
    /// <summary>
    /// 显示游戏结束UI
    /// </summary>
    public void ShowGameOver(int finalScore, float survivalTime)
    {
        Debug.Log($"游戏结束！最终分数: {finalScore}, 生存时间: {survivalTime:F1}秒");
        // 这里可以显示游戏结束界面
    }
    
    private void OnDestroy()
    {
        // 取消事件订阅
        // GameManager.OnPlayerHealthChanged -= UpdatePlayerHealth;
        
        // 恢复时间比例
        Time.timeScale = 1f;
    }
}
