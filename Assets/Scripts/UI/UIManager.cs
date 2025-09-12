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
    }
    
    private void Update()
    {
        // 检测暂停输入 (使用新的Input System)
        if (UnityEngine.InputSystem.Keyboard.current != null && 
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
        
        // 更新调试信息
        if (showDebugInfo && debugText != null)
        {
            UpdateDebugInfo();
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
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        // 默认显示游戏UI，隐藏暂停菜单
        if (gameUIPanel != null)
            gameUIPanel.SetActive(true);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
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
