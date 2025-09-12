using UnityEngine;

/// <summary>
/// 单个飞剑控制器 - 处理飞剑的环绕、攻击和返回逻辑
/// </summary>
public class FlyingSword : MonoBehaviour
{
    [Header("视觉设置")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SwordAfterimageManager afterimageManager;
    
    // 飞剑状态
    private enum SwordState
    {
        Orbiting,   // 环绕状态
        Attacking,  // 攻击状态
        Returning   // 返回状态
    }
    
    private SwordState currentState = SwordState.Orbiting;
    private FlyingSwordManager manager;
    private int swordIndex;
    private int totalSwords;
    
    // 攻击相关
    private Transform attackTarget;
    private float damage;
    private float attackSpeed;
    private float returnSpeed;
    private Vector3 orbitPosition;
    private bool hasHitTarget = false;
    
    // 移动相关
    private Vector3 velocity;
    
    // 属性
    public bool IsOrbiting => currentState == SwordState.Orbiting;
    public bool IsAvailable => currentState == SwordState.Orbiting;
    
    private void Awake()
    {
        // 获取组件
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (afterimageManager == null)
            afterimageManager = GetComponent<SwordAfterimageManager>();
            
        // 如果没有残影管理器，自动添加一个
        if (afterimageManager == null)
            afterimageManager = gameObject.AddComponent<SwordAfterimageManager>();
    }
    
    private void Update()
    {
        UpdateMovement();
        UpdateRotation();
    }
    
    /// <summary>
    /// 初始化飞剑
    /// </summary>
    public void Initialize(FlyingSwordManager swordManager, int index, int total)
    {
        manager = swordManager;
        swordIndex = index;
        totalSwords = total;
        
        // 设置初始位置
        float angle = index * 360f / total;
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * manager.OrbitRadius,
            Mathf.Sin(angle * Mathf.Deg2Rad) * manager.OrbitRadius,
            0
        );
        transform.position = manager.Player.position + offset;
        orbitPosition = transform.position;
    }
    
    /// <summary>
    /// 设置环绕位置
    /// </summary>
    public void SetOrbitPosition(Vector3 position)
    {
        orbitPosition = position;
    }
    
    /// <summary>
    /// 发起攻击
    /// </summary>
    public void LaunchAttack(Transform target, float swordDamage, float speed, float retSpeed)
    {
        if (currentState != SwordState.Orbiting) return;
        
        attackTarget = target;
        damage = swordDamage;
        attackSpeed = speed;
        returnSpeed = retSpeed;
        hasHitTarget = false;
        
        currentState = SwordState.Attacking;
        
        // 启用残影效果
        if (afterimageManager != null)
            afterimageManager.EnableAfterimage();
        
        Debug.Log($"飞剑 {swordIndex} 开始攻击 {target.name}");
    }
    
    /// <summary>
    /// 更新移动逻辑
    /// </summary>
    private void UpdateMovement()
    {
        switch (currentState)
        {
            case SwordState.Orbiting:
                MoveToOrbitPosition();
                break;
                
            case SwordState.Attacking:
                MoveToTarget();
                break;
                
            case SwordState.Returning:
                ReturnToOrbit();
                break;
        }
    }
    
    /// <summary>
    /// 移动到环绕位置
    /// </summary>
    private void MoveToOrbitPosition()
    {
        // 在环绕模式下直接设置位置，确保飞剑始终跟随玩家
        transform.position = orbitPosition;
    }
    
    /// <summary>
    /// 移动向目标
    /// </summary>
    private void MoveToTarget()
    {
        if (attackTarget == null)
        {
            StartReturning();
            return;
        }
        
        Vector3 direction = (attackTarget.position - transform.position).normalized;
        transform.position += direction * attackSpeed * Time.deltaTime;
        
        // 检查是否到达目标或超过距离
        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);
        
        if (distanceToTarget < 0.5f && !hasHitTarget)
        {
            HitTarget();
        }
        else if (distanceToTarget > 10f) // 如果飞得太远，返回
        {
            StartReturning();
        }
    }
    
    /// <summary>
    /// 返回轨道
    /// </summary>
    private void ReturnToOrbit()
    {
        if (manager == null || manager.Player == null)
        {
            return;
        }
        
        Vector3 direction = (orbitPosition - transform.position).normalized;
        transform.position += direction * returnSpeed * Time.deltaTime;
        
        // 检查是否返回到轨道
        if (Vector3.Distance(transform.position, orbitPosition) < 0.3f)
        {
            currentState = SwordState.Orbiting;
            
            // 关闭残影效果
            if (afterimageManager != null)
                afterimageManager.DisableAfterimage();
            
            // 通知管理器
            manager?.OnSwordReturned(this);
        }
    }
    
    /// <summary>
    /// 命中目标
    /// </summary>
    private void HitTarget()
    {
        hasHitTarget = true;
        
        if (attackTarget != null)
        {
            // 对目标造成伤害
            HealthSystem targetHealth = attackTarget.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage, gameObject);
                Debug.Log($"飞剑命中 {attackTarget.name}，造成 {damage} 点伤害");
            }
            
            // 也可以尝试调用敌人的受伤方法
            EnemyAI enemyAI = attackTarget.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamageFromPlayer(damage);
            }
        }
        
        // 开始返回
        StartReturning();
    }
    
    /// <summary>
    /// 开始返回轨道
    /// </summary>
    private void StartReturning()
    {
        currentState = SwordState.Returning;
        attackTarget = null;
        
        // 生成一个返回时的残影
        if (afterimageManager != null)
            afterimageManager.SpawnImmediateAfterimage();
    }
    
    /// <summary>
    /// 更新旋转
    /// </summary>
    private void UpdateRotation()
    {
        Vector3 direction = Vector3.zero;
        
        switch (currentState)
        {
            case SwordState.Orbiting:
                // 环绕时指向运动方向
                direction = velocity.normalized;
                break;
                
            case SwordState.Attacking:
                if (attackTarget != null)
                {
                    direction = (attackTarget.position - transform.position).normalized;
                }
                break;
                
            case SwordState.Returning:
                direction = (orbitPosition - transform.position).normalized;
                break;
        }
        
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    /// <summary>
    /// 碰撞检测（备用方案）
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == SwordState.Attacking && !hasHitTarget)
        {
            if (other.transform == attackTarget)
            {
                HitTarget();
            }
        }
    }
}
