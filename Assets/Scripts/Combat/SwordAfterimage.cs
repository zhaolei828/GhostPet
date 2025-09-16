using UnityEngine;

/// <summary>
/// 飞剑残影效果 - 单个残影的生命周期管理
/// </summary>
public class SwordAfterimage : MonoBehaviour
{
    [Header("残影设置")]
    [SerializeField] private float fadeSpeed = 3f; // 淡出速度
    [SerializeField] private float lifetime = 0.5f; // 残影持续时间
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float currentAlpha;
    private float timer;
    private MaterialPropertyBlock propBlock;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 确保MaterialPropertyBlock初始化
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            currentAlpha = originalColor.a;
        }
        else
        {
            Debug.LogWarning($"SwordAfterimage: 未找到SpriteRenderer组件在 {gameObject.name}");
        }
    }
    
    private void Update()
    {
        FadeOut();
        
        // 计时销毁
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            DestroyAfterimage();
        }
    }
    
    /// <summary>
    /// 初始化残影
    /// </summary>
    public void Initialize(Sprite swordSprite, Color swordColor, float initialAlpha = 0.6f)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // 设置残影外观
        spriteRenderer.sprite = swordSprite;
        originalColor = swordColor;
        currentAlpha = initialAlpha;
        
        // 设置初始透明度
        Color afterimageColor = originalColor;
        afterimageColor.a = currentAlpha;
        spriteRenderer.color = afterimageColor;
        
        // 设置渲染层级（稍微靠后）
        spriteRenderer.sortingOrder = -1;
        
        timer = 0f;
    }
    
    /// <summary>
    /// 淡出效果
    /// </summary>
    private void FadeOut()
    {
        if (spriteRenderer == null) return;
        
        // 确保propBlock已初始化
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }
        
        // 逐渐降低透明度
        currentAlpha -= fadeSpeed * Time.deltaTime;
        currentAlpha = Mathf.Max(0f, currentAlpha);
        
        // 使用MaterialPropertyBlock更新颜色，避免创建材质实例
        spriteRenderer.GetPropertyBlock(propBlock);
        Color newColor = originalColor;
        newColor.a = currentAlpha;
        propBlock.SetColor("_Color", newColor);
        spriteRenderer.SetPropertyBlock(propBlock);
        
        // 如果完全透明则销毁
        if (currentAlpha <= 0f)
        {
            DestroyAfterimage();
        }
    }
    
    /// <summary>
    /// 销毁残影
    /// </summary>
    private void DestroyAfterimage()
    {
        if (gameObject != null)
        {
            // 检查是否使用对象池系统
            if (SwordAfterimagePool.Instance != null)
            {
                // 回收到对象池
                SwordAfterimagePool.Instance.RecycleAfterimage(this);
            }
            else
            {
                // 传统模式：销毁对象
                Destroy(gameObject);
            }
        }
    }
    
    /// <summary>
    /// 设置残影参数（用于对象池）
    /// </summary>
    public void SetParameters(float newLifetime, float newFadeSpeed)
    {
        lifetime = newLifetime;
        fadeSpeed = newFadeSpeed;
        timer = 0f; // 重置计时器
    }
    
    /// <summary>
    /// 重置残影状态（用于对象池）
    /// </summary>
    public void ResetState()
    {
        timer = 0f;
        currentAlpha = originalColor.a;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    /// <summary>
    /// 强制回收到对象池
    /// </summary>
    public void ForceRecycle()
    {
        if (SwordAfterimagePool.Instance != null)
        {
            SwordAfterimagePool.Instance.RecycleAfterimage(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
