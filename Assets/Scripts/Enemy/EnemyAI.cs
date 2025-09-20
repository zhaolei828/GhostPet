using UnityEngine;

/// <summary>
/// 敌人AI控制器 - 控制鬼怪的行为和移动
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 3f;        // 移动速度
    [SerializeField] private float followRange = 10f;     // 跟随玩家的范围
    [SerializeField] private float attackRange = 1.5f;    // 攻击范围
    
    [Header("攻击设置")]
    [SerializeField] private float attackDamage = 10f;    // 攻击伤害
    [SerializeField] private float attackCooldown = 1f;   // 攻击冷却时间
    
    [Header("组件引用")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private HealthSystem healthSystem;
    
    // AI状态
    private Transform player;
    private float lastAttackTime;
    private bool isAlive = true;
    
    // AI状态枚举
    private enum EnemyState
    {
        Idle,       // 空闲
        Chasing,    // 追击玩家
        Attacking   // 攻击玩家
    }
    
    private EnemyState currentState = EnemyState.Idle;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthSystem = GetComponent<HealthSystem>();
    }
    
    private void Start()
    {
        // 寻找玩家
        FindPlayer();
        
        // 订阅死亡事件
        if (healthSystem != null)
        {
            healthSystem.OnDeath += OnDeath;
        }
    }
    
    private void Update()
    {
        if (!isAlive || player == null) return;
        
        UpdateAI();
    }
    
    private void FixedUpdate()
    {
        if (!isAlive) return;
        
        HandleMovement();
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
    /// 更新AI逻辑
    /// </summary>
    private void UpdateAI()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 状态机逻辑
        switch (currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer <= followRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;
                
            case EnemyState.Chasing:
                if (distanceToPlayer > followRange)
                {
                    currentState = EnemyState.Idle;
                }
                else if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                }
                break;
                
            case EnemyState.Attacking:
                if (distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chasing;
                }
                else
                {
                    TryAttack();
                }
                break;
        }
    }
    
    /// <summary>
    /// 处理移动
    /// </summary>
    private void HandleMovement()
    {
        Vector2 movement = Vector2.zero;
        
        if (currentState == EnemyState.Chasing || currentState == EnemyState.Attacking)
        {
            // 向玩家移动，但避开飞剑环绕区域
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 targetPosition = transform.position + (Vector3)(direction * moveSpeed * Time.fixedDeltaTime);
            
            // 检查目标位置是否在飞剑环绕区域内
            float distanceToPlayer = Vector2.Distance(targetPosition, player.position);
            float swordOrbitRadius = GetSwordOrbitRadius();
            
            if (distanceToPlayer <= swordOrbitRadius)
            {
                // 如果目标位置在飞剑区域内，寻找绕行路径
                movement = CalculateAvoidanceMovement(direction, swordOrbitRadius);
            }
            else
            {
                // 正常向玩家移动
                movement = direction * moveSpeed;
            }
            
            // 根据移动方向翻转精灵
            if (movement.x != 0)
            {
                spriteRenderer.flipX = movement.x < 0;
            }
        }
        
        rb.linearVelocity = movement;
    }
    
    /// <summary>
    /// 获取飞剑环绕半径
    /// </summary>
    private float GetSwordOrbitRadius()
    {
        FlyingSwordManager swordManager = FindFirstObjectByType<FlyingSwordManager>();
        if (swordManager != null)
        {
            return swordManager.OrbitRadius + 0.5f; // 添加一些缓冲距离
        }
        return 3f; // 默认值
    }
    
    /// <summary>
    /// 计算避让移动方向
    /// </summary>
    private Vector2 CalculateAvoidanceMovement(Vector2 originalDirection, float avoidRadius)
    {
        // 计算当前距离玩家的距离
        float currentDistance = Vector2.Distance(transform.position, player.position);
        
        // 如果已经在飞剑区域外，尝试沿着圆周移动
        if (currentDistance >= avoidRadius)
        {
            // 计算垂直于玩家方向的切线方向
            Vector2 tangent1 = new Vector2(-originalDirection.y, originalDirection.x);
            Vector2 tangent2 = new Vector2(originalDirection.y, -originalDirection.x);
            
            // 选择能更快接近玩家的切线方向
            Vector2 pos1 = (Vector2)transform.position + tangent1 * moveSpeed * Time.fixedDeltaTime;
            Vector2 pos2 = (Vector2)transform.position + tangent2 * moveSpeed * Time.fixedDeltaTime;
            
            float dist1 = Vector2.Distance(pos1, player.position);
            float dist2 = Vector2.Distance(pos2, player.position);
            
            return (dist1 < dist2) ? tangent1 * moveSpeed : tangent2 * moveSpeed;
        }
        else
        {
            // 如果在飞剑区域内，向外移动到安全距离
            Vector2 awayDirection = ((Vector2)transform.position - (Vector2)player.position).normalized;
            return awayDirection * moveSpeed;
        }
    }
    
    /// <summary>
    /// 尝试攻击玩家
    /// </summary>
    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }
    
    /// <summary>
    /// 攻击玩家
    /// </summary>
    private void Attack()
    {
        if (player == null) return;
        
        // 获取玩家的血量系统
        HealthSystem playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage, gameObject);
            Debug.Log($"{gameObject.name} 攻击了玩家，造成 {attackDamage} 点伤害");
        }
        
        // TODO: 播放攻击动画/特效
    }
    
    /// <summary>
    /// 死亡处理
    /// </summary>
    private void OnDeath()
    {
        isAlive = false;
        currentState = EnemyState.Idle;
        rb.linearVelocity = Vector2.zero;
        
        // 通知UI系统增加击杀数
        if (UIManager.Instance != null)
        {
            UIManager.Instance.AddKill();
        }
        // TODO: 播放死亡动画
        // TODO: 掉落物品
        
        // 传统模式：延迟销毁
        // TODO: 对象池系统集成后启用
        Destroy(gameObject, 2f);
        
        /* 对象池版本 - 待测试完成后启用
        // 检查是否使用对象池系统
        if (EnemyPool.Instance != null)
        {
            // 延迟回收到对象池（给动画时间播放）
            StartCoroutine(DelayedRecycle());
        }
        else
        {
            // 传统模式：延迟销毁
            Destroy(gameObject, 2f);
        }
        */
        
        Debug.Log($"{gameObject.name} 死亡了！击杀数+1");
    }
    
    /* 对象池版本 - 待测试完成后启用
    /// <summary>
    /// 延迟回收协程
    /// </summary>
    private System.Collections.IEnumerator DelayedRecycle()
    {
        // 等待一段时间以播放死亡动画
        yield return new WaitForSeconds(2f);
        
        // 回收到对象池
        if (EnemyPool.Instance != null && gameObject != null)
        {
            EnemyPool.Instance.RecycleEnemy(this);
        }
    }
    */
    
    /// <summary>
    /// 受到玩家攻击
    /// </summary>
    public void TakeDamageFromPlayer(float damage)
    {
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage);
        }
    }
    
    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnDeath -= OnDeath;
        }
    }
    
    // 调试用：绘制AI范围
    private void OnDrawGizmosSelected()
    {
        // 跟随范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        
        // 攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
