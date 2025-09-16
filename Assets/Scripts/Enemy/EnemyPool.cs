using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 敌人对象池 - 专门管理敌人对象的池化
/// </summary>
public class EnemyPool : MonoBehaviour
{
    [Header("池配置")]
    [SerializeField] private GameObject[] enemyPrefabs;      // 敌人预制体数组
    [SerializeField] private int initialPoolSize = 15;      // 每种敌人的初始池大小
    [SerializeField] private int maxPoolSize = 50;          // 每种敌人的最大池大小
    [SerializeField] private bool allowPoolGrowth = true;   // 是否允许池增长
    [SerializeField] private bool enableDebugLog = false;   // 是否启用调试日志
    
    [Header("敌人管理")]
    [SerializeField] private float enemyLifetime = 60f;     // 敌人默认生命时长（防止无限积累）
    [SerializeField] private bool enableLifetimeLimit = true; // 是否启用生命时长限制
    
    // 单例模式
    public static EnemyPool Instance { get; private set; }
    
    // 多类型对象池字典
    private Dictionary<string, ObjectPool<EnemyAI>> enemyPools = new Dictionary<string, ObjectPool<EnemyAI>>();
    private Transform poolParent;
    
    // 活跃敌人跟踪
    private List<EnemyAI> activeEnemies = new List<EnemyAI>();
    private Dictionary<EnemyAI, Coroutine> lifetimeCoroutines = new Dictionary<EnemyAI, Coroutine>();
    
    // 统计信息
    public int TotalEnemyTypes => enemyPools.Count;
    public int TotalActiveEnemies => activeEnemies.Count;
    public int TotalPooledEnemies 
    { 
        get 
        {
            int total = 0;
            foreach (var pool in enemyPools.Values)
            {
                total += pool.TotalCount;
            }
            return total;
        }
    }
    
    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化所有敌人类型的对象池
    /// </summary>
    private void InitializePools()
    {
        // 创建池容器
        GameObject poolContainer = new GameObject("EnemyPools");
        poolContainer.transform.SetParent(transform);
        poolParent = poolContainer.transform;
        
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("[EnemyPool] 没有设置敌人预制体");
            return;
        }
        
        // 为每种敌人类型创建对象池
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            if (enemyPrefab == null) continue;
            
            CreatePoolForEnemyType(enemyPrefab);
        }
        
        if (enableDebugLog)
            Debug.Log($"[EnemyPool] 初始化完成: 创建了 {enemyPools.Count} 种敌人类型的对象池");
    }
    
    /// <summary>
    /// 为特定敌人类型创建对象池
    /// </summary>
    private void CreatePoolForEnemyType(GameObject enemyPrefab)
    {
        string enemyTypeName = enemyPrefab.name;
        
        // 验证预制体有EnemyAI组件
        if (enemyPrefab.GetComponent<EnemyAI>() == null)
        {
            Debug.LogError($"[EnemyPool] 预制体 {enemyTypeName} 缺少EnemyAI组件");
            return;
        }
        
        // 创建该类型的池容器
        GameObject typeContainer = new GameObject($"{enemyTypeName}_Pool");
        typeContainer.transform.SetParent(poolParent);
        
        // 创建对象池
        ObjectPool<EnemyAI> pool = new ObjectPool<EnemyAI>(
            prefab: enemyPrefab,
            poolParent: typeContainer.transform,
            initialSize: initialPoolSize,
            maxSize: maxPoolSize,
            allowGrowth: allowPoolGrowth,
            createFunction: null, // 使用默认创建函数
            onGet: OnGetEnemy,
            onRelease: OnReleaseEnemy,
            onDestroy: OnDestroyEnemy
        );
        
        enemyPools[enemyTypeName] = pool;
        
        if (enableDebugLog)
            Debug.Log($"[EnemyPool] 为 {enemyTypeName} 创建对象池完成");
    }
    
    /// <summary>
    /// 生成指定类型的敌人
    /// </summary>
    /// <param name="enemyTypeName">敌人类型名称</param>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">生成旋转</param>
    /// <param name="difficulty">难度系数</param>
    /// <returns>生成的敌人AI组件</returns>
    public EnemyAI SpawnEnemy(string enemyTypeName, Vector3 position, Quaternion rotation, float difficulty = 1f)
    {
        if (!enemyPools.ContainsKey(enemyTypeName))
        {
            Debug.LogError($"[EnemyPool] 未找到敌人类型: {enemyTypeName}");
            return null;
        }
        
        ObjectPool<EnemyAI> pool = enemyPools[enemyTypeName];
        EnemyAI enemy = pool.Get();
        
        if (enemy == null)
        {
            Debug.LogWarning($"[EnemyPool] 无法从池中获取敌人: {enemyTypeName}");
            return null;
        }
        
        // 设置位置和旋转
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
        
        // 应用难度设置
        ApplyDifficultyToEnemy(enemy, difficulty);
        
        // 添加到活跃列表
        activeEnemies.Add(enemy);
        
        // 设置生命时长限制
        if (enableLifetimeLimit)
        {
            Coroutine lifetimeCoroutine = StartCoroutine(EnemyLifetimeCoroutine(enemy));
            lifetimeCoroutines[enemy] = lifetimeCoroutine;
        }
        
        if (enableDebugLog)
            Debug.Log($"[EnemyPool] 生成敌人: {enemyTypeName} 在位置 {position}");
        
        return enemy;
    }
    
    /// <summary>
    /// 生成随机类型的敌人
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">生成旋转</param>
    /// <param name="difficulty">难度系数</param>
    /// <returns>生成的敌人AI组件</returns>
    public EnemyAI SpawnRandomEnemy(Vector3 position, Quaternion rotation, float difficulty = 1f)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("[EnemyPool] 没有可用的敌人预制体");
            return null;
        }
        
        string randomEnemyType = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)].name;
        return SpawnEnemy(randomEnemyType, position, rotation, difficulty);
    }
    
    /// <summary>
    /// 回收敌人到对象池
    /// </summary>
    /// <param name="enemy">要回收的敌人</param>
    public void RecycleEnemy(EnemyAI enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("[EnemyPool] 尝试回收null敌人");
            return;
        }
        
        // 从活跃列表移除
        activeEnemies.Remove(enemy);
        
        // 停止生命时长协程
        if (lifetimeCoroutines.TryGetValue(enemy, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            lifetimeCoroutines.Remove(enemy);
        }
        
        // 找到对应的对象池并回收
        string enemyTypeName = enemy.name.Replace("(Clone)", "").Trim();
        if (enemyPools.TryGetValue(enemyTypeName, out ObjectPool<EnemyAI> pool))
        {
            pool.Release(enemy);
            
            if (enableDebugLog)
                Debug.Log($"[EnemyPool] 回收敌人: {enemyTypeName}");
        }
        else
        {
            Debug.LogWarning($"[EnemyPool] 找不到敌人类型 {enemyTypeName} 的对象池");
        }
    }
    
    /// <summary>
    /// 敌人生命时长协程
    /// </summary>
    private IEnumerator EnemyLifetimeCoroutine(EnemyAI enemy)
    {
        yield return new WaitForSeconds(enemyLifetime);
        
        // 时间到了，自动回收敌人
        if (enemy != null && activeEnemies.Contains(enemy))
        {
            if (enableDebugLog)
                Debug.Log($"[EnemyPool] 敌人生命时长到期，自动回收: {enemy.name}");
            
            RecycleEnemy(enemy);
        }
    }
    
    /// <summary>
    /// 应用难度到敌人
    /// </summary>
    private void ApplyDifficultyToEnemy(EnemyAI enemy, float difficulty)
    {
        // 这里可以根据难度调整敌人属性
        HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            float baseHealth = 50f; // 基础血量
            float adjustedHealth = baseHealth * (1f + (difficulty - 1f) * 0.5f);
            healthSystem.SetMaxHealth(adjustedHealth, true);
        }
        
        // 可以添加更多难度相关的调整，如移动速度、攻击力等
    }
    
    /// <summary>
    /// 获取敌人时的回调
    /// </summary>
    private void OnGetEnemy(EnemyAI enemy)
    {
        if (enemy == null) return;
        
        // 激活对象
        enemy.gameObject.SetActive(true);
        
        // 重置状态
        enemy.enabled = true;
        
        // 重置健康系统
        HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.Revive(1f); // 满血复活
        }
        
        // 重置物理
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        
        if (enableDebugLog)
            Debug.Log($"[EnemyPool] OnGet: {enemy.name}");
    }
    
    /// <summary>
    /// 释放敌人时的回调
    /// </summary>
    private void OnReleaseEnemy(EnemyAI enemy)
    {
        if (enemy == null) return;
        
        // 停止所有协程
        enemy.StopAllCoroutines();
        
        // 重置到池父级
        string enemyTypeName = enemy.name.Replace("(Clone)", "").Trim();
        if (enemyPools.TryGetValue(enemyTypeName, out ObjectPool<EnemyAI> pool))
        {
            enemy.transform.SetParent(poolParent.Find($"{enemyTypeName}_Pool"));
        }
        
        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.localRotation = Quaternion.identity;
        
        // 禁用对象
        enemy.gameObject.SetActive(false);
        
        if (enableDebugLog)
            Debug.Log($"[EnemyPool] OnRelease: {enemy.name}");
    }
    
    /// <summary>
    /// 销毁敌人时的回调
    /// </summary>
    private void OnDestroyEnemy(EnemyAI enemy)
    {
        if (enemy == null) return;
        
        if (enableDebugLog)
            Debug.Log($"[EnemyPool] OnDestroy: {enemy.name}");
    }
    
    /// <summary>
    /// 回收所有活跃敌人
    /// </summary>
    public void RecycleAllEnemies()
    {
        var enemiesToRecycle = new List<EnemyAI>(activeEnemies);
        foreach (EnemyAI enemy in enemiesToRecycle)
        {
            RecycleEnemy(enemy);
        }
        
        Debug.Log($"[EnemyPool] 已回收所有活跃敌人: {enemiesToRecycle.Count} 个");
    }
    
    /// <summary>
    /// 清空所有对象池
    /// </summary>
    public void ClearAllPools()
    {
        // 先回收所有活跃敌人
        RecycleAllEnemies();
        
        // 清空所有对象池
        foreach (var pool in enemyPools.Values)
        {
            pool.Clear();
        }
        
        Debug.Log("[EnemyPool] 已清空所有敌人对象池");
    }
    
    /// <summary>
    /// 预加载指定类型的敌人
    /// </summary>
    public void PreloadEnemies(string enemyTypeName, int count)
    {
        if (enemyPools.TryGetValue(enemyTypeName, out ObjectPool<EnemyAI> pool))
        {
            pool.Preload(count);
            Debug.Log($"[EnemyPool] 预加载 {enemyTypeName}: {count} 个");
        }
    }
    
    /// <summary>
    /// 预加载所有类型的敌人
    /// </summary>
    public void PreloadAllEnemies(int countPerType)
    {
        foreach (var kvp in enemyPools)
        {
            kvp.Value.Preload(countPerType);
        }
        Debug.Log($"[EnemyPool] 预加载所有敌人类型: 每种 {countPerType} 个");
    }
    
    /// <summary>
    /// 获取池统计信息
    /// </summary>
    public string GetPoolStats()
    {
        string stats = $"EnemyPool统计:\n";
        stats += $"- 活跃敌人: {TotalActiveEnemies}\n";
        stats += $"- 池中总数: {TotalPooledEnemies}\n";
        stats += $"- 敌人类型: {TotalEnemyTypes}\n";
        
        foreach (var kvp in enemyPools)
        {
            stats += $"  · {kvp.Key}: {kvp.Value.GetPoolStats()}\n";
        }
        
        return stats;
    }
    
    /// <summary>
    /// 验证所有池的完整性
    /// </summary>
    public bool ValidateAllPools()
    {
        bool allValid = true;
        foreach (var kvp in enemyPools)
        {
            if (!kvp.Value.ValidatePool())
            {
                Debug.LogError($"[EnemyPool] 池完整性验证失败: {kvp.Key}");
                allValid = false;
            }
        }
        return allValid;
    }
    
    /// <summary>
    /// 获取可用的敌人类型名称
    /// </summary>
    public string[] GetAvailableEnemyTypes()
    {
        string[] types = new string[enemyPools.Count];
        enemyPools.Keys.CopyTo(types, 0);
        return types;
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        
        // 清空所有池
        ClearAllPools();
    }
    
    // 调试方法
    [ContextMenu("显示池状态")]
    private void ShowPoolStatus()
    {
        Debug.Log($"[EnemyPool] {GetPoolStats()}");
    }
    
    [ContextMenu("验证池完整性")]
    private void ValidatePoolIntegrity()
    {
        bool isValid = ValidateAllPools();
        Debug.Log($"[EnemyPool] 池完整性验证: {(isValid ? "通过" : "失败")}");
    }
    
    [ContextMenu("回收所有敌人")]
    private void RecycleAll()
    {
        RecycleAllEnemies();
    }
}
