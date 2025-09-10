using UnityEngine;

/// <summary>
/// 游戏管理器 - 管理游戏整体状态和逻辑
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("游戏设置")]
    [SerializeField] private Vector3 playerSpawnPoint = Vector3.zero; // 玩家重生点
    [SerializeField] private float respawnDelay = 2f;                 // 重生延迟时间
    
    [Header("组件引用")]
    [SerializeField] private GameObject playerPrefab;                 // 玩家预制体
    [SerializeField] private CameraFollow cameraFollow;               // 摄像机跟随组件
    
    // 单例模式
    public static GameManager Instance { get; private set; }
    
    // 当前游戏对象
    private PlayerController currentPlayer;
    
    // 游戏状态
    public bool IsGameRunning { get; private set; } = true;
    
    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        InitializeGame();
    }
    
    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        // 寻找当前场景中的玩家
        currentPlayer = FindFirstObjectByType<PlayerController>();
        
        if (currentPlayer == null && playerPrefab != null)
        {
            // 如果场景中没有玩家，创建一个
            SpawnPlayer();
        }
        
        // 设置摄像机跟随
        if (cameraFollow == null)
        {
            cameraFollow = FindFirstObjectByType<CameraFollow>();
        }
        
        if (cameraFollow != null && currentPlayer != null)
        {
            cameraFollow.SetTarget(currentPlayer.transform);
        }
    }
    
    /// <summary>
    /// 生成玩家
    /// </summary>
    private void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            GameObject playerObject = Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
            currentPlayer = playerObject.GetComponent<PlayerController>();
            
            // 确保玩家有正确的标签
            playerObject.tag = "Player";
            
            Debug.Log("玩家已生成在位置: " + playerSpawnPoint);
        }
    }
    
    /// <summary>
    /// 玩家重生
    /// </summary>
    public void RespawnPlayer()
    {
        if (currentPlayer == null) return;
        
        // 延迟重生
        Invoke(nameof(PerformRespawn), respawnDelay);
    }
    
    /// <summary>
    /// 执行重生
    /// </summary>
    private void PerformRespawn()
    {
        if (currentPlayer != null)
        {
            currentPlayer.Respawn(playerSpawnPoint);
            
            // 摄像机立即跟随到重生位置
            if (cameraFollow != null)
            {
                cameraFollow.TeleportToTarget();
            }
        }
    }
    
    /// <summary>
    /// 设置玩家重生点
    /// </summary>
    public void SetPlayerSpawnPoint(Vector3 newSpawnPoint)
    {
        playerSpawnPoint = newSpawnPoint;
    }
    
    /// <summary>
    /// 获取当前玩家
    /// </summary>
    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }
    
    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        IsGameRunning = false;
        Debug.Log("游戏结束！");
        // TODO: 显示游戏结束界面
    }
    
    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        IsGameRunning = true;
        // TODO: 重置游戏状态
        RespawnPlayer();
    }
}
