using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敌人生成器 - 在玩家周围随机生成鬼怪敌人（使用对象池优化）
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("生成设置")]
    [SerializeField] private GameObject[] enemyPrefabs;        // 敌人预制体数组（用于EnemyPool初始化）
    [SerializeField] private float spawnInterval = 3f;        // 生成间隔时间
    [SerializeField] private int maxEnemies = 20;              // 最大敌人数量
    [SerializeField] private float spawnVariance = 2f;        // 生成位置随机偏差
    
    [Header("生成区域")]
    [SerializeField] private float minSpawnRadius = 6f;       // 最小生成半径
    [SerializeField] private float maxSpawnRadius = 12f;      // 最大生成半径
    
    [Header("难度设置")]
    [SerializeField] private float difficultyIncreaseRate = 0.1f; // 难度增长率
    [SerializeField] private float minSpawnInterval = 0.5f;    // 最小生成间隔
    
    [Header("对象池设置")]
    [SerializeField] private bool useObjectPool = true;       // 是否使用对象池
    [SerializeField] private bool enableDebugLog = false;     // 是否启用调试日志
    
    // 运行时变量
    private Transform player;
    private float nextSpawnTime;
    private List<EnemyAI> spawnedEnemies = new List<EnemyAI>(); // 改为EnemyAI列表以支持对象池
    private float gameStartTime;
    
    // 难度系统
    private float currentDifficulty = 1f;
    
    private void Start()
    {
        // 寻找玩家
        FindPlayer();
        
        gameStartTime = Time.time;
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null)
            {
                Debug.LogWarning("EnemySpawner: 找不到玩家！");
            }
            return;
        }
        
        UpdateDifficulty();
        CleanupDestroyedEnemies();
        
        // 检查是否需要生成敌人
        if (Time.time >= nextSpawnTime && spawnedEnemies.Count < maxEnemies)
        {
            Debug.Log($"EnemySpawner: 尝试生成敌人 - 时间:{Time.time:F1}, 下次生成时间:{nextSpawnTime:F1}, 当前敌人数:{spawnedEnemies.Count}");
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }
    
    /// <summary>
    /// 寻找玩家
    /// </summary>
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    /// <summary>
    /// 更新游戏难度
    /// </summary>
    private void UpdateDifficulty()
    {
        float timePlayed = Time.time - gameStartTime;
        currentDifficulty = 1f + (timePlayed * difficultyIncreaseRate);
    }
    
    /// <summary>
    /// 清理已销毁或非活跃的敌人引用
    /// </summary>
    private void CleanupDestroyedEnemies()
    {
        if (useObjectPool)
        {
            // 对象池模式：移除null、非活跃或已死亡的敌人
            spawnedEnemies.RemoveAll(enemy => 
                enemy == null || 
                !enemy.gameObject.activeInHierarchy ||
                (enemy.GetComponent<HealthSystem>()?.IsAlive == false)
            );
        }
        else
        {
            // 传统模式：移除null的敌人
            spawnedEnemies.RemoveAll(enemy => enemy == null);
        }
    }
    
    /// <summary>
    /// 生成敌人
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) 
        {
            Debug.LogError("[EnemySpawner] enemyPrefabs数组为空！");
            return;
        }
        
        // 尝试多次寻找有效生成位置
        const int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            
            // 检查位置是否有效
            if (IsValidSpawnPosition(spawnPosition))
            {
                EnemyAI spawnedEnemy = null;
                
                if (useObjectPool && EnemyPool.Instance != null)
                {
                    // 使用对象池生成
                    spawnedEnemy = SpawnEnemyFromPool(spawnPosition);
                }
                else
                {
                    // 传统方式生成
                    spawnedEnemy = SpawnEnemyTraditional(spawnPosition);
                }
                
                if (spawnedEnemy != null)
                {
                    spawnedEnemies.Add(spawnedEnemy);
                    
                    if (enableDebugLog)
                        Debug.Log($"[EnemySpawner] 成功生成敌人: {spawnedEnemy.name} 在位置 {spawnPosition}, 当前难度: {currentDifficulty:F1}");
                    
                    return; // 成功生成，退出方法
                }
            }
        }
        
        Debug.LogWarning($"[EnemySpawner] 尝试 {maxAttempts} 次后仍未找到有效生成位置，跳过本次生成");
    }
    
    /// <summary>
    /// 使用对象池生成敌人
    /// </summary>
    private EnemyAI SpawnEnemyFromPool(Vector3 position)
    {
        if (EnemyPool.Instance == null)
        {
            Debug.LogError("[EnemySpawner] EnemyPool未初始化，切换到传统生成模式");
            return SpawnEnemyTraditional(position);
        }
        
        // 从对象池生成随机敌人
        EnemyAI enemy = EnemyPool.Instance.SpawnRandomEnemy(position, Quaternion.identity, currentDifficulty);
        
        if (enemy != null)
        {
            // 订阅敌人死亡事件以便从列表中移除
            HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.OnDeath += () => OnEnemyDeath(enemy);
            }
            
            if (enableDebugLog)
                Debug.Log($"[EnemySpawner] 从对象池生成敌人: {enemy.name}");
        }
        
        return enemy;
    }
    
    /// <summary>
    /// 传统方式生成敌人
    /// </summary>
    private EnemyAI SpawnEnemyTraditional(Vector3 position)
    {
        // 随机选择敌人类型
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        // 实例化敌人
        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
        EnemyAI enemy = enemyObj.GetComponent<EnemyAI>();
        
        if (enemy != null)
        {
            // 应用难度修正
            ApplyDifficultyToEnemyTraditional(enemyObj);
            
            if (enableDebugLog)
                Debug.Log($"[EnemySpawner] 传统方式生成敌人: {enemy.name}");
        }
        
        return enemy;
    }
    
    /// <summary>
    /// 敌人死亡回调
    /// </summary>
    private void OnEnemyDeath(EnemyAI enemy)
    {
        if (enemy == null) return;
        
        // 从生成列表中移除
        spawnedEnemies.Remove(enemy);
        
        if (useObjectPool && EnemyPool.Instance != null)
        {
            // 回收到对象池
            EnemyPool.Instance.RecycleEnemy(enemy);
            
            if (enableDebugLog)
                Debug.Log($"[EnemySpawner] 敌人已死亡并回收到对象池: {enemy.name}");
        }
        
        // 传统模式下，敌人会自动销毁，无需额外处理
    }
    
    /// <summary>
    /// 获取随机生成位置
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        // 在玩家周围的环形区域生成
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minSpawnRadius, maxSpawnRadius);
        
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        Vector3 basePosition = player.position + direction * distance;
        
        // 添加随机偏差
        Vector3 variance = new Vector3(
            Random.Range(-spawnVariance, spawnVariance),
            Random.Range(-spawnVariance, spawnVariance),
            0
        );
        
        return basePosition + variance;
    }
    
    /// <summary>
    /// 检查生成位置是否有效
    /// </summary>
    private bool IsValidSpawnPosition(Vector3 position)
    {
        // 检查是否离玩家太近
        float distanceToPlayer = Vector2.Distance(position, player.position);
        if (distanceToPlayer < minSpawnRadius)
        {
            return false;
        }
        
        // 检查是否已经有敌人在附近
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(position, 1f);
        foreach (Collider2D col in nearbyColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 对敌人应用难度修正（传统模式）
    /// </summary>
    private void ApplyDifficultyToEnemyTraditional(GameObject enemy)
    {
        // 增强敌人血量
        HealthSystem health = enemy.GetComponent<HealthSystem>();
        if (health != null)
        {
            float healthMultiplier = 1f + (currentDifficulty - 1f) * 0.5f;
            health.SetMaxHealth(health.MaxHealth * healthMultiplier, true);
        }
        
        // 增强敌人速度
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            // 通过反射或公开属性来修改移动速度
            // 这里需要EnemyAI脚本提供公开的修改方法
        }
    }
    
    /// <summary>
    /// 设置下次生成时间
    /// </summary>
    private void SetNextSpawnTime()
    {
        float adjustedInterval = spawnInterval / currentDifficulty;
        adjustedInterval = Mathf.Max(adjustedInterval, minSpawnInterval);
        
        nextSpawnTime = Time.time + adjustedInterval;
    }
    
    /// <summary>
    /// 强制生成一个敌人（用于测试）
    /// </summary>
    [ContextMenu("强制生成敌人")]
    public void ForceSpawnEnemy()
    {
        SpawnEnemy();
    }
    
    /// <summary>
    /// 清除所有敌人
    /// </summary>
    [ContextMenu("清除所有敌人")]
    public void ClearAllEnemies()
    {
        foreach (EnemyAI enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        spawnedEnemies.Clear();
    }
    
    // 调试用：绘制生成范围
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        
        // 最小生成半径
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, minSpawnRadius);
        
        // 最大生成半径
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxSpawnRadius);
    }
    
    // 公开属性
    public int CurrentEnemyCount => spawnedEnemies.Count;
    public float CurrentDifficulty => currentDifficulty;
}
