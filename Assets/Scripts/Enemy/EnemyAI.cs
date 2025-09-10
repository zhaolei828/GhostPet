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
            // 向玩家移动
            Vector2 direction = (player.position - transform.position).normalized;
            movement = direction * moveSpeed;
            
            // 根据移动方向翻转精灵
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        
        rb.linearVelocity = movement;
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
        
        // TODO: 播放死亡动画
        // TODO: 掉落物品
        
        // 延迟销毁
        Destroy(gameObject, 2f);
        
        Debug.Log($"{gameObject.name} 死亡了！");
    }
    
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
