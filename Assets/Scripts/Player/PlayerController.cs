using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家控制器 - 处理玩家移动和基础行为
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField, Range(0.1f, 2.0f)] private float playerScale = 0.2f; // 玩家缩放比例
    
    [Header("组件引用")]
    [SerializeField] private InputActionAsset inputActions;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private TouchInputManager touchInputManager;
    private HealthSystem healthSystem;
    
    // 输入相关
    private Vector2 moveInput;
    private Vector2 touchMoveInput;
    
    // 玩家状态
    public bool IsAlive { get; private set; } = true;
    
    private void Awake()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        touchInputManager = FindFirstObjectByType<TouchInputManager>();
        healthSystem = GetComponent<HealthSystem>();
        
        // 设置物理属性
        if (rb != null)
        {
            rb.freezeRotation = true; // 冻结旋转，防止Player转圈
        }
        
        // 应用玩家缩放设置
        transform.localScale = Vector3.one * playerScale;
        
        // 订阅死亡事件
        if (healthSystem != null)
        {
            healthSystem.OnDeath += Die;
        }
    }
    
    private void OnEnable()
    {
        if (inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            if (playerMap != null)
            {
                playerMap.Enable();
                var moveAction = playerMap.FindAction("Move");
                var attackAction = playerMap.FindAction("Attack");
                
                if (moveAction != null)
                {
                    moveAction.performed += OnMove;
                    moveAction.canceled += OnMove;
                }
                
                if (attackAction != null)
                {
                    attackAction.performed += OnAttack;
                }
            }
        }
        
        // 订阅触控输入事件
        TouchInputManager.OnTouchMove += OnTouchMove;
    }
    
    private void OnDisable()
    {
        if (inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            if (playerMap != null)
            {
                var moveAction = playerMap.FindAction("Move");
                var attackAction = playerMap.FindAction("Attack");
                
                if (moveAction != null)
                {
                    moveAction.performed -= OnMove;
                    moveAction.canceled -= OnMove;
                }
                
                if (attackAction != null)
                {
                    attackAction.performed -= OnAttack;
                }
                
                playerMap.Disable();
            }
        }
        
        // 取消订阅触控输入事件
        TouchInputManager.OnTouchMove -= OnTouchMove;
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    /// <summary>
    /// 处理玩家移动
    /// </summary>
    private void HandleMovement()
    {
        if (!IsAlive) return;
        
        // 合并键盘输入和触控输入
        Vector2 finalInput = moveInput + touchMoveInput;
        
        // 限制移动向量长度，防止对角线移动过快
        if (finalInput.magnitude > 1f)
        {
            finalInput = finalInput.normalized;
        }
        
        // 应用移动
        Vector2 movement = finalInput * moveSpeed;
        rb.linearVelocity = movement;
        
        // 根据移动方向翻转精灵
        if (finalInput.x != 0)
        {
            spriteRenderer.flipX = finalInput.x < 0;
        }
    }
    
    /// <summary>
    /// 移动输入回调
    /// </summary>
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// 攻击输入回调
    /// </summary>
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!IsAlive) return;
        
        // TODO: 实现攻击逻辑
        Debug.Log("玩家攻击！");
    }
    
    /// <summary>
    /// 触控移动输入回调
    /// </summary>
    private void OnTouchMove(Vector2 direction)
    {
        touchMoveInput = direction;
    }
    
    /// <summary>
    /// 玩家死亡
    /// </summary>
    public void Die()
    {
        if (!IsAlive) return; // 防止重复死亡
        
        IsAlive = false;
        rb.linearVelocity = Vector2.zero; // 停止移动
        
        // TODO: 播放死亡动画
        Debug.Log("玩家死亡！");
        
        // 触发重生
        GameManager.Instance?.RespawnPlayer();
    }
    
    /// <summary>
    /// 玩家重生
    /// </summary>
    public void Respawn(Vector3 spawnPosition)
    {
        // 重置位置和旋转
        transform.position = spawnPosition;
        transform.rotation = Quaternion.identity;
        
        // 重置物理状态
        IsAlive = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true; // 确保重生后冻结旋转
        
        // 重置精灵状态
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }
        
        // 重置特效组件的原始位置
        HitEffect hitEffect = GetComponent<HitEffect>();
        if (hitEffect != null)
        {
            hitEffect.UpdateOriginalPosition();
        }
        
        // 恢复血量
        if (healthSystem != null)
        {
            healthSystem.Revive(1f); // 满血复活
        }
        
        Debug.Log("玩家重生！位置: " + spawnPosition);
    }
    
    private void OnDestroy()
    {
        // 取消事件订阅
        if (healthSystem != null)
        {
            healthSystem.OnDeath -= Die;
        }
    }
    
}
