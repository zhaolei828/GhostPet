using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敌人生成器 - 在玩家周围随机生成鬼怪敌人
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("生成设置")]
    [SerializeField] private GameObject[] enemyPrefabs;        // 敌人预制体数组
    [SerializeField] private float spawnInterval = 3f;        // 生成间隔时间
    [SerializeField] private int maxEnemies = 20;              // 最大敌人数量
    [SerializeField] private float spawnDistance = 8f;        // 生成距离（离玩家的距离）
    [SerializeField] private float spawnVariance = 2f;        // 生成位置随机偏差
    
    [Header("生成区域")]
    [SerializeField] private float minSpawnRadius = 6f;       // 最小生成半径
    [SerializeField] private float maxSpawnRadius = 12f;      // 最大生成半径
    
    [Header("难度设置")]
    [SerializeField] private float difficultyIncreaseRate = 0.1f; // 难度增长率
    [SerializeField] private float minSpawnInterval = 0.5f;    // 最小生成间隔
    
    // 运行时变量
    private Transform player;
    private float nextSpawnTime;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
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
            return;
        }
        
        UpdateDifficulty();
        CleanupDestroyedEnemies();
        
        // 检查是否需要生成敌人
        if (Time.time >= nextSpawnTime && spawnedEnemies.Count < maxEnemies)
        {
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
    /// 清理已销毁的敌人引用
    /// </summary>
    private void CleanupDestroyedEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }
    
    /// <summary>
    /// 生成敌人
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;
        
        // 随机选择敌人类型
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        // 计算生成位置
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // 检查位置是否有效
        if (IsValidSpawnPosition(spawnPosition))
        {
            // 生成敌人
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            // 应用难度修正
            ApplyDifficultyToEnemy(enemy);
            
            spawnedEnemies.Add(enemy);
            
            Debug.Log($"生成敌人: {enemy.name} 在位置 {spawnPosition}, 当前难度: {currentDifficulty:F1}");
        }
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
    /// 对敌人应用难度修正
    /// </summary>
    private void ApplyDifficultyToEnemy(GameObject enemy)
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
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
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
        Gizmos.DrawWireCircle(player.position, minSpawnRadius);
        
        // 最大生成半径
        Gizmos.color = Color.red;
        Gizmos.DrawWireCircle(player.position, maxSpawnRadius);
    }
    
    // 公开属性
    public int CurrentEnemyCount => spawnedEnemies.Count;
    public float CurrentDifficulty => currentDifficulty;
}
