using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家控制器 - 处理玩家移动和基础行为
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("组件引用")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    // 输入相关
    private Vector2 moveInput;
    private PlayerInputActions inputActions;
    
    // 玩家状态
    public bool IsAlive { get; private set; } = true;
    
    private void Awake()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 初始化输入系统
        inputActions = new PlayerInputActions();
    }
    
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Attack.performed += OnAttack;
    }
    
    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Disable();
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
        
        // 应用移动
        Vector2 movement = moveInput * moveSpeed;
        rb.velocity = movement;
        
        // 根据移动方向翻转精灵
        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
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
    /// 玩家死亡
    /// </summary>
    public void Die()
    {
        IsAlive = false;
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
        transform.position = spawnPosition;
        IsAlive = true;
        rb.velocity = Vector2.zero;
        Debug.Log("玩家重生！");
    }
    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
