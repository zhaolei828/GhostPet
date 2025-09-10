using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 飞剑管理器 - 管理玩家周围环绕的飞剑和自动攻击
/// </summary>
public class FlyingSwordManager : MonoBehaviour
{
    [Header("飞剑设置")]
    [SerializeField] private GameObject swordPrefab;           // 飞剑预制体
    [SerializeField] private int maxSwords = 6;                // 最大飞剑数量
    [SerializeField] private float orbitRadius = 2f;           // 环绕半径
    [SerializeField] private float orbitSpeed = 2f;            // 环绕速度
    
    [Header("攻击设置")]
    [SerializeField] private float attackRange = 5f;           // 攻击范围
    [SerializeField] private float swordDamage = 25f;          // 飞剑伤害
    [SerializeField] private float attackCooldown = 0.5f;      // 攻击冷却时间
    [SerializeField] private float swordSpeed = 10f;           // 飞剑飞行速度
    [SerializeField] private float swordReturnSpeed = 8f;      // 飞剑返回速度
    
    // 飞剑管理
    private List<FlyingSword> swords = new List<FlyingSword>();
    private Transform player;
    private float lastAttackTime;
    
    // 目标检测
    private List<Transform> enemiesInRange = new List<Transform>();
    
    private void Start()
    {
        player = transform;
        InitializeSwords();
    }
    
    private void Update()
    {
        UpdateSwordOrbits();
        DetectEnemies();
        TryAutoAttack();
    }
    
    /// <summary>
    /// 初始化飞剑
    /// </summary>
    private void InitializeSwords()
    {
        for (int i = 0; i < maxSwords; i++)
        {
            CreateSword(i);
        }
    }
    
    /// <summary>
    /// 创建单个飞剑
    /// </summary>
    private void CreateSword(int index)
    {
        if (swordPrefab == null) return;
        
        GameObject swordObj = Instantiate(swordPrefab, transform.position, Quaternion.identity);
        FlyingSword sword = swordObj.GetComponent<FlyingSword>();
        
        if (sword == null)
        {
            sword = swordObj.AddComponent<FlyingSword>();
        }
        
        sword.Initialize(this, index, maxSwords);
        swords.Add(sword);
    }
    
    /// <summary>
    /// 更新飞剑环绕轨道
    /// </summary>
    private void UpdateSwordOrbits()
    {
        for (int i = 0; i < swords.Count; i++)
        {
            if (swords[i] != null && swords[i].IsOrbiting)
            {
                float angle = (i * 360f / maxSwords) + (Time.time * orbitSpeed * 57.3f); // 57.3f ≈ Mathf.Rad2Deg
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius,
                    0
                );
                
                Vector3 targetPosition = player.position + offset;
                swords[i].SetOrbitPosition(targetPosition);
            }
        }
    }
    
    /// <summary>
    /// 检测范围内的敌人
    /// </summary>
    private void DetectEnemies()
    {
        enemiesInRange.Clear();
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.position, attackRange);
        
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") && col.transform != player)
            {
                // 检查敌人是否还活着
                HealthSystem enemyHealth = col.GetComponent<HealthSystem>();
                if (enemyHealth != null && enemyHealth.IsAlive)
                {
                    enemiesInRange.Add(col.transform);
                }
            }
        }
    }
    
    /// <summary>
    /// 尝试自动攻击
    /// </summary>
    private void TryAutoAttack()
    {
        if (enemiesInRange.Count == 0) return;
        if (Time.time < lastAttackTime + attackCooldown) return;
        
        // 找到一个可用的飞剑
        FlyingSword availableSword = GetAvailableSword();
        if (availableSword == null) return;
        
        // 选择最近的敌人作为目标
        Transform target = GetNearestEnemy();
        if (target != null)
        {
            LaunchSword(availableSword, target);
            lastAttackTime = Time.time;
        }
    }
    
    /// <summary>
    /// 获取可用的飞剑
    /// </summary>
    private FlyingSword GetAvailableSword()
    {
        foreach (FlyingSword sword in swords)
        {
            if (sword != null && sword.IsAvailable)
            {
                return sword;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 获取最近的敌人
    /// </summary>
    private Transform GetNearestEnemy()
    {
        Transform nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            
            float distance = Vector2.Distance(player.position, enemy.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// 发射飞剑攻击目标
    /// </summary>
    public void LaunchSword(FlyingSword sword, Transform target)
    {
        if (sword == null || target == null) return;
        
        sword.LaunchAttack(target, swordDamage, swordSpeed, swordReturnSpeed);
        Debug.Log($"飞剑攻击 {target.name}!");
    }
    
    /// <summary>
    /// 飞剑返回完成回调
    /// </summary>
    public void OnSwordReturned(FlyingSword sword)
    {
        // 飞剑已经在FlyingSword脚本中处理状态重置
        Debug.Log("飞剑已返回轨道");
    }
    
    // 调试用：绘制攻击范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, orbitRadius);
    }
    
    // 属性访问器
    public float OrbitRadius => orbitRadius;
    public float OrbitSpeed => orbitSpeed;
    public Transform Player => player;
}
